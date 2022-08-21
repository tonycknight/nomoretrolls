# Bot administration

Admin commands are only available to users with an ``Administrator`` permission: [see here for details](https://discord.com/moderation/1500000176222-201:-Permissions-on-Discord). It's advisable that you use a private channel for these.

All configuration actions apply to all guilds and channels that the bot watches. 

## Active & Passive features

Some functions are passive - in that they are "always on" & are not specifically targetted - whereas some are active - they must be explicitly started against a named user.

Passive functions:
* Shout detection
* Alternating Caps detection

Active functions:
* Blacklisting
* Knocking
* Emote labelling

## Feature configuration

Each of the above features are managed by the ``!features`` command, and the configuration reflects this.

### Admin bot commands

``!features`` - to list features and their status

``!disable <feature name>`` - to disable a feature for all users

``!enable <feature name>`` - to enable a feature

Valid ``<feature name>`` values are:

* ``blacklist``
* ``shouting``
* ``altcaps``
* ``emotes``
* ``knocking``

## Listing configured users

Get a list of users that have some configuration against them:

``!users``

## Double quoting user names

Discord allows spaces within user names. The bot command parser will need these user names double quoted. 

For example: ``Joe User#1234`` should be given as ``"Joe User#1234"``

---

# Blacklisting a user

Blacklisting a user will cause the bot to steadily intrude in their chat. First, the bot will add reactions; once more chat is added then the bot will start replying directly. If the user continues to use chat, their messages will be deleted. If the target stops chatting, the process will reset.

## Setting a blacklist

``!blacklist <user name>``

e.g.

``!blacklist "joe troll#1234"``

Optionally, a cool down time can be given in minutes, e.g. to blacklist ``joe troll#1234`` for 60 minutes:

``!blacklist "joe troll#1234" 60``

## Clearing a blacklist

``!deleteblacklist <user name>``

e.g.

``!deleteblacklist "joe troll#1234"``

---

# Waking up a user

Want to repeatedly annoy a troll? What better way than to give them an alarm call at the wrong time of day! 

## Adding a knock

Knocking a user will send a message - ``Knock knock`` - to them, where the message is immediately deleted. Just give a [cron expression](https://en.wikipedia.org/wiki/Cron) in UTC, and the bot will wake them up.

``!knock <user name> <duration in minutes> <cron frequency for UTC>``

e.g.

``!knock "JoeUser#1234" 60 "*/1 * * * 1"`` to knock Joe every minute on Mondays for an hour.

Note the double quotes for the the cron expression!


## Clearing a knock

``!deleteknock <user name>``

e.g.

``!deleteknock "JoeUser#1234"``

---

# Emote labelling

Sometimes just continually applying emotes can help change a troll's behaviour. 

## The available emotes

The bot has a curated set of standard emotes:

``!emotes``

## Adding emotes

Adding an emote set to a user will result in a randomly picked emote being applied to every message for a period of time:

``!emote <user name> <duration in minutes> <emote list name>``

e.g. to add emotes from the ``shrug`` set for 60 minutes:

``!emote "joe troll#1234" 60 shrug``

## Clearing emotes

``!deleteemote <user name>``

---

# No more shouting

People who continually use all capitals (aka "shouting") are annoying. The bot will detect shouters and warn them; first by giving reactions, then steadily replying to them direct and eventually deleting their messages. Like blacklisting, the cycle is not permanent and will reset after a period of time.

Shout detection is automatic if the ``shouting`` feature is enabled.

---

# No more AlTeNaTiNg CaPs

For some, continuous use of alternating caps text is a troll. The bot will detect and warn the troll; first by giving reactions, then steadily replying to them direct and eventually deleting their messages. Like blacklisting, the cycle is not permanent and will reset after a period of time.

Alt caps detection is automatic if the ``altcaps`` feature is enabled.

---

