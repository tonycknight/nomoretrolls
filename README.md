# nomoretrolls - A Discord bot to counter the awkward

## ** :warning: This is a work in progress! **


[![Build & Release](https://github.com/tonycknight/nomoretrolls/actions/workflows/build.yml/badge.svg)](https://github.com/tonycknight/nomoretrolls/actions/workflows/build.yml)

Discord getting unruly? Some folks don't know how to read rooms, nor when to shut up? Tired of long fireside chats, and being ignored? **Then it's time to troll the trolls!**

--- 

# Features

* Ramped-up annoyance of blacklisted users

* Ramped-up silencing of shouting users

Details on features and their configuration can be found [here](./docs/bot_actions.md).

--- 

# Getting started

* You'll need to install the [.Net 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to build. All project dependencies are managed by the build process.

* You'll need access to a Mongo DB server. [MongoDB Atlas](https://www.mongodb.com/atlas/database) will be useful for initial trials.

* To build, run

  ``build.bat``

  or 

  ``dotnet tool restore``

  ``dotnet fake run "build.fsx"``

* To run from the command line:

  ``.\nomoretrolls.exe start -c <path to config file>``

  See [discord configuration](./docs/discord_config.md) for details on configuration.

--- 

# Installation

Discord Configuration: see [discord configuration](./docs/discord_config.md).

Bot Configuration: see [bot configuration](./docs/bot_config.md).

