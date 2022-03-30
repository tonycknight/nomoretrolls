
# Discord Integration

You'll need to set up 2 Discord bot accounts. You can register your own [Discord bots here](https://discord.com/developers/applications/), and [Discord.Net gives a good introduction guide](https://discordnet.dev/guides/getting_started/first-bot.html).

## Chat bot

The Chat bot will scan discord users' messages and react to them accordingly. 

You'll need the bot's client ID, secret and token in the configuration, and give the bot the following permissions:

* ``send messages``
* ``create public threads``
* ``create private threads``
* ``send messages in threads``
* ``manage messages``
* ``manage threads``
* ``embed links``
* ``attach files``
* ``read message history``
* ``add reactions``

---

## Admin bot

The bot is managed by another, discrete bot. You'll want to register this bot to a private channel where only authorised admins have access.

You'll need the bot's client ID, secret and token in the configuration, and give the bot the following permissions:

* ``send messages``
* ``read message history``

---

## Single Discord server

The Chat bot and Admin bot must both be on the same server, to help resolve user names, permissions, etc.
