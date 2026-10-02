# Lethal Company

This pack uses a BepInEx Crowd Control plugin. The repository includes an
installable BepInEx layout in `mod` and a Thunderstore package description in
`thunderstore`.

## Requirements

- Lethal Company.
- Crowd Control with the **Lethal Company** pack selected.
- BepInEx 5.4.2100 and TerminalApi 1.5.6, as declared in the Thunderstore
  manifest. The bundled `mod` layout includes BepInEx files and
  `CrowdControl.dll`.

## Installation and setup

1. Close Lethal Company.
2. Overlay the contents of `mod` onto the game directory, preserving the
   `BepInEx` directory and placing `winhttp.dll` beside the game executable.
3. Start Crowd Control and select Lethal Company.
4. Launch the game, host or join a lobby, and wait until the game is playable.

For the published setup workflow, see
<https://crowdcontrol.live/guides/lethalcompany/>.

## Connection behavior

The plugin connects to the local Crowd Control server at
`127.0.0.1:51338`. The game host has authority for many effect handlers. In a
multiplayer game, every player should install the same mod, as required by the
included Thunderstore documentation.

## Troubleshooting

- **The game does not load the plugin:** confirm that `winhttp.dll` is beside
  the executable and `BepInEx\plugins\CrowdControl.dll` exists after copying.
- **No effects or connection:** start the Crowd Control desktop app, verify
  the Lethal Company pack is selected, then relaunch the game.
- **Multiplayer effects fail:** ensure every player has the mod installed and
  that the session host is present. Compatibility with unrelated mods is
  limited; try a clean mod-manager profile.

## Repository layout

- `LethalCompany.cs` defines the pack.
- `mod/`, `src/`, and `thunderstore/` contain the game-side, supporting, and package source.
