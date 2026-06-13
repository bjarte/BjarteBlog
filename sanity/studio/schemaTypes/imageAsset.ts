import {defineType, defineField} from 'sanity'

/**
 * Image — a reusable image document with metadata.
 *
 * Named `imageAsset` (titled "Image") so it does not shadow Sanity's built-in
 * `image` object type. Follows Sanity conventions: the image carries its own
 * `alt` and `caption`, with crop/hotspot enabled.
 */
export const imageAsset = defineType({
  name: 'imageAsset',
  title: 'Image',
  type: 'document',
  fields: [
    defineField({
      name: 'title',
      title: 'Title',
      type: 'string',
    }),
    defineField({
      name: 'image',
      title: 'Image',
      type: 'image',
      options: {hotspot: true},
      validation: (Rule) => Rule.required(),
      fields: [
        {
          name: 'alt',
          title: 'Alternative text',
          type: 'string',
          description:
            'Alternative text for those who cannot see the image. Keep it short, and skip it if the image is purely decorative.',
        },
        {
          name: 'caption',
          title: 'Caption',
          type: 'text',
          rows: 2,
          description: 'Displayed below the image and gives extra context.',
        },
      ],
    }),
    defineField({
      name: 'credit',
      title: 'Credit',
      type: 'string',
      description: 'Name of photographer or creator of the image to give them credit.',
    }),
  ],
  preview: {
    select: {
      title: 'title',
      media: 'image',
    },
  },
})
