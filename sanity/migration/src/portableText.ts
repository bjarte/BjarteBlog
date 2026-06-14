import {randomBytes} from 'node:crypto'
import {CfNode, LoadedExport, loc} from './contentful.js'

/** Random _key for Portable Text members (blocks, spans, markDefs, objects). */
export function genKey(): string {
  return randomBytes(6).toString('hex')
}

// --- Portable Text output shapes ----------------------------------------------

export interface PtSpan {
  _type: 'span'
  _key: string
  text: string
  marks: string[]
}

export interface PtMarkDef {
  _type: 'link'
  _key: string
  href: string
}

export interface PtBlock {
  _type: 'block'
  _key: string
  style: string
  markDefs: PtMarkDef[]
  children: PtSpan[]
  listItem?: 'bullet' | 'number'
  level?: number
}

export interface PtCode {
  _type: 'code'
  _key: string
  code: string
  language?: string
}

/** Deferred image: alt only, no `asset` ref yet (binary uploaded later). */
export interface PtImage {
  _type: 'image'
  _key: string
  alt?: string
}

export type PtMember = PtBlock | PtCode | PtImage

export interface ConvertContext {
  load: LoadedExport
  docId: string
  /**
   * Note that a Contentful asset is referenced, for the image manifest. `opts.key`
   * is the `_key` of the body block (image block or the block containing a link),
   * and `opts.markKey` is the link markDef key — so the upload step can patch the
   * exact spot.
   */
  recordAsset: (assetId: string, field: string, opts?: {key?: string; markKey?: string}) => void
  warn: (msg: string) => void
}

const MARK_MAP: Record<string, string> = {
  bold: 'strong',
  italic: 'em',
  underline: 'underline',
  code: 'code',
}

const HEADINGS: Record<string, string> = {
  'heading-1': 'h1',
  'heading-2': 'h2',
  'heading-3': 'h3',
  'heading-4': 'h4',
  'heading-5': 'h5',
  'heading-6': 'h6',
}

function httpsUrl(url: string): string {
  return url.startsWith('//') ? `https:${url}` : url
}

const CTFASSETS_HOST = /(?:images|assets|videos|downloads)\.ctfassets\.net/i

/** If `url` is a Contentful asset CDN URL, return its asset id (if we have it). */
function contentfulAssetIdFromUrl(url: string, load: LoadedExport): string | undefined {
  if (!CTFASSETS_HOST.test(url)) return undefined
  try {
    const u = new URL(url.startsWith('//') ? `https:${url}` : url)
    // /<spaceId>/<assetId>/<hash>/<fileName>
    const assetId = u.pathname.split('/').filter(Boolean)[1]
    return assetId && load.assetsById.has(assetId) ? assetId : undefined
  } catch {
    return undefined
  }
}

/** Build spans for inline nodes, threading link markDefs through `markDefs`. */
function spansFrom(
  nodes: CfNode[],
  ctx: ConvertContext,
  markDefs: PtMarkDef[],
  blockKey: string,
  extraMarkKeys: string[] = [],
): PtSpan[] {
  const spans: PtSpan[] = []

  for (const node of nodes) {
    switch (node.nodeType) {
      case 'text': {
        const decorators = (node.marks ?? [])
          .map((m) => MARK_MAP[m.type])
          .filter((v): v is string => Boolean(v))
        spans.push({
          _type: 'span',
          _key: genKey(),
          text: node.value ?? '',
          marks: [...decorators, ...extraMarkKeys],
        })
        break
      }
      case 'hyperlink': {
        const href = node.data.uri
        const keys = [...extraMarkKeys]
        if (href) {
          const def: PtMarkDef = {_type: 'link', _key: genKey(), href}
          markDefs.push(def)
          keys.push(def._key)
          // A plain hyperlink may still point at a Contentful asset CDN URL,
          // which dies after migration — record it so the href can be repointed.
          const assetId = contentfulAssetIdFromUrl(href, ctx.load)
          if (assetId) {
            ctx.recordAsset(assetId, 'body-link-href', {key: blockKey, markKey: def._key})
          }
        }
        spans.push(...spansFrom(node.content ?? [], ctx, markDefs, blockKey, keys))
        break
      }
      case 'entry-hyperlink': {
        const targetId = node.data.target?.sys.id
        const slug = targetId ? resolveEntrySlug(ctx.load, targetId) : undefined
        const keys = [...extraMarkKeys]
        if (slug) {
          const def: PtMarkDef = {_type: 'link', _key: genKey(), href: `/${slug}`}
          markDefs.push(def)
          keys.push(def._key)
        } else {
          ctx.warn(`Unresolved entry-hyperlink target ${targetId} — link dropped, text kept`)
        }
        spans.push(...spansFrom(node.content ?? [], ctx, markDefs, blockKey, keys))
        break
      }
      case 'asset-hyperlink': {
        const assetId = node.data.target?.sys.id
        const keys = [...extraMarkKeys]
        if (assetId) {
          const asset = ctx.load.assetsById.get(assetId)
          const url = loc<{url: string}>(asset?.fields.file)?.url
          if (url) {
            const def: PtMarkDef = {_type: 'link', _key: genKey(), href: httpsUrl(url)}
            markDefs.push(def)
            keys.push(def._key)
            ctx.recordAsset(assetId, 'body-link-href', {key: blockKey, markKey: def._key})
          }
        }
        spans.push(...spansFrom(node.content ?? [], ctx, markDefs, blockKey, keys))
        break
      }
      default:
        // Inline embedded entries etc. — keep any nested text, drop the wrapper.
        if (node.content?.length) {
          spans.push(...spansFrom(node.content, ctx, markDefs, blockKey, extraMarkKeys))
        } else {
          ctx.warn(`Dropped inline node of type "${node.nodeType}"`)
        }
    }
  }

  return spans
}

