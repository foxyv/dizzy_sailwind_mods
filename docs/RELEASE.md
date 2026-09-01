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
$version = "0.1.2"
$staging = "dist\Dizzy.Gamma-$version"
$pluginDir = "$staging\BepInEx\plugins\Dizzy.Gamma"

New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item "src\Dizzy.Gamma\bin\Release\Dizzy.Gamma.dll" $pluginDir
Compress-Archive -Path $staging\* -DestinationPath "dist\Dizzy.Gamma-$version.zip" -Force
```

Install layout inside the zip:

```
BepInEx/plugins/Dizzy.Gamma/
  Dizzy.Gamma.dll
```

Users extract into their Sailwind folder (next to `Sailwind.exe`).

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

1. Update `PluginVersion` in [`src/Dizzy.Gamma/Plugin.cs`](../src/Dizzy.Gamma/Plugin.cs) and `<Version>` in [`src/Dizzy.Gamma/Dizzy.Gamma.csproj`](../src/Dizzy.Gamma/Dizzy.Gamma.csproj)
2. Rebuild, re-run `package-gamma-release.ps1` with the new version in the script (or pass `-Version`)
3. New tag `vX.Y.Z` and new zip on GitHub Releases

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
