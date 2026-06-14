import 'dotenv/config'
import {existsSync, readdirSync} from 'node:fs'
import {dirname, join, resolve} from 'node:path'
import {fileURLToPath} from 'node:url'

const __dirname = dirname(fileURLToPath(import.meta.url))

/** Repo root, three levels up from sanity/migration/src. */
const repoRoot = resolve(__dirname, '..', '..', '..')

export const isDryRun = process.argv.includes('--dry-run')

/**
 * Resolve the Contentful export JSON. Honours CONTENTFUL_EXPORT, otherwise picks
 * the newest contentful-export-*.json under Contentful/exports.
 */
export function resolveExportPath(): string {
  const override = process.env.CONTENTFUL_EXPORT?.trim()
  if (override) {
    const p = resolve(override)
    if (!existsSync(p)) throw new Error(`CONTENTFUL_EXPORT does not exist: ${p}`)
    return p
  }

  const exportsDir = join(repoRoot, 'Contentful', 'exports')
  if (!existsSync(exportsDir)) {
    throw new Error(`Could not find Contentful exports directory at ${exportsDir}`)
  }
  const candidates = readdirSync(exportsDir)
    .filter((f) => f.startsWith('contentful-export-') && f.endsWith('.json'))
    .sort()
  if (candidates.length === 0) {
    throw new Error(`No contentful-export-*.json found in ${exportsDir}`)
  }
  return join(exportsDir, candidates[candidates.length - 1])
}

export interface SanityEnv {
  projectId: string
  dataset: string
  token: string
}

/** Read Sanity connection settings; only required for a real (non-dry) run. */
export function readSanityEnv(): SanityEnv {
  const projectId = process.env.SANITY_PROJECT_ID?.trim()
  const dataset = process.env.SANITY_DATASET?.trim() || 'production'
  const token = process.env.SANITY_WRITE_TOKEN?.trim()

  const missing: string[] = []
  if (!projectId) missing.push('SANITY_PROJECT_ID')
  if (!token) missing.push('SANITY_WRITE_TOKEN')
  if (missing.length > 0) {
    throw new Error(
      `Missing ${missing.join(', ')} in .env. Required for a real migrate run ` +
        `(not needed for --dry-run).`,
    )
  }

  return {projectId: projectId!, dataset, token: token!}
}

/** Output directory for dry-run artifacts (this app's own folder). */
export const migrationDir = resolve(__dirname, '..')
