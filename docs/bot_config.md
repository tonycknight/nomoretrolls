# Configuration

The bot's configured by a single JSON configuration file. The schema is:

```json
{
  "discord": {
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

The configuration contains plain text values: please remember to place the file in a secure location!