# nomoretrolls - A Discord bot to counter the awkward

## **This is a work in progress!**


[![Build & Release](https://github.com/tonycknight/nomoretrolls/actions/workflows/build.yml/badge.svg)](https://github.com/tonycknight/nomoretrolls/actions/workflows/build.yml)

Discord getting unruly? Some folks don't know when to shut up? Tired of long fireside chats, and being ignored?

**Then it's time to out-troll the trolls!**


--- 

# Getting started

* You'll need to install the [.Net 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to build. All project dependencies are managed by the build process.

* You'll need access to a Mongo DB server. [MongoDB Atlas](https://www.mongodb.com/atlas/database) will be useful for initial trials.

* To build, run [build.bat](./build.bat) 

  or 

  ``dotnet tool restore``

  ``dotnet fake run "build.fsx"``

* To run from the command line:

  ``.\nomoretrolls.exe start <path to config file>``

# Configuration

**Work in progress! This is not sufficiently secure for a production environment. Please make sure this is in a secure folder!**

```json
{
  "discord": {
    "clientToken": "<bot token>",
    "clientId": "<client id>",
    "clientSecret": "<client secret>"
  },
  "discordAdmin": {
    "clientToken": "<bot token>",
    "clientId": "<client id>",
    "clientSecret": "<client secret>"
  },
  "mongoDb": {
    "connection": "<mongo DB connection name>"
  },
  "telemetry": {
    "logMessageContent": false
  }
}
```

# Discord Integration

You'll need to set up 2 Discord bot accounts, and these will need configuration. You can register your own [Discord bots here](https://discord.com/developers/applications/), and [Discord.Net gives a good introduction guide](https://discordnet.dev/guides/getting_started/first-bot.html).

## Chat bot

The Chat bot will scan discord users' messages and react. 

You'll need the bot's client ID, secret and token in the configuration, and give the bot the following permissions:

* send messages
* create public threads
* create private threads
* send messages in threads
* manage messages
* manage threads
* embed links
* attach files
* read message history
* add reactions

## Admin bot

The bot is managed by a discrete bot. You'll want to register this bot to a private channel where only authorised admins have access.

You'll need the bot's client ID, secret and token in the configuration, and give the bot the following permissions:

* send messages
* read message history

