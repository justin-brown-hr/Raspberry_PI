# The Catapult — Raspberry Pi 5 Deployment

Getting a decompiled Unity 2021.3 Android game building and running on a Raspberry Pi 5,
playable with a gamepad, fully manageable over a remote connection.

**Legend:** `[ ]` todo · `[x]` done · `[~]` in progress · `[!]` blocked

## Status — 2026-09-20

| Phase | State |
|---|---|
| 0 — Remote access | **Done** — SSH + VNC over Tailscale, desktop at 1080p |
| 1 — Build environment | **Done** — Unity 2021.3.23f1 licensed, Android toolchain installed, no root |
| 2 — Project builds | **Done** — zero code errors; `TheCatapult-arm64.apk` (93 MB) verified ARM64 |
| 3 — Android on the Pi | **Done** — game runs on the Pi 5 under Waydroid |
| 4 — Gamepad | Partial — code written; **gameplay verified via touch injection** |

**Artifacts:**
- `Build/TheCatapult-cabinet.apk` (58 MB) — **release + portrait**, current cabinet build
- `Build/TheCatapult-arm64.apk` (93 MB) — dev build, landscape, kept for debugging

`com.pj.catapult` · minSdk 24 · targetSdk 33 · `arm64-v8a` · `screenOrientation=portrait`

---

## Target environment

| | |
|---|---|
| **Build host** | Ubuntu 24.04, x86_64, 12 cores, 47 GB RAM, no sudo |
| **Target** | Raspberry Pi 5 Model B Rev 1.1, 8 GB, Debian 13 (trixie) arm64 |
| **Pi hostname** | `garra` @ `100.122.184.82` (Tailscale) |
| **Display** | LG TV via HDMI-A-1 @ 1920x1080 60Hz |
| **Unity** | 2021.3.23f1, revision `213b516bf396` |
| **Package** | `com.pj.catapult` |

---

## Phase 0 — Remote access ✅

- [x] Verify SSH access to the Pi over Tailscale
- [x] Confirm desktop environment installed (labwc/Wayland — was already present)
- [x] Determine Chrome Remote Desktop viability → **not possible, no arm64 build from Google**
- [x] Restrict `wayvnc` to the Tailscale interface only (was bound to all interfaces)
- [x] Enable and start `wayvnc.service`
- [x] Add systemd drop-in so VNC waits for `tailscaled` at boot
- [x] Install TigerVNC viewer on the build host (no root, to `~/.local`)
- [x] Confirm end-to-end VNC connection (TLS negotiated, auth dialog reached)
- [x] Drop display 4K → 1080p and persist via kanshi
- [x] Add `.gitignore` covering Unity artifacts and credential files

---

## Phase 1 — Build environment

- [x] Download Unity 2021.3.23f1 editor tarball (2.5 GB)
- [x] Extract editor to `~/.local/unity` — reports `2021.3.23f1`
- [x] Download Android Build Support module (554 MB, `.pkg` format)
- [x] Install Android playback engine into `PlaybackEngines/AndroidPlayer` (1.9 GB) + SDK/NDK/JDK symlinks
- [x] Install JDK 11 — 11.0.32, dangling Debian conf symlinks repaired
- [x] Install Android SDK: cmdline-tools, platform-tools, android-33, build-tools 33.0.2
- [x] Install NDK r21d — confirmed `Pkg.Revision = 21.3.6528147`
- [x] Generate licence activation request — `Unity_v2021.3.23f1.alf` in repo root
- [x] Licence activated — offline/.alf route is Enterprise-only now; used Unity Hub GUI sign-in instead (Personal, 2026-09-20)
- [x] Licence verified on disk (`Unity_lic.ulf` + `UnityEntitlementLicense.xml`)

## Phase 2 — Make the project build

- [x] Open project headless, capture full compile log
- [x] Compile errors — **none in the game code**. All 11 initial errors were caused by compiling for the wrong target; with `-buildTarget Android` the project builds clean
- [x] Switch `AndroidTargetArchitectures` ARMv7 → ARM64 + IL2CPP
- [x] Target SDK set — resolves to 33, minSdk 24
- [x] **APK built** — `Build/TheCatapult-arm64.apk`, 93 MB
- [x] APK verified arm64 — `native-code: 'arm64-v8a'`, only `lib/arm64-v8a/` present, ELF header confirms AArch64

