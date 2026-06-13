# Contentful import/export

## Install Contentful CLI

> npm install -g contentful-cli

## How to export your current setup

To get a complete dump of the data model (to commit to git), update export-config.json with the appropriate settings.

At least you need to enter a valid Management API key.

Export command
> contentful space export --config .\export-config.json

## How to export, v.2

Instead of adding API management key and space id to a config file, just add them to the command line once.

Install contentful-cli with Mise

> mise use --global npm:contentful-cli

Authenticate and generate API key

> contentful login

Select space to export

> contentful space use

Export to contentful-export.json

> contentful export

## Import the exported content model to a new space

To import your content-model into a new space, update import-config.json with the appropriate settings.

At least you need to enter a valid Management API key.

Import command:
> contentful space import --config .\import-config.json
