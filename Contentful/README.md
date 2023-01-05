# Contentful import/export

## Install Contentful CLI

> npm install -g contentful-cli

## How to export your current setup

To get a complete dump of the data model (to commit to git), update export-config.json with the appropriate settings.

At least you need to enter a valid Management API key.

Export command:
> contentful space export --config .\export-config.json

## Import the exported content model to a new space

To import your content-model into a new space, update import-config.json with the appropriate settings.

At least you need to enter a valid Management API key.

Import command:
> contentful space import --config .\import-config.json