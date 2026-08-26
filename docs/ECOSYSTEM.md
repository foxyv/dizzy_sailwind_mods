# Ecosystem map

## Distribution

| Channel | Link | Notes |
|---------|------|-------|
| Thunderstore | https://thunderstore.io/c/sailwind/ | Searchable public mods; managers: r2modman, Thunderstore Mod Manager, Gale |
| Official Discord | https://discord.gg/u4C8RQG6X9 | Still the densest hub; many mods announced / hosted in channels first |
| Thunderstore Sailwind Discord | Linked from Thunderstore community page | Mod-manager / packaging oriented |
| GitHub | Various authors | Best for reading source; not all Thunderstore mods are mirrored |

Steam discussion consensus: Discord + Thunderstore cover almost everything; Nexus is not the primary store for this game.

## Stack (canonical)

```
Sailwind.exe (Unity 2019.1 Mono)
  └─ doorstop → BepInEx 5.4.23.x
       ├─ HarmonyX (0Harmony)
       ├─ plugins/*.dll
       └─ optional: SailwindModdingHelper, ConfigurationManager, Sailwind.API, …
```

## Notable authors / packages (as of research)

Actively updated on Thunderstore (examples):

- **RadDude** — RadRefinements, BetterFishing, HooksHangMore, BitsAndBobsRadRedux, Climate, RandomEncounters, ModVersionChecker, …
- **DogEggz** — recent QoL / systems experiments (sails, camera, postal, etc.)
- **pander33** — SailwindCoop (LAN/VPN co-op)
- **App24** — SailwindModdingHelper (library many mods depend on)

## Libraries worth knowing

| Package | Purpose |
|---------|---------|
| BepInExPack | Required loader |
| SailwindModdingHelper | Shared modding helpers / incumbent API-ish layer |
| BepInEx.ConfigurationManager | In-game config UI (common soft dependency pattern) |
| Sailwind.API | Newer version-checked interface layer (sailwind_online; not yet the default Thunderstore dependency) |

## Open-source study list

Read these before inventing patterns:

1. https://github.com/alesparise/CookedInfo-Sailwind-Mod — small, clear plugin project
2. https://github.com/bryon82/SailwindHooksHangMore — documents how other mods can integrate
3. https://github.com/bryon82/SailwindRadRefinements — typical QoL Harmony mod shape
4. https://github.com/AppSailwindMods/SailwindModdingHelper — what the ecosystem already shares
5. https://github.com/aram-devdocs/sailwind_online — Sailwind.API + sample + template
6. https://github.com/ai-pop/sailwind-modding-notes — deep systems encyclopedia (EN under `en/`)

## Community lessons (don't learn the hard way)

- **Harmony without version checks** breaks when game methods move (AnchorImprovements / `Anchor.Start` case).
- **Item twin physics** is a footgun; read modding-notes before adding solid colliders to held items.
- Prefer **Thunderstore dependency strings** so managers pull BepInEx + helpers automatically.
- Keep game DLLs out of git; copy into `lib/` and ignore.
