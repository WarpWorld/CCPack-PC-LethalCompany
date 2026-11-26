**1.2.0** :
```
Fixes for v81, and the mod now runs on the current Crowd Control plugin framework.
- Effects relayed through chat (every effect for non-host players; Spawn Body, Revive, Doors, Quota, Weather and more for hosts) failed with MissingMethodException because the chat command handler could not load.
- Kill / Kill Crewmate failed (KillPlayer signature changed), and enemy spawn patching failed at round start.
- Give Item effects always retried on v81 (FirstEmptyItemSlot gained a parameter).
- New v80 creatures can be spawned: Cadaver Growths, Cadaver Bloom, Kidnapper Fox, Feiopar and Stingray (also at crewmates). Kidnapper Fox is refused on moons without Vain Shrouds, where it would vanish at once.
- Spawn Forest Giant no longer sometimes spawns a Giant Kiwi, and Spawn Butler no longer matches Butler Bees.
- Give Weed Killer and Weather - Cloudy work (their effect IDs never matched the mod).
- New: on-screen Crowd Control overlay with connection status and active timed effects (F8 toggles it), F9 reconnects.
- Timed effects pause and resume cleanly and report their remaining time to Crowd Control.
- The startup log shows the real mod version (1.1.18 still reported 1.1.17).
- Requires ConnectorLib.JSON.dll next to CrowdControl.dll (included).
```

**1.1.18** :
```
Crash report fixes: stop calling Unity FindObjectOfType/FindObjectsByType off the CC request thread (moved to the main-thread ActionQueue; fixes native AV crash on scene changes).
```

**1.1.17** :
```
Fixes for v80
```

**1.1.16** :
```
Landmines, Turrets, and Spike traps now despawn on round end properly. 
```

**1.1.15** :
```
Fix Landmines respawning when leaving / landing on a new level
Add Spawn Turret, Spawn Spike Trap at Players / Crewmates
```

**1.1.14** :  
```
Actually Fix Monster Spawns for all Levels.  
(Please Note: LethalLevelLoader Moons are not 100% tested, so use with warning that some spawns may not work)  
Inverse Teleporting Players and Crewmates  
Player State Check fixes for timed effects  
Add Tulip Snake, add Sapsucker Spawns  
```

**1.1.13** :  
```
More Fixes
```
**1.1.11** :
```
Fix Incompatibility with LethalLevelLoader
Fully Fix Item ID's.
Fix Mimics / Masks
```

**1.1.10** :

```
Fix All Item ID's across the board
Vehicle Options (Give Cruiser, Spring Seat, Turn on Engine, Turn off Engine, Destroy Vehicle)
Thanks again to @TheUnknownCod3r : https://www.github.com/TheUnknownCod3r/
```
**1.1.9** :

```
Fixes for some items ids
Big thanks, again, to @TheUnknownCod3r
```

**1.1.8** :

```
Updates to support v55
Standardize item ids
Add Spawn Radmech
Big thanks to @TheUnknownCod3r
```

**1.1.7** :

```
Fixed Landmine effects to spawn outside the ship 
Added Drunk effect
```

**1.1.5** :

```
Fixed mimic spawns
Fixed nutcracker issue
Added landmine spawns
```

**1.1.4** :

```
Giant/leviathan no longer spawn inside
Enemy spawns delay on modded moons that don't contain that enemy
```

**1.1.3** :

```
removed logging text to console
Added nutcracker spawn
Added crew health effects
```

**1.1.2** :

```
Rework of effect targeting. All effects that would target the host, now target the player they were sent to. Effect descriptions and names have been update to show this.
Added new effect Spawn Mimic
Added new effect Give Tragedy Mask
Added new effect Give Comedy Mask
Added versioning in more places
Changed the default Crowd Control communication port to disable older versions of our mod
```

**1.1.1** :

```
Support for game version v47
Added custom terminal command to see what version of the plugin is running and who is connected to Crowd Control
```


**1.1.0** : 
```
Revamp of spawn effects.  These effects will queue until they can be spawned! (In the facility or outside) If they can't they will be automagically refunded.
 - Rename of spawn effects to match public names
 - Limit the number of spawns that can happen at once
 - Enemies that normally spawn in the facility will only spawn in the facility when using Crowd Control and vice versa for outside enemies
 - Spawning dead bodies should be more stable when spammed (even tho spamming 30+ at once will lag the game hard, it will eventually recover)
 Fixed a bug found in testing that allows users to receive more items then they can carry, which will bug out the player and prevent them from interacting with anything else in game!
 Other stability improvements
 ```

**1.0.9** :
```
Fixes for weather effects, including the Flood effect
```

**1.0.8** : 
```
Fix to this changelog...
```

**1.0.7** :
```
Requests queue until ship has landed
Requests queue at the company
Added Low Pitch Voices
Voice Pitch effects should work more consistently
```


**1.0.6** :
```
Prepping for integration into the CC desktop app. Added our version checker ccver
```
