# Manual GitHub Releases

How to publish downloadable mod zips on [GitHub Releases](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository). No CI — build locally, upload the zip in the GitHub UI.

Repo: `https://github.com/foxyv/dizzy_sailwind_mods`

## Dizzy Gamma (ready to ship)

Single DLL — no Unity or AssetBundle step.

### 1. Build

From the repo root:

```powershell
dotnet build src\Dizzy.Gamma\Dizzy.Gamma.csproj -c Release
```

Output: `src\Dizzy.Gamma\bin\Release\Dizzy.Gamma.dll`

### 2. Package

```powershell
.\scripts\package-gamma-release.ps1
```

Or by hand:

```powershell
$version = "0.1.6"
$pluginDir = "dist\Dizzy.Gamma-$version\Dizzy.Gamma"

New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item "src\Dizzy.Gamma\bin\Release\Dizzy.Gamma.dll" $pluginDir
Compress-Archive -Path $pluginDir -DestinationPath "dist\Dizzy.Gamma-$version.zip" -Force
```

Install layout inside the zip:

```
Dizzy.Gamma/
  Dizzy.Gamma.dll
```

Copy the `Dizzy.Gamma` folder into `BepInEx/plugins/` in your Sailwind install.

### 3. Publish on GitHub

**Prepared artifacts** (after running the package script):

- `dist/Dizzy.Gamma-<version>.zip` — attach to the release
- `dist/GITHUB_RELEASE_NOTES-v<version>.md` — paste into the release description

**Quick link** (0.1.2): [Draft new release](https://github.com/foxyv/dizzy_sailwind_mods/releases/new?tag=v0.1.2)

1. **Releases** → **Draft a new release**
2. Tag: `v0.1.2` (match `PluginVersion` in `src/Dizzy.Gamma/Plugin.cs`)
3. Title: `Dizzy Gamma 0.1.2`
4. Paste release notes from `dist/GITHUB_RELEASE_NOTES-v0.1.2.md`
5. Attach `dist\Dizzy.Gamma-0.1.2.zip`
6. **Publish release**

Optional (after `gh auth login`):

```powershell
gh release create v0.1.2 dist\Dizzy.Gamma-0.1.2.zip --title "Dizzy Gamma 0.1.2" --notes-file dist\GITHUB_RELEASE_NOTES-v0.1.2.md
```

### 4. Bump version next time

| Step | Action |
|------|--------|
| Version bump | Keep `PluginVersion` in [`src/Dizzy.Gamma/Plugin.cs`](../src/Dizzy.Gamma/Plugin.cs) and `<Version>` in [`src/Dizzy.Gamma/Dizzy.Gamma.csproj`](../src/Dizzy.Gamma/Dizzy.Gamma.csproj) in sync |
| Package | Rebuild, re-run `package-gamma-release.ps1` with `-Version X.Y.Z` |
| GitHub tag | Always `vX.Y.Z` matching the plugin version (required for ModVersionChecker) |
| ModVersionChecker | No action after the initial listing; update prompts break only if the tag ≠ plugin version |

### 5. ModVersionChecker

[Sailwind ModVersionChecker](https://github.com/bryon82/SailwindModVersionChecker) (optional for players) compares installed BepInEx plugin versions against GitHub release tags.

- **Listing:** Dizzy Gamma is registered in upstream [`ModList.json`](https://github.com/bryon82/SailwindModVersionChecker/blob/main/ModList.json) as GUID `com.dizzy.sailwind.gamma` → `https://github.com/foxyv/dizzy_sailwind_mods`. Adding or changing listings is done via PR to that repo.
- **Tag rule:** Release tags must be `vX.Y.Z` and match `PluginVersion` (e.g. tag `v0.2.0` ↔ `PluginVersion = "0.2.0"`).
- **After each release:** No extra steps once listed. An hourly upstream Action refreshes `release_versions.json` from GitHub releases (plus CDN cache lag).

Do **not** list Dizzy Calendar from this monorepo until it ships with independent versioning — ModVersionChecker uses the repo’s latest tag for every GUID that points here.

---

## Dizzy Calendar (not ready)

Deferred until the AssetBundle is baked. When ready, ship **three files** in one folder:

```
BepInEx/plugins/Dizzy.Calendar/
  Dizzy.Calendar.dll
  Dizzy.Calendar.Bridge.dll
  dizzycalendar
```

See [CALENDAR_ASSETBUNDLE.md](CALENDAR_ASSETBUNDLE.md) for Unity bake steps.

---

## Do not commit

- `dist/` — release zips and staging (gitignored)
- Game DLLs from `Sailwind_Data/Managed/`
- Built plugin DLLs on `main` (attach only to Releases, not the repo tree)
