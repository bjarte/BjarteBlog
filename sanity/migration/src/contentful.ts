import {readFileSync} from 'node:fs'

export const LOCALE = 'nb-NO'

// --- Contentful export shapes (only what we consume) ---------------------------

export interface CfLink {
  sys: {type: 'Link'; linkType: 'Asset' | 'Entry'; id: string}
}

export interface CfSys {
  id: string
  type: 'Entry' | 'Asset'
  createdAt: string
  updatedAt: string
  contentType?: {sys: {id: string}}
}

/** A localized field is keyed by locale, e.g. `{ 'nb-NO': value }`. */
export type Localized<T> = Record<string, T>

export interface CfEntry {
  sys: CfSys
  fields: Record<string, Localized<unknown> | undefined>
}

export interface CfAssetFile {
  url: string
  fileName: string
  contentType: string
  details: {size: number; image?: {width: number; height: number}}
}

export interface CfAsset {
  sys: CfSys
  fields: {
    title?: Localized<string>
    description?: Localized<string>
    file?: Localized<CfAssetFile>
  }
}

/** Contentful rich-text node (document tree). */
export interface CfNode {
  nodeType: string
  data: {
    uri?: string
    target?: CfLink
  }
  content?: CfNode[]
  value?: string
  marks?: {type: string}[]
}

export interface CfExport {
  contentTypes: unknown[]
  entries: CfEntry[]
  assets: CfAsset[]
  locales: {code: string}[]
}

// --- Loading & indexing --------------------------------------------------------

export interface LoadedExport {
  raw: CfExport
  entries: CfEntry[]
  entriesById: Map<string, CfEntry>
  assetsById: Map<string, CfAsset>
}

export function loadExport(path: string): LoadedExport {
  const raw = JSON.parse(readFileSync(path, 'utf8')) as CfExport
  const entriesById = new Map(raw.entries.map((e) => [e.sys.id, e]))
  const assetsById = new Map(raw.assets.map((a) => [a.sys.id, a]))
  return {raw, entries: raw.entries, entriesById, assetsById}
}

export function entriesOfType(load: LoadedExport, contentTypeId: string): CfEntry[] {
  return load.entries.filter((e) => e.sys.contentType?.sys.id === contentTypeId)
}

/** Unwrap the single-locale value of a Contentful field. */
export function loc<T>(field: Localized<unknown> | undefined): T | undefined {
  if (!field) return undefined
  return field[LOCALE] as T | undefined
}
