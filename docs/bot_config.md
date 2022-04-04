# Configuration

You can configure the bot either through environment variables or a configuration file.

## Environment variables

Environment variables may be used, for instance injection into a Docker container.

| Variable Name | Value |
| - | - |
| ``nomoretrolls_Discord_DiscordClientId`` | The bot's client ID |
| ``nomoretrolls_Discord_DiscordClientToken`` | The bot's client token|
| ``nomoretrolls_MongoDb_Connection`` | Connection string to Mongo |
| ``nomoretrolls_MongoDb_DatabaseName`` | The Mongo database name |

To use environment variables, start the bot without the ``-c`` option: 

``.\nomoretrolls.exe start``

## Configuration file

The bot can be configured by a single JSON configuration file. The file's schema is:

```json
{
  "discord": {
    "clientToken": "<bot token>",
    "clientId": "<client id>"
  },
  "mongoDb": {
    "connection": "<mongo DB connection string>",
    "databaseName": "<database name>"
  },
  "telemetry": {
    "logMessageContent": false
  }
}
```

The configuration file's path is provided by the ``-c`` start option: 

``.\nomoretrolls.exe start -c <path to config file>``

