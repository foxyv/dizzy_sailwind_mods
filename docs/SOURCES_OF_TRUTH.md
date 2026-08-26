# Sources of truth

When docs, Discord advice, and decompiled notes disagree, use this ranking.

## Tier 0 — Ground truth (always wins)

| Source | Why |
|--------|-----|
| Your installed game | `Sailwind_Data/Managed/Assembly-CSharp.dll` (and related Managed DLLs) is the real API surface for *your* build |
| `steamapps/appmanifest_1764530.acf` | Steam `buildid` / install provenance |
| BepInEx console + `BepInEx/LogOutput.log` | What actually loaded, Unity version string, `Supports SRE`, plugin errors |

Never commit or redistribute game assemblies. Reference them locally (e.g. a gitignored `lib/` folder).

## Tier 1 — Official / maintained tooling docs

| Source | URL | Use for |
|--------|-----|---------|
| BepInEx docs — install | https://docs.bepinex.dev/articles/user_guide/installation/index.html | Installing BepInEx |
| BepInEx docs — plugin tutorial | https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html | Project setup, templates, metadata, Harmony |
| BepInEx GitHub releases | https://github.com/BepInEx/BepInEx/releases | Upstream version tags |
| Thunderstore Sailwind BepInExPack | https://thunderstore.io/c/sailwind/p/BepInEx/BepInExPack/ | **Community-standard** pack for this game (Mono). Currently documented as **5.4.23.5** (`BepInExPack` **5.4.2305**) |

**Rule:** Sailwind uses **BepInEx 5 (Mono)**, not BepInEx 6 Il2Cpp. Prefer the Thunderstore pack over improvising a raw zip unless you know why.

## Tier 2 — Confirmed Sailwind-specific facts (version-sensitive)

These are excellent, but they pin a **specific** game build. Re-check after every Steam update.

| Source | URL | Notes |
|--------|-----|-------|
| `sailwind_online` game facts | https://github.com/aram-devdocs/sailwind_online/blob/dev/docs/04-reference/game-facts.md | Unity **2019.1.10f1**, Mono, net472/netstandard2.0, Crest ocean, BepInEx **5.4.23.5**, Managed path, key type names. Recon dated **2026-07-21**. |
| `sailwind_online` prior art | https://github.com/aram-devdocs/sailwind_online/blob/dev/docs/04-reference/prior-art.md | Ecosystem map: SailwindModdingHelper coexistence, AnchorImprovements Harmony breakage lesson, popular-mod seed list |
| Sailwind.API (same repo) | https://github.com/aram-devdocs/sailwind_online | Stable interfaces + surface hash over game internals; sample mod + `dotnet new sailwind-mod` template. Early-access / build-from-source as of research date. |

### Snapshot from game-facts (verify locally)

- Steam appid: **1764530**
- Engine: Unity **2019.1.10f1**, Mono, scripting ~**.NET 4.7.x** (ships `netstandard.dll` 2.0)
- Target frameworks that load: **net472** or **netstandard2.0** (not netstandard2.1)
- Ocean: **Crest**
- Managed: `...\Steam\steamapps\common\Sailwind\Sailwind_Data\Managed\`
- Runtime: `Supports SRE: False` → no `System.Reflection.Emit` / `DynamicMethod`; use Mono.Cecil offline, plain reflection / HarmonyX at runtime
- Saves: `SaveContainer` via **BinaryFormatter** (not JSON)

Seed types confirmed in Assembly-CSharp (member names still require DLL inspection):  
`Boat`, `BoatRefs`, `SaveLoadManager`, `SaveSlots`, `SaveContainer`, `GameState`, `Sun`, `Wind`, `IslandMarket`, `Mooring`, `Anchor`, `PlayerGold`, `Currency`.

## Tier 3 — Deep internals encyclopedias (decompiled notes)

| Source | URL | Notes |
|--------|-----|-------|
| ai-pop / sailwind-modding-notes | https://github.com/ai-pop/sailwind-modding-notes | Large English+Russian note set from ILSpy + runtime hooks. Pinned to **Sailwind v0.38** / Unity 2019.1.10f1 / BepInEx 5.4.23.5. Treat as a map, not gospel after patches. |

High-value entry points from that repo:

- Mod persistence: `GameState.modData["MyMod.*"]` (note 11)
- World scale: **1 Unity unit = 1 meter** (note 28)
- Daily hook: `Sun.OnNewDay` (note 18)
- Item model: ShipItem visual GO + physics **twin** (`ItemRigidbody`) (notes 16, 44, 67)
- SRE false but HarmonyX works (note 07)
- Worked examples: floating loot (34), fast travel (42)

## Tier 4 — Community APIs and example mods

| Source | URL | Role |
|--------|-----|------|
| SailwindModdingHelper | Thunderstore: App24; GitHub: https://github.com/AppSailwindMods/SailwindModdingHelper | Incumbent helper many published mods depend on (v2.1.1 widely cited). Coexist; don't casually break it. |
| RadDude / bryon82 mods | e.g. https://github.com/bryon82/SailwindRadRefinements, HooksHangMore, BitsAndBobsRadRedux | Living patterns for BepInEx plugins, config, content |
| CookedInfo | https://github.com/alesparise/CookedInfo-Sailwind-Mod | Small readable plugin + classic Reference HintPaths to Managed / BepInEx core |
| App24/SailwindMods | https://github.com/App24/SailwindMods | Early historical mods (2022); useful archaeology, not current defaults |

## Tier 5 — Distribution & community (discoverability, not API truth)

| Source | URL | Role |
|--------|-----|------|
| Thunderstore Sailwind | https://thunderstore.io/c/sailwind/ | Public mod database (~49 packages); install via r2modman / Thunderstore / Gale |
| Thunderstore “Modding Discord” button | invite on community page (`discord.com/invite/ySH63huD`) | Thunderstore-side Sailwind modding chat |
| Official Sailwind Discord | https://discord.gg/u4C8RQG6X9 (also linked from Fandom wiki) | Primary community hub; many mods still live in channels/threads first |
| Sailwind Fandom wiki | https://sailwind.fandom.com/ | Game lore/UI; modding pages incomplete |
| Steam discussions | app `1764530` | Pointers to Discord / Thunderstore; not technical SoT |

## Conflict resolution cheatsheet

1. **Does my build match the doc's buildid / game version?** If no → re-decompile / re-probe.
2. **Harmony patch missing a method?** Game update moved a member (see AnchorImprovements in prior-art). Prefer soft failure / version checks / Sailwind.API surface hash over silent hard patches.
3. **Thunderstore pack vs GitHub BepInEx zip?** Prefer Thunderstore **BepInExPack** for Sailwind unless debugging the injector itself.
4. **Sailwind.API vs SailwindModdingHelper?** Different layers. Helper is the installed ecosystem dependency; API is a newer contract layer aiming for coexistence. Many existing mods only need Helper + Harmony on Assembly-CSharp.
