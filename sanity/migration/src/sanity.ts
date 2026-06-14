import {createClient, SanityClient} from '@sanity/client'
import {SanityEnv} from './config.js'
import {SanityDoc} from './transformers.js'

export function makeClient(env: SanityEnv): SanityClient {
  return createClient({
    projectId: env.projectId,
    dataset: env.dataset,
    token: env.token,
    apiVersion: '2024-01-01',
    useCdn: false,
  })
}

/**
 * Upsert documents in chunked transactions. `createOrReplace` makes re-runs
 * idempotent. Referenced docs (categories, imageAssets) must be uploaded before
 * the docs that reference them.
 */
export async function uploadDocs(
  client: SanityClient,
  docs: SanityDoc[],
  chunkSize = 50,
): Promise<void> {
  for (let i = 0; i < docs.length; i += chunkSize) {
    const chunk = docs.slice(i, i + chunkSize)
    let tx = client.transaction()
    for (const doc of chunk) tx = tx.createOrReplace(doc)
    await tx.commit({visibility: 'async'})
  }
}
