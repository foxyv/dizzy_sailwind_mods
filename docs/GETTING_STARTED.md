# Getting started — Sailwind mod development

## What you're building

Sailwind is a **Unity Mono** game. Mods are **BepInEx 5 plugins** (C# class libraries) that:

1. Load at game start from `BepInEx/plugins`
2. Optionally **HarmonyX**-patch game methods
3. Optionally depend on shared helpers (SailwindModdingHelper, Sailwind.API, etc.)

You do **not** edit `Assembly-CSharp.dll` in place for shipped mods. Decompile it to learn; patch at runtime.

## Prerequisites

- [ ] Steam copy of Sailwind (appid `1764530`)
- [ ] [.NET SDK](https://dotnet.microsoft.com/download) 6+ (8/9 fine for building; plugins still target **net472** / netstandard2.0)
- [ ] IDE: Visual Studio, Rider, or VS Code + C#
- [ ] Decompiler for learning: [ILSpy](https://github.com/icsharpcode/ILSpy) or dnSpy (read-only preferred)

## 1. Install the modding runtime

**Recommended:** Thunderstore Mod Manager or [r2modman](https://thunderstore.io/c/sailwind/p/ebkr/r2modman/) → community **Sailwind** → install pinned **BepInExPack**.

Manual (same pack):

1. Download https://thunderstore.io/c/sailwind/p/BepInEx/BepInExPack/
2. Extract archive **outside** the game folder
3. Copy contents of `BepInExPack/` into the folder that contains `Sailwind.exe`
4. Launch once → expect BepInEx console (pack enables it) and folders under `BepInEx/`

Useful paths (default Steam):

```
...\steamapps\common\Sailwind\Sailwind.exe
...\steamapps\common\Sailwind\Sailwind_Data\Managed\Assembly-CSharp.dll
...\steamapps\common\Sailwind\BepInEx\plugins\          # drop your DLL here (or profile equivalent)
...\steamapps\common\Sailwind\BepInEx\config\BepInEx.cfg
...\steamapps\common\Sailwind\BepInEx\LogOutput.log
```

If you use a mod manager profile, BepInEx may live under  
`%APPDATA%\Thunderstore Mod Manager\DataFolder\Sailwind\profiles\<Profile>\`  
instead of the game root — match your actual layout when setting HintPaths.

## 2. Scaffold a BepInEx 5 plugin

Official templates:

```powershell
dotnet new install BepInEx.Templates::2.0.0-be.4 --nuget-source https://nuget.bepinex.dev/v3/index.json
dotnet new bepinex5plugin -n Dizzy.MyFirstMod -T net472 -U 2019.1.10
```

- **TFM:** `net472` (or `netstandard2.0`) — matches Sailwind's Mono BCL
- **Unity:** `2019.1.10` — confirmed in community recon; confirm in BepInEx console on first run

Minimal plugin shape:

```csharp
using BepInEx;

namespace Dizzy.MyFirstMod
{
    [BepInPlugin("com.dizzy.sailwind.myfirstmod", "My First Mod", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("My First Mod loaded");
        }
    }
}
```

`BaseUnityPlugin` is a `MonoBehaviour` — `Awake` / `Update` / coroutines work as usual.

## 3. Reference game assemblies

Copy needed DLLs into a **gitignored** `lib/` (do not HintPath straight into Program Files if you can avoid it):

Typical set for a first Harmony mod:

| Assembly | From |
|----------|------|
| `BepInEx.dll`, `0Harmony.dll` | `BepInEx/core/` |
| `Assembly-CSharp.dll` | `Sailwind_Data/Managed/` |
| `UnityEngine.dll`, `UnityEngine.CoreModule.dll` | Managed (add modules as needed) |

Do **not** reference game `mscorlib.dll`, `netstandard.dll`, or random `System.*.dll` from Managed.

Example `.csproj` Reference pattern (paths local to your machine):

```xml
<ItemGroup>
  <Reference Include="BepInEx">
    <HintPath>lib\BepInEx.dll</HintPath>
  </Reference>
  <Reference Include="0Harmony">
    <HintPath>lib\0Harmony.dll</HintPath>
  </Reference>
  <Reference Include="Assembly-CSharp">
    <HintPath>lib\Assembly-CSharp.dll</HintPath>
  </Reference>
  <Reference Include="UnityEngine.CoreModule">
    <HintPath>lib\UnityEngine.CoreModule.dll</HintPath>
  </Reference>
</ItemGroup>
```

Build → copy `bin/Debug/net472/YourMod.dll` into `BepInEx/plugins` → launch → check console / log.

## 4. Two development lanes (pick per mod)

### Lane A — Classic: Harmony on Assembly-CSharp

What most Thunderstore mods do today.

- Reference game DLL + HarmonyX
- Soft-depend on **SailwindModdingHelper** when you need shared utilities / when other mods expect it
- Inspect types in ILSpy; patch carefully; expect breakage on game updates

Good first reads in the wild:

- https://github.com/alesparise/CookedInfo-Sailwind-Mod
- https://github.com/bryon82/SailwindHooksHangMore (documents extension points for other authors)

### Lane B — Sailwind.API contract

From https://github.com/aram-devdocs/sailwind_online:

```powershell
git clone https://github.com/aram-devdocs/sailwind_online
cd sailwind_online
# see README: make setup / template install (Windows may need make or follow docs/03-guides/setup-windows.md)
dotnet new install ./templates/sailwind-mod
dotnet new sailwind-mod --name Dizzy.CoolMod
```

Build against `Sailwind.Api.Abstractions` shipped by the API host plugin. Wait for `Ready`, then use interfaces (clock, wind, boat, save events) instead of raw reflection. Surface hash warns on game drift instead of silent Harmony failures.

Still early-access; evaluate maturity before depending on it for a public release.

## 5. Useful runtime realities

| Fact | Implication |
|------|-------------|
| `Supports SRE: False` | HarmonyX still works; avoid DynamicMethod-based tricks |
| Floating origin / ~1 km shifts | World positions are not a naïve infinite float grid — see modding-notes |
| Items: visual + twin | Physics/buoyancy live on the twin; breaking that contract crashes (documented extensively) |
| `GameState.modData` | Preferred persistence hook for many mods (note 11 in modding-notes) |
| TextMesh (not TMP) for much UI | UI text mods need the right component |

## 6. Suggested first milestones

1. Hello-world plugin that only logs
2. Read-only Harmony postfix that logs something safe (e.g. time-of-day / boat name once per minute)
3. BepInEx `ConfigEntry<>` for a toggle
4. Decide Lane A vs B for the real feature
5. Join Discord + skim Thunderstore top mods for patterns / conflicts

## 7. Publishing later

- Thunderstore package with `manifest.json`, icon, README, dependency string on `BepInEx-BepInExPack`
- Prefer unique permanent **GUID**; never change it after release
- Document game version / build tested against
