
# Blacklisting a user

Blacklisting a user will cause the bot to steadily intrude in their chat. First, the bot will add reactions; once more chat is added then the bot will start replying directly. If the user continues to use chat, their messages will be deleted. If the target stops chatting, the process will reset.

### Admin bot commands

#### Blacklisting a user

``!user blacklist <user name>``

e.g.

``!user blacklist joe_troll#1234``

#### Clearing a user from the blacklist

``!user allow <user name>``

e.g.

``!user allow joe_troll#1234``

#### Listing blacklisted users

``!user list``

---

# No more shouting

People who shout are annoying. The bot will detect shouters and warn them, first by giving reactions, then steadily replying to them direct and eventually deleting their messages. Like blacklisting, the cycle is not permanent and will reset after a period of time.

### Admin bot commands

None: the bot detects all shouting on all channels it guards.

---

# Workflow configuration

Each of the above actions are managed by ``workflows``, and the configuration reflects this.

### Admin bot commands

``!workflow list`` - to list workflows and their status

``!workflow disable <workflow name>`` - to disable a workflow for all users

``!workflow enable <workflow name>`` - to enable a workflow

Valid ``<workflow name>`` are:

* ``blacklist``
* ``shouting``
