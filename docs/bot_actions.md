# Bot administration

Admin commands are only available to users with an ``Administrator`` permission: [see here for details](https://discord.com/moderation/1500000176222-201:-Permissions-on-Discord). It's advisable that you use a private channel for these.

All configuration actions apply to all guilds and channels that the bot watches. 

# Blacklisting a user

Blacklisting a user will cause the bot to steadily intrude in their chat. First, the bot will add reactions; once more chat is added then the bot will start replying directly. If the user continues to use chat, their messages will be deleted. If the target stops chatting, the process will reset.

## Setting a blacklist

``!blacklist <user name>``

e.g.

``!blacklist "joe_troll#1234"``

Optionally, a cool down time can be given in minutes, e.g. to blacklist ``joe_troll#1234`` for 60 minutes:

``!blacklist joe_troll#1234 60``

A user name with spaces must be quoted, e.g.

``!blacklist "joe troll#1234" 60``

## Clearing a blacklist

``!deleteblacklist <user name>``

e.g.

``!deleteblacklist "joe_troll#1234"``

---

# Waking up a user

Want to repeatedly annoy a troll? What better way than to give them an alarm call at the wrong time of day! 

## Adding a knock

Knocking a user will send a message - ``Knock knock`` - to them, where the message is immediately deleted. Just give a [cron expression](https://en.wikipedia.org/wiki/Cron) in UTC, and the bot will wake them up.

``!knock <user name> <duration in minutes> <cron frequency for UTC>``

e.g.

``!knock "JoeUser#1234" 60 "*/1 * * * 1"`` to knock Joe every minute on Mondays for an hour.

Note the double quotes for the user name and the cron expression.


## Clearing a knock

``!deleteknock <user name>``

e.g.

``!deleteknock "JoeUser#1234"``

---

# Listing configured users

Get a list of users that have some configuration against them:

``!users``

---

# No more shouting

People who shout are annoying. The bot will detect shouters and warn them, first by giving reactions, then steadily replying to them direct and eventually deleting their messages. Like blacklisting, the cycle is not permanent and will reset after a period of time.

Shout detection is automatic if the ``shouting`` feature is enabled (see below).

---

# Feature configuration

Each of the above actions are managed by ``features``, and the configuration reflects this.

### Admin bot commands

``!features`` - to list features and their status

``!disable <feature name>`` - to disable a feature for all users

``!enable <feature name>`` - to enable a feature

Valid ``<feature name>`` are:

* ``blacklist``
* ``shouting``
