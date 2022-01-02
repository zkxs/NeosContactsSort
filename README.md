# NeosContactsSort

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that sorts contacts Better™.

Relevant Neos issue: [#2596](https://github.com/Neos-Metaverse/NeosPublic/issues/2596).

## Sorting Order
I've made a few noteworthy changes to the sorting order:
- No longer sorts by most recent message timestamp
- Incoming friend requests are now the first category, preceding Online friends
- Neos Bot is now forced to the top of the Online friends category
- Sent Requests are separated from Offline friends, and have a yellow background color

### Vanilla Sort
1. Friends with unread messages
2. Ties broken by online status
   1. Online Friends
   2. Incoming Friend Requests
   3. Away Friends
   4. Busy Friends
   5. Offline Friends and Sent Requests
3. Further ties broken by most recent message
4. Even further ties broken by username alphabetical order

### Modded Sort
1. Friends with unread messages
2. Ties broken by online status
   1. Incoming Friend Requests
   2. Neos Bot
   3. Online Friends
   4. Away Friends
   5. Busy Friends
   6. Sent Requests (background color changed from gray to yellow!)
   7. Offline Friends
3. Further ties broken by username alphabetical order

## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
2. Place [NeosContactsSort.dll](https://github.com/zkxs/NeosContactsSort/releases/latest/download/NeosContactsSort.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Neos logs.
