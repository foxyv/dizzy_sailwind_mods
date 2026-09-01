# DizzySailwindMods

Workspace for Sailwind (Unity / BepInEx) mod development.

## Dizzy Gamma

Plugin: [`src/Dizzy.Gamma`](src/Dizzy.Gamma)

Brightens dark scenes by scaling **scene ambient lighting** (not a fullscreen color filter), so overlay UI stays unaffected. `1.0` is vanilla.

```powershell
dotnet build src\Dizzy.Gamma\Dizzy.Gamma.csproj -c Release
```

| Action | Default |
|--------|---------|
| Toggle settings panel | `F7` |
| Gamma up | `Right Ctrl` + `=` |
| Gamma down | `Right Ctrl` + `-` |

Config: `BepInEx\config\com.dizzy.sailwind.gamma.cfg`

**Download:** [GitHub Releases](https://github.com/foxyv/dizzy_sailwind_mods/releases) — or build and package locally via [docs/RELEASE.md](docs/RELEASE.md).

## Dizzy Calendar

WARNING: WORK IN PROGRESS, REALLY FREAKING BROKEN RIGHT NOW

Plugin: [`src/Dizzy.Calendar`](src/Dizzy.Calendar)

Wall-mountable calendar ShipItem that shows the live in-game day (`Day N`) on its face and in look text.

```powershell
dotnet build src\Dizzy.Calendar\Dizzy.Calendar.csproj -c Release
```

Deploys to `BepInEx\plugins\Dizzy.Calendar\`.

| Detail | Value |
|--------|--------|
| Prefab index | **930** (reserved — avoid in other mods) |
| Shop | Dragon Cliffs market stall area (near Climate instrument spawners) |
| Debug spawn | `F8` — sold calendar in front of the player |

### Hang it

1. Buy at Dragon Cliffs (or press `F8` to debug-spawn)
2. Hold near an interior wall until the attach preview appears
3. Place, then **nail with the hammer**

Config: `BepInEx\config\com.dizzy.sailwind.calendar.cfg`

## Docs

| Doc | Purpose |
|-----|---------|
| [docs/RELEASE.md](docs/RELEASE.md) | Manual GitHub Releases (Gamma zip + publish checklist) |
| [docs/SOURCES_OF_TRUTH.md](docs/SOURCES_OF_TRUTH.md) | Ranked references — what to trust when facts conflict |
| [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md) | Environment, toolchain, first plugin path |
| [docs/ECOSYSTEM.md](docs/ECOSYSTEM.md) | Communities, mod hosts, example open-source mods |

Research snapshot: **2026-08-25**. Re-verify game build / BepInEx pack versions before shipping.
