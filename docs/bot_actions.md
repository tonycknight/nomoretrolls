# Bot administration

Admin commands are only available to users with an ``Administrator`` permission: [see here for details](https://discord.com/moderation/1500000176222-201:-Permissions-on-Discord). It's advisable that you use a private channel for these.

All configuration actions apply to all guilds and channels that the bot watches. 

# Blacklisting a user

Blacklisting a user will cause the bot to steadily intrude in their chat. First, the bot will add reactions; once more chat is added then the bot will start replying directly. If the user continues to use chat, their messages will be deleted. If the target stops chatting, the process will reset.

### Admin bot commands

#### Blacklisting a user

``!blacklist <user name>``

e.g.

``!blacklist joe_troll#1234``

Optionally, a cool down time can be given in minutes, e.g. to blacklist ``joe_troll#1234`` for 60 minutes:

``!blacklist joe_troll#1234 60``

A user name with spaces must be quoted, e.g.

``!blacklist "joe troll#1234" 60``

#### Clearing a user from the blacklist

``!allow <user name>``

e.g.

``!allow joe_troll#1234``

#### Listing blacklisted users

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
