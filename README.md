# nomoretrolls - A Discord bot to counter the awkward

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

* You'll need access to a [Mongo DB](/docs/mongo.md) server. [MongoDB Atlas](https://www.mongodb.com/atlas/database) will be useful for initial trials.

* To build, run

  ``build.bat``

  or 

  ``dotnet tool restore``

  ``dotnet fake run "build.fsx"``

* To run from the command line using environment variables :

  ``.\nomoretrolls.exe start``

  See [discord configuration](./docs/discord_config.md) for details on configuration.

* To run from the command line with a configuration file:

  ``.\nomoretrolls.exe start -c <path to config file>``

  See [discord configuration](./docs/discord_config.md) for details on configuration.

--- 

# Installation

A [docker image](https://github.com/users/tonycknight/packages/container/package/nomoretrolls) is built by this repository:

```
docker pull ghcr.io/tonycknight/nomoretrolls:<tag>
```

To run, provide environment variable configuration:

```
docker run -it --rm -e nomoretrolls_Discord_DiscordClientId=<discord client id> 
                    -e nomoretrolls_Discord_DiscordClientToken=<discord client token> 
                    -e nomoretrolls_MongoDb_Connection=<mongo DB connection string> 
                    -e nomoretrolls_MongoDb_DatabaseName=<mongo DB name> ghcr.io/tonycknight/nomoretrolls:<tag>
```

## Configuration

For Discord Configuration, see [discord configuration](./docs/discord_config.md).

For Bot Configuration, see [bot configuration](./docs/bot_config.md).

