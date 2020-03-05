# PlanetbaseMultiplayer
A multiplayer mod for Planetbase.

*This mod is currently incomplete. It's in a very early stage of development and many features don't work properly. I wouldn't suggest playing it right now as you will often encounter crashes and desync.*

## Working features
Parts of the game which are already synced:
- Placing buildings
- Placing connections
- Placing components inside buildings
- Resource production and mine ore production
- Recycling
- Character walking
- Interactions

## To-Do List
- Ship syncing
- Weather syncing
- Meteor syncing
- Trading

And much more...

## How to contribute
Just send me a pull request! I'll try to merge it as soon as possible.
There's one rule for now though. I'd like to avoid modifying the Assembly-CSharp dll if possible. Small changes like changing private or protected fields to public are acceptable. Try to add all the patches to the Patcher project instead.

## Setting up a dev environment
- Clone this repo
- Create a copy of the game (and backup your save data as well in case something goes horribly wrong)
- Locate the Assembly-CSharp DLL in your game files. (Planetbase_Data > Managed). 
- Replace it with the DLL provided in the References directory of this project.
- Compile the project and enter the bin directory of the Planetbase.Patcher project. 
- Copy the relevant DLLs (0Harmony, Lidgren.Network, PlanetbaseMultiplayer.Client, PlanetbaseMultiplayer.SharedLibs, PlanetbasePatcher) to the Managed directory.
- Run the server binary and connect!