## Phase 3 — Android runtime on the Pi

- [x] Add Waydroid repo, install Waydroid 1.6.2 + LXC
- [x] `waydroid init` — LineageOS 20 (Android 13) arm64, system 1.98 GB + vendor 421 MB
- [x] Wire Waydroid to the labwc Wayland session
- [x] V3D GPU passthrough — needed a udev rule for the DRM render node
- [x] `waydroid app install` — `com.pj.catapult` installed
- [x] **Game launches and renders** — main menu confirmed by screenshot
- [x] Baseline at menu: 56°C, `throttled=0x0`, ~386 MB RSS

## Phase 4 — Gamepad

- [x] Write gamepad input for `CatapultLogic` (single player) — `GamepadAim.cs` + minimal patch
- [ ] Write gamepad input for `PvPCatapultLogic` — deferred until single-player is verified on hardware (needs 2 pads via joystick number)
- [x] Shop / shield controls — **no change needed**: their touch loops sit inside uGUI EventTrigger handlers (`MouseDown`/`MouseUp`), which EventSystem still fires from a pad. The loop only records `guiPressedFingerId`, which is read solely inside real-touch paths
- [ ] Verify menu navigation via EventSystem (expected to work already)
- [ ] Attach a physical gamepad to the Pi, **or** set up `/dev/uinput` virtual pad
- [x] **Gameplay verified on hardware** — full loop plays: menu → 1 Player → fire → round end
- [ ] Test the gamepad path specifically (needs a real pad; touch path confirmed working)

---

## Known issues in the project

Found during review, not yet addressed:

- [ ] IAP is entirely commented out (`Unity_PurchaiseIAP.cs` body inside `/* PJ */`)
- [ ] Ads never show — no `DEFINE_ADMOB`, SDK absent
- [ ] Google Play Games app ID is the placeholder `\u003`
- [ ] Third-party AdMob + GPGS IDs still present from the original publisher
- [ ] Only `CatapultLogic` has a mouse-input branch, and only under `Application.isEditor`
- [ ] Repo is 148 MB with large uncompressed WAVs — consider Git LFS before first commit
- [ ] Pi password is 3 characters — change before any client demo

---

## Verified facts

Checked directly, not assumed:

- Unity CDN reachable from build host (`unity.com` blocked, `download.unity3d.com` **200**)
- `license.unity3d.com` **200** — manual activation route open
- Pi kernel has `CONFIG_ANDROID_BINDER_IPC=y` + `CONFIG_ANDROID_BINDERFS=y` — **Waydroid viable**
- Waydroid repo and LineageOS arm64 images reachable from the Pi
- VNC server offers VeNCrypt (19), RealVNC (129), RA2 (5) — TigerVNC and RealVNC both work
- Shot power is a single scalar: `d = 5 + (aimDistance - 1) * 1.82`, clamped [5, 25].
  Direction is fixed per catapult — **gamepad needs one analog axis + one button**
- No gamepad currently attached to the Pi (USB keyboard only)
- `box64` available in Debian repos as fallback if Waydroid fails

---

## Notes

- Build host has **no sudo** — everything installs to `$HOME`
- Pi VNC is bound to the Tailscale IP only; not reachable from LAN or internet
- Display config backups: `~/.config/kanshi/config.bak.4k`, `config.init.bak.4k`
- wayvnc config backup: `/etc/wayvnc/config.bak`

---

## Environment fixes required (build host)

Unity 2021.3 on Ubuntu 24.04 without root needed these. All are recorded in
`~/.local/unity-sdk/env.sh`:

