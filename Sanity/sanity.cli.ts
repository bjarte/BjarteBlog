import { defineCliConfig } from 'sanity/cli'

const projectId = process.env.SANITY_STUDIO_PROJECT_ID || 'your-project-id'
const dataset = process.env.SANITY_STUDIO_DATASET || 'production'

export default defineCliConfig({
  deployment: {
    appId: 'q8hxf5bdsv57btq4jlfrzabz',
  },
  api: {
    projectId,
    dataset,
  },
})
