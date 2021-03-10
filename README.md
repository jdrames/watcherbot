## ForceMX Discord Bot
This is the bot that is used to monitor and display updates to groups for games from www.the100.io.

---
### System Requirements
**Active Internet Connection**  
**.net 5.0 sdk & runtime**  
**Discord Developer Account**  
**The100.io API Access Token**  
**Available MongoDB Server**
  
You must include a settings.json file with the following format for the bot to connect and begin listening for requests.
If you run the app without the settings.json file it will create a template settings.json file that you can use to fill
in the required values.

```json
{
  "discord": {
    "token": "{your discord bot token here}",
    "command_prefixes": [
      "!"
    ],
    "shards": 1
  },
  "mongo": {
    "connection_string": "{your mongo server connection string here}",
    "database_name": "FmxMonitorBot",
    "collection_names": {
      "games": "games",
      "guilds": "guilds"
    }
  },
  "the100": {
    "token": "{your www.the100.io API token here}"
  }
}
```
---
### Usage

### User Commands
`!games` *displays a pagable list of games currently available*

### Admin Commands
`!posthere` *displays game notifications in the specified discord channel*  
`!addgroup {int: groupId}` *add a group via their the100 group number to monitor games from. Requries that the group is available for the API token used for the100*  
`!removegroup {int: groupId}` *removes a group from monitoring*  
`!showgamemessages {bool: showMessages}` *set to allow the bot to post notifications for game [new game, time change, etc]*  
`!showplayermessages {bool: showMessages}` *set to allow the bot to post notifications for player activity [join game, leave game]*

