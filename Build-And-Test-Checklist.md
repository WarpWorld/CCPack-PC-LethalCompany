# Build & Test Checklist

Games touched in the game-state rollout that still need a local build and/or in-game verification.

**Pack change (all listed games):** `GameStateCheckInterval = 0.5f` on the Crowd Control pack — rebuild/redeploy packs if you ship pack DLLs separately.

**What to verify in-game:** CC shows correct state (menu / loading / ready), effects retry when not ready, and state updates within ~0.5s of entering/leaving gameplay.

---

## Mod rebuild required (BepInEx — legacy TCP)

These mods had **new `0xFD` GameUpdate** handling added. Deploy the rebuilt BepInEx plugin to the game’s `BepInEx/plugins/` folder.

| Game | Pack repo | Mod project path | Notes |
|------|-----------|------------------|-------|
| Lethal Company | `CCPack-PC-LethalCompany` | `src/LethalCompany.sln` | Was `UpdateReadyState` only; now responds to state polls |
| R.E.P.O | `CCPack-PC-REPO` | `src/REPO.sln` | `GetGameState()` from existing `IsReady()` |
| Content Warning | `CCPack-PC-ContentWarning` | `src/*.sln` or `.csproj` | Same legacy GameUpdate pattern |
| Ale & Tale Tavern | `CCPack-PC-AleAndTaleTavern` | `src/*.sln` or `.csproj` | |
| Anger Foot | `CCPack-PC-AngerFoot` | `src/*.sln` or `.csproj` | |
| TCG Card Shop Simulator | `CCPack-PC-TCGCardShopSimulator` | `src/*.sln` or `.csproj` | |
| Supermarket Together | `CCPack-PC-SupermarketTogether` | `src/*.sln` or `.csproj` | |

---

## Mod rebuild required (BepInEx — minor fix)

| Game | Pack repo | Mod project path | Notes |
|------|-----------|------------------|-------|
| Peglin | `CCPack-PC-Peglin` | `src/*.sln` or `.csproj` | Already had state; fixed `GameStateResponse` to set request `id` |
| ATLYSS | `CCPack-PC-Atlyss` | `src/*.sln` or `.csproj` | Same `id` fix as Peglin |

---

## Mod rebuild required (MelonLoader / WebSocket)

New **`JSONGameUpdate`** + `getState()` + periodic `CrowdControl.state` updates.

| Game | Pack repo | Mod project path | Notes |
|------|-----------|------------------|-------|
| Tricky Towers | `CCPack-PC-TrickyTowers` | `Source/ML_CrowdControl_TrickyTowers.sln` | |
| The Forest | `CCPack-PC-TheForest` | `Source/` MelonLoader solution | Same mod for flat + VR packs |
| The Forest VR | `CCPack-PC-TheForest` | *(same mod as The Forest)* | Pack-only variant (`TheForestVR.cs`) |
| Death's Door | `CCPack-PC-DeathsDoor` | `Source/` MelonLoader solution | |
| A Difficult Game About Climbing | `CCPack-PC-ADifficultGameAboutClimbing` | `Source/` MelonLoader solution | Mod under `Source/CCEffects/` |
| Core Keeper | `CCPack-PC-CoreKeeper` | `GameMod/` MelonLoader solution | TCP pack + WebSocket mod |

---

## Mod rebuild required (BepInEx IL2CPP + WebSocket)

| Game | Pack repo | Mod project path | Notes |
|------|-----------|------------------|-------|
| Sonic Superstars | `CCPack-PC-SonicSuperstars` | `BepInEx.CC` + `CrowdControl` projects | New `GameUpdate` case in `ProcessMessage`, `EffectPack/Base.getState()` |

---

## Pack interval only — mod already had state (still test end-to-end)

Mod source unchanged (or trivial); **pack** now polls every 0.5s. Confirm CC client receives state without rebuilding the mod unless you want a fresh deploy anyway.

| Game | Pack repo | Mod stack |
|------|-----------|-----------|
| Cuphead | `CCPack-PC-Cuphead` | MelonLoader — already had `getState()` + `JSONGameUpdate` |
| Rogue Legacy 2 | `CCPack-PC-RogueLegacy2` | MelonLoader — same as Cuphead |

---

## Earlier session — still pending build/test

These were touched before the BepInEx/MelonLoader batch and may still need work on your machine.

| Game | Pack repo | What to build | Status |
|------|-----------|---------------|--------|
| Stardew Valley | `CCPack-PC-StardewValley` | `mod/CrowdControl.sln` → deploy to `Mods/CrowdControl/` | Built v1.6.0 in session; re-test if not deployed |
| Terraria | `CCPack-PC-Terraria` | `mod/` tModLoader project → `.tmod` or Mod Sources workflow | Mod cloned + state added; full tML deploy may be incomplete |
| Retro Rewind | `CCPack-PC-RetroRewind` | `native/CrowdControlTcpBridge/` → `main.dll` | Lua + C++ GameUpdate added; **native DLL rebuild required** |
| RV There Yet | `CCPack-PC-RVThereYet` | Same TCP bridge pattern as Retro Rewind | Lua updated; confirm native build if applicable |

---

## Suggested test order

1. **Lethal Company** — you asked about this one explicitly; good smoke test for legacy BepInEx.
2. **REPO** or **Content Warning** — same code path, different ready logic.
3. **Cuphead** or **Rogue Legacy 2** — quickest WebSocket validation (mod unchanged).
4. **Tricky Towers** or **The Forest** — full new WebSocket state plumbing.
5. **Stardew / Terraria / Retro Rewind** — if still on your backlog from the wider audit.

---

## Quick counts

| Category | Count |
|----------|------:|
| BepInEx legacy — mod rebuild | 7 |
| BepInEx — minor mod fix | 2 |
| MelonLoader — mod rebuild | 6 (5 mods; Forest VR shares The Forest) |
| Sonic Superstars — mod rebuild | 1 |
| Pack-only test (mod already OK) | 2 |
| Earlier session — pending | 4 |
| **Total pack `.cs` files with new interval** | **19** (18 games + The Forest VR) |

*Generated after the BepInEx/MelonLoader game-state batch (June 2026).*