function resolveEntrySlug(load: LoadedExport, entryId: string): string | undefined {
  const entry = load.entriesById.get(entryId)
  return loc<string>(entry?.fields.slug)
}

function makeBlock(
  node: CfNode,
  style: string,
  ctx: ConvertContext,
  listItem?: 'bullet' | 'number',
  level?: number,
): PtBlock {
  const _key = genKey()
  const markDefs: PtMarkDef[] = []
  let children = spansFrom(node.content ?? [], ctx, markDefs, _key)
  if (children.length === 0) {
    children = [{_type: 'span', _key: genKey(), text: '', marks: []}]
  }
  const block: PtBlock = {_type: 'block', _key, style, markDefs, children}
  if (listItem) {
    block.listItem = listItem
    block.level = level ?? 1
  }
  return block
}

function flattenList(
  listNode: CfNode,
  ctx: ConvertContext,
  listItem: 'bullet' | 'number',
  level: number,
  out: PtMember[],
): void {
  for (const item of listNode.content ?? []) {
    if (item.nodeType !== 'list-item') continue
    for (const child of item.content ?? []) {
      if (child.nodeType === 'unordered-list') {
        flattenList(child, ctx, 'bullet', level + 1, out)
      } else if (child.nodeType === 'ordered-list') {
        flattenList(child, ctx, 'number', level + 1, out)
      } else {
        const style = HEADINGS[child.nodeType] ?? 'normal'
        out.push(makeBlock(child, style, ctx, listItem, level))
      }
    }
  }
}

/** Flatten a Contentful rich-text document to plain text (used for code blocks). */
export function richTextToPlainText(doc: CfNode | undefined): string {
  if (!doc?.content) return ''
  const lines: string[] = []
  const collect = (node: CfNode): string => {
    if (node.nodeType === 'text') return node.value ?? ''
    return (node.content ?? []).map(collect).join('')
  }
  for (const node of doc.content) {
    lines.push(collect(node))
  }
  return lines.join('\n').replace(/\r\n?/g, '\n')
}

function codeBlockMember(ctx: ConvertContext, entryId: string): PtCode | undefined {
  const entry = ctx.load.entriesById.get(entryId)
  if (!entry || entry.sys.contentType?.sys.id !== 'codeBlock') return undefined
  const language = loc<string>(entry.fields.language)
  const code = richTextToPlainText(loc<CfNode>(entry.fields.code))
  const member: PtCode = {_type: 'code', _key: genKey(), code}
  if (language) member.language = language
  return member
}

/** Convert a Contentful rich-text document into Portable Text members. */
export function richTextToPortableText(
  doc: CfNode | undefined,
  ctx: ConvertContext,
): PtMember[] {
  if (!doc?.content) return []
  const out: PtMember[] = []

  for (const node of doc.content) {
    if (node.nodeType === 'paragraph') {
      out.push(makeBlock(node, 'normal', ctx))
    } else if (HEADINGS[node.nodeType]) {
      out.push(makeBlock(node, HEADINGS[node.nodeType], ctx))
    } else if (node.nodeType === 'blockquote') {
      out.push(makeBlock(node, 'blockquote', ctx))
    } else if (node.nodeType === 'unordered-list') {
      flattenList(node, ctx, 'bullet', 1, out)
    } else if (node.nodeType === 'ordered-list') {
      flattenList(node, ctx, 'number', 1, out)
    } else if (node.nodeType === 'hr') {
      // No schema equivalent — dropped.
    } else if (node.nodeType === 'embedded-entry-block') {
      const targetId = node.data.target?.sys.id
      const code = targetId ? codeBlockMember(ctx, targetId) : undefined
      if (code) {
        out.push(code)
      } else {
        const refType = targetId
          ? ctx.load.entriesById.get(targetId)?.sys.contentType?.sys.id
          : undefined
        ctx.warn(`Dropped embedded-entry-block (target ${targetId}, type ${refType ?? '?'})`)
      }
    } else if (node.nodeType === 'embedded-asset-block') {
      const assetId = node.data.target?.sys.id
      const asset = assetId ? ctx.load.assetsById.get(assetId) : undefined
      const alt = loc<string>(asset?.fields.description) ?? loc<string>(asset?.fields.title)
      const img: PtImage = {_type: 'image', _key: genKey()}
      if (alt) img.alt = alt
      if (assetId) ctx.recordAsset(assetId, 'body-image', {key: img._key})
      out.push(img)
    } else {
      ctx.warn(`Dropped top-level node of type "${node.nodeType}"`)
    }
  }

  return out
}