- **OpenSSL 1.1** — Unity's bundled .NET aborts with `No usable version of libssl
  was found` on OpenSSL 3. Extracted `libssl1.1` from the Focal archive to
  `~/.local/openssl11`, prepended to `LD_LIBRARY_PATH`. This was the real cause of
  the `bee_backend` hang: Roslyn crashed and Unity deadlocked on the dead child.
- **Java trust store** — `dpkg -x` skips `ca-certificates-java`, leaving `cacerts` a
  dangling symlink, so sdkmanager could not fetch anything. Built by hand from
  `/etc/ssl/certs/ca-certificates.crt` (121 certs) and pointed at via
  `JAVA_TOOL_OPTIONS`.
- **JDK conf symlinks** — Debian symlinks `conf/*` to `/etc/java-11-openjdk`, which
  does not exist without a real install. Replaced with the extracted files.

## Known-good compile command

    . ~/.local/unity-sdk/env.sh
    cd "The Catapult"
    ~/.local/unity/Editor/Data/bee_backend \
      --dagfile=Library/Bee/1300b0aE.dag --continue-on-failure ScriptAssemblies

### The `bee_backend` hang — solved

Unity batchmode deadlocked every time it spawned `bee_backend`. Two tests isolated it:

| stdin given to bee | Result |
|---|---|
| open pipe, no data | `Tundra build success` then **hangs forever** |
| `/dev/null` (instant EOF) | build **interrupted**, aborts |

`--stdin-canary` starts a thread that blocks on `read(stdin)` and never returns here,
so bee completes the build but never exits, and Unity waits on a child that cannot die.

Fixed with a shim at `~/.local/unity/Editor/Data/bee_backend` that strips the flag and
execs the real binary (moved to `bee_backend.real`). Unity batchmode then works normally.

**Note:** this shim lives in the Unity install, not the repo. A fresh Unity install on
another machine needs it reapplied.

## Unity version-specific tooling (from Unity's own release manifest)

`services.api.unity.com/unity/editor/release/v1/releases?version=2021.3.23f1` names
exactly what this editor expects. Installing *current* versions fails:

| Component | Required | Why current fails |
|---|---|---|
| JDK | **OpenJDK 8u172** | Unity rejects JDK 11 as "not valid JDK path" |
| SDK tools | `sdk-tools-linux-4333796` (legacy `tools/`) | cmdline-tools 9.0 needs Java 11, Unity runs it under JDK 8 |
| build-tools | **30.0.2** | Unity: "Latest supported build-tools version is 30.0.3" |
| platform-tools | 30.0.4 | — |
| NDK | r21d (21.3.6528147) | pinned |

`cmdline-tools/latest` was renamed to `cmdline-tools/9.0` so Unity falls back to the
legacy sdkmanager. Use `cmdline-tools/9.0/bin/sdkmanager` manually (needs JDK 11).


## Phase 3 blockers and fixes (Raspberry Pi 5 + Debian 13)

Waydroid needed four unrelated fixes. Each produced a misleading error.

| Symptom | Real cause | Fix |
|---|---|---|
| `waydroid-net.sh` fails, `ip_tables` module not found | Script hardcodes `iptables-legacy` (line 35); Debian 13 is nftables-only | Patched to use `iptables`; backup `.bak` |
| Container init `Segmentation fault(11)` | Pi 5 default kernel uses **16 KB pages**; Android needs **4 KB** | `kernel=kernel8.img` in `config.txt`; backup `config.txt.bak16k` |
| `createProcessGroup failed: Read-only file system` | LXC mounted cgroups read-only | `lxc.mount.auto = cgroup:rw:force sys:rw proc:rw` |
| `critical process 'lmkd' exited 4 times` → `InitFatalReboot` | `CONFIG_PSI_DEFAULT_DISABLED=y`; lmkd needs PSI on cgroup v2 | `psi=1` on kernel cmdline |
| App segfaults in minigbm, `renderD128 Permission denied` | Android UIDs are not in host `render` group (GID 992) | udev rule `KERNEL=="renderD*", MODE="0666"` |

Kernel cmdline additions (`/boot/firmware/cmdline.txt`, backup `.bak`):

    cgroup_enable=memory cgroup_memory=1 systemd.unified_cgroup_hierarchy=0 psi=1

Note: `systemd.unified_cgroup_hierarchy=0` is **ignored** — Debian 13 ships systemd 257,
which dropped cgroup v1 entirely. It is harmless; `psi=1` is what actually fixed lmkd.

### Start the game

    ssh pi5@100.122.184.82
    export XDG_RUNTIME_DIR=/run/user/1000 WAYLAND_DISPLAY=wayland-0
    waydroid session start &
    waydroid app launch com.pj.catapult


## Gameplay verification (2026-09-21)

Driven remotely with Android's own input injection - no gamepad needed, because
Waydroid delivers mouse/touch to Android, so the game's original touch path works:

    waydroid shell -- input tap <x> <y>
    waydroid shell -- input swipe <x1> <y1> <x2> <y2> <ms>

Confirmed working: main menu → 1 Player → game scene → **projectile fired and in flight**
→ round ends → game-over panel. Android display is 1920x1044.

### Performance on Pi 5 (1080p, 5+ shots)

| Metric | Result |
|---|---|
| Game CPU | **40–52%** of one core, no spikes during impacts |
| Memory | 418 MB, stable (no leak across rounds) |
| Host load avg | 1.76 on 4 cores (~44%) |
| ARM clock | 2.4 GHz — full speed, never downclocked |
| Temperature | 57–58°C |
| Throttling | `throttled=0x0` throughout |

**Verdict: the Pi 5 runs this game comfortably with substantial headroom.** The
destructible-sprite CPU work that was the main performance concern did not cause
measurable strain.

Note: `dumpsys gfxinfo` and `SurfaceFlinger --latency` return no per-frame data for
Unity under Waydroid (Unity bypasses the Android view system), so exact FPS was not
measurable; CPU headroom and absence of throttling are the proxies used.


---

## THE DEVICE: arcade cabinet (from `device/` photos)

This is **not** a desktop game. It is a coin-op arcade cabinet:

- **Portrait / vertical screen**
- **Arcade joystick + one large button**, wired via a USB encoder (Zero Delay / ARC).
  These present as a standard USB HID joystick, so `GamepadAim`'s
  `KeyCode.JoystickButton0` + `Vertical` axis maps onto them directly.
- **Card payment terminal** on the cabinet
- Paired with a claw machine — a commercial amusement installation
- Pi 5 + Waveshare PCIe-to-M.2 HAT + NVMe (`SSSTC CL4-4D256-Q79`, verified on device)

The LG TV used for all testing so far is a **bench setup**, not the cabinet display.

## Portrait build result (tested 2026-09-21)

Built release + portrait, installed, played. **It works** — but composition is poor.

| | |
|---|---|
| Renders in portrait | Yes — 1080x1920 |
| "Development Build" watermark | **Gone** (release build) |
| APK size | 58 MB, down from 93 MB |
| UI anchoring | Correct — buttons/score position properly |
| Gameplay | Playable — round starts, catapults active |
| CPU | 28% of one core |

**Problems to fix:**

- ~~Left edge clips the enemy tower~~ — **FIXED**, see camera fit below.
- Remaining: faint seam at the very top on night scenes (see below).

This is camera/UI tuning, **not** a rewrite — both combatants are visible and the
game is playable as-is.

## Kiosk setup — VERIFIED by reboot (2026-09-21)

- `~/.local/bin/cabinet-start.sh` — waits for the container service, starts the
  Waydroid session, polls until Android is up, launches the game.
- `~/.config/labwc/autostart` — runs it at session start.
- `~/.config/labwc/rc.xml` — window rules: fullscreen, no decoration, skip taskbar.
  Backup at `rc.xml.bak`; XML validated.

**Reboot test passed.** Clean boot-to-game, no intervention:

    00:54:00  cabinet start
    00:54:00  container service: active
    00:54:17  boot_completed=1 after 5 polls
    00:54:30  launched com.pj.catapult (attempt 1)

Two bugs found and fixed during the test:

1. **Race on first attempt.** The script waited for `Container: RUNNING`, which
   happens ~6s before Android can accept a launch. Now waits on
   `waydroid prop get sys.boot_completed` instead, and retries up to 5 times.
2. **Broken verification.** It checked with `waydroid shell`, which needs root, so
   it always reported failure even when the game had started. Now checks the host
   process table (`ps -eo args | grep com.pj.catapult`) — the container shares the
   host kernel, so Android processes are visible without root.

### Desktop furniture

- **Taskbar (`wf-panel-pi`): hidden** ✓
- **Desktop icons (`pcmanfm`): still respawn.** `lwrespawn` restarts them. Not worth
  more effort — on the portrait cabinet screen the game window fills the display and
  covers them. Revisit only if they show on the real hardware.
- labwc `windowRules` did **not** apply to the Waydroid surface despite trying
  `waydroid*`, `Waydroid*`, `*catapult*` and title matching. Killing the panel
  directly proved more reliable than fighting the rules.

## Still open

- Reboot test for kiosk auto-start
- Camera framing for portrait (the real remaining work)
- Arcade joystick: no encoder attached yet, so `GamepadAim` is still unverified
- **Credit / payment flow — does not exist.** No coin-op concept in the game at all.


## Portrait camera fit (2026-09-21)

`Assets/Scripts/Logic/PortraitCameraFit.cs`

Unity's `orthographicSize` fixes the **vertical** half-height, so horizontal view
scales with aspect. `GameScene` uses size 20:

| | Aspect | Horizontal view |
|---|---|---|
| Landscape 1920x1044 | 1.839 | 73.5 world units |
| Portrait 1080x1920 | 0.5625 | **22.5 world units** (31%) |

That is why the enemy tower was clipped — the camera was behaving correctly, the
screen was just the wrong shape for it.

The component keys the camera off a target **width** instead. It attaches itself at
runtime (`RuntimeInitializeOnLoadMethod` + `sceneLoaded`), so **no scene files were
edited** — important with decompiled assets. Landscape returns early and is left
exactly as authored.

### Tuning, measured on device

| Setting | Result |
|---|---|
| `TARGET_WORLD_WIDTH = 34` | Nothing clipped, but grey bars top **and** bottom |
| `TARGET_WORLD_WIDTH = 30` (current) | Nothing clipped, bottom bar **gone**, top seam faint |

The bars appear because the background art only covers ~40 world units vertically.
Zooming past that shows bare camera. 30 units cuts the excess from 51% to 33%.

Also sets the camera to clear to sky blue so the gap above the artwork blends.

### Known remaining issue

The clear colour is a fixed **daytime** blue, so on **night scenes** there is a faint
seam at the very top where the darker sky art ends.

**Proper fix is art, not code:** extend the sky/ground sprites to cover ~55 world
units instead of ~40. Then the camera can zoom freely with no bars and no seam.
Alternatively, sample each scene's sky colour instead of hardcoding one.


## Camera fit — FINAL (2026-09-21)

`Assets/Scripts/Logic/PortraitCameraFit.cs` — final form does exactly one thing:
in portrait, scale orthographic size 1.5x **in GameScene and PvPScene only**.

### Iteration history (4 regressions, all reverted)

| Attempt | Outcome |
|---|---|
| Absolute target world width | Fixed clipping but **broke the menu** (scenes use different base sizes: GameScene 20, MainScene 10/8) |
| + sky-blue camera clear colour | **Washed out the BYV publisher splash** — applied to every camera |
| Proportional (1.5x of authored size) | Menu scale fixed; splash still washed out |
| Removed clear-colour override | Splash correct again |
| **Scoped to gameplay scenes only** | Menu fully restored, gameplay keeps wider view |

**Lesson:** the problem existed in 2 scenes; every global fix broke others. Should
have scoped it from the start.

### Verified final result

- **Menu:** identical to original — full-bleed art, catapult visible, no bands
- **Splash:** correct, full colour on black
- **Gameplay:** enemy tower + stickman fully visible (was clipped off the left edge)
- **Remaining:** grey band at top of gameplay (~11%) where background art ends

## Deployment gotchas (measured)

- After `waydroid app install`, the package manager needs **~120 s** before
  `cmd package resolve-activity` returns the activity.
- Even then, **dexopt is still running** (`DexInv ... END (success)` in logcat).
  Launching before that finishes gives
  `Error: Activity class ... does not exist` or an immediately-exiting process.
- **Wait for the DexInv success line, not a fixed sleep.**

## Startup hang — NOT YET FIXED

First launch after install takes **4-6 minutes** to reach the menu. Unity log shows:

    IERequestToIpAPI info:{"status":"success","country":"Brazil",...}
    Logic.PromoCheckingCat2:CheckShowPermissions()
    AsyncDownloadF4AConfig url:            <- empty URL
    AndroidJNISafe.FindClass ... exception  <- missing Java class

The F4A promo/ads SDK does a geo-IP lookup then waits on a remote config with an
empty URL. CPU sits at 130%+ during part of it, so it is partly real work
(first-run asset processing) and partly SDK timeouts.

**For an arcade cabinet that boots into the game, a 4-6 minute startup is a
blocker.** Likely fix: stub out the F4A promo/config init, since its ads are
already dead (no DEFINE_ADMOB, SDK absent).

## Video recording

- Android `screenrecord` is **useless under Waydroid** — 6 fps for the game, but
  only 3.8 fps for Android's own launcher, proving it is a capture artifact.
- **`wf-recorder` (installed) works**: steady 30 fps, 1920x1080 from the compositor.
  Record with: `wf-recorder -f out.mp4 -c libx264 -r 30`
