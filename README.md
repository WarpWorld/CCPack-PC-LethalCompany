# Lethal Company

## Pack metadata
- **Game display name:** Lethal Company
- **Crowd Control game ID:** `LethalCompany`
- **Connector type:** `SimpleTCPServerConnector`
- **Endpoint port:** `51338`
- **Mod framework:** BepInEx `5.4.2100`


This pack uses a BepInEx Crowd Control plugin. The repository includes an
installable BepInEx layout in `mod` and a Thunderstore package description in
`thunderstore`.

## Requirements

- Lethal Company.
- Crowd Control with the **Lethal Company** pack selected.
- BepInEx 5.4.2100 and TerminalApi 1.5.6, as declared in the Thunderstore
  manifest. The bundled `mod` layout includes BepInEx files,
  `CrowdControl.dll` and `ConnectorLib.JSON.dll` (both are required).

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
`127.0.0.1:51338` using the Crowd Control JSON protocol (ConnectorLib.JSON),
so the pack definition must not set `MessageFormat` to `CrowdControlLegacy`.
By default it connects without first checking that the Crowd Control app is
running (`Advanced.AlwaysAttemptConnection` in
`BepInEx/config/WarpWorld.CrowdControl.cfg`), which keeps it working when the
app runs as administrator.

In game, **F8** hides or shows the Crowd Control overlay (connection indicator
and active timed effects) and **F9** forces a reconnect. The game host has authority for many effect handlers. In a
multiplayer game, every player should install the same mod, as required by the
included Thunderstore documentation.

## Custom effects

The mod supports community-written effects. Drop an effect (a `.cs` file or a
pack folder) into `%APPDATA%\CrowdControl-Apps\CustomEffects\LethalCompany`
and the mod compiles and loads it; see [CustomEffect.md](CustomEffect.md) for
how to write one. The C# compiler (the `Microsoft.CodeAnalysis*.dll` and
`System.*.dll` files next to `CrowdControl.dll`) is only loaded when an effect
has to be built from source.

Streamers can turn this off with `CustomEffects.AllowCustomEffects` in
`BepInEx/config/WarpWorld.CrowdControl.cfg`. Viewers can only buy custom
effects once the Lethal Company pack has `allowCustomEffects` set; until then
they load and run locally but are not offered in the menu.

## Troubleshooting

- **The game does not load the plugin:** confirm that `winhttp.dll` is beside
  the executable and `BepInEx\plugins\CrowdControl.dll` and
  `BepInEx\plugins\ConnectorLib.JSON.dll` exist after copying.
- **No effects or connection:** start the Crowd Control desktop app, verify
  the Lethal Company pack is selected, then relaunch the game.
- **Multiplayer effects fail:** ensure every player has the mod installed and
  that the session host is present. Compatibility with unrelated mods is
  limited; try a clean mod-manager profile.

## Repository layout

- `LethalCompany.cs` defines the pack.
- `src/` is the BepInEx plugin, built on the
  [BepInEx example plugin](https://github.com/WarpWorld/BepinEx-Example-Plugin)
  (network client, scheduler, timed effects, overlay, custom-effect support).
  Lethal Company's game logic is in `GameActions.cs`, `Class1.cs` (patches and
  the multiplayer chat relay) and `TimedEffects.cs`; the `[Effect]` classes in
  `src/Delegates/Effects/Implementations` map each effect code to it.
- `ConnectorLib.JSON/` is the Crowd Control protocol library the plugin uses.
- The mod version lives only in `<Version>` in `src/LethalCompany.csproj`.
  A Release build stamps it everywhere else and stages the plugin into `mod/`
  and `thunderstore/`.
