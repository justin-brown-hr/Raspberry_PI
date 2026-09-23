# SmashFest — Arcade Cabinet Port

Replaces The Catapult on the Raspberry Pi 5 cabinet (portrait TV, joystick + button, next to the claw machine).

**Legend:** `[ ]` todo · `[x]` done · `[~]` in progress · `[!]` blocked · `[?]` needs answer from client

## What we have

- `SmashFest/` — Unity **2022.3.62f2** project (Flow Games, "Smash Fest"), 3D, URP
- 1001 levels in `Assets/Resources/levels/prod-14/`, each level has several stages
- Aim = mouse/touch raycast (`Cannon.cs` `GetPointerViewportPosition`), fire on release (`GameController.cs` `OnClick`)
- Carries mobile SDKs we don't want: AppLovin ads, Adjust, IAP, backend login/leaderboard, geo, A/B testing, rate-us
- Unity **2022.3.62f2** installed at `~/.local/unity-2022.3.62f2` (2021.3 kept for The Catapult). Env: `. ~/.local/unity-sdk/env-2022.sh`

## Client requirements

1. Everything vertical (portrait)
2. Crosshair moved by the joystick, one button fires the ball
3. Carefully chosen levels, **not too hard**
4. Happy sounds
5. **Attract mode**: when nobody is playing, instruction messages cycle on screen
6. **Claw busy**: when someone is using the claw machine, show "aguardando jogador terminar sua jogada"

---

## Phase 0 — Retire The Catapult

- [ ] **Back up first** — nothing in this repo is committed. Archive `The Catapult/`, the APKs and demo videos (tar.gz outside the repo) before deleting
- [~] The Catapult **stopped** on the Pi (`am force-stop`), still installed — uninstall pending your OK
- [x] `~/.local/bin/cabinet-start.sh` now launches `com.pj.smashfest` (original saved as `cabinet-start.sh.catapult.bak`); runs at boot from `~/.config/labwc/autostart`
- [ ] Delete `The Catapult/` and its videos from the repo
- [ ] Move the Catapult notes in `TODO.md` to an archive section (keep the Pi/Waydroid/VNC setup notes — still valid)

## Phase 1 — Build environment

- [x] Install Unity 2022.3.62f2 + Android module (no root, `~/.local/unity-2022.3.62f2`), md5 verified
- [x] NDK r23b (2022.3 requirement) → `android-sdk/ndk/android-ndk-r23b`; JDK 11 reused; bee_backend shim reapplied
- [x] Build script `Assets/Editor/PiBuild.cs` (arm64 / IL2CPP / GLES3 / portrait, `com.pj.smashfest`)
- [ ] Move license to the new editor version
- [ ] Open project, let it import, record compile errors (decompiled code — expect some)
- [x] Target decided: Android APK under Waydroid (no Linux ARM64 in 2022.3.62f2 module list)

## Phase 2 — Make it build clean

- [x] Compiles clean (no code errors) in 2022.3.62f2
- [~] IAP init skipped + Play Billing stripped from the Gradle build (`Assets/Editor/CabinetGradlePostprocess.cs`); ads/analytics/backend still linked but unused — game runs offline
- [x] Menu, tutorials, lives, coins, win/fail popups and settings button bypassed
- [x] Portrait 1080×1920 confirmed in playtest screenshots
- [~] No blocking network call seen; runs fine on the Pi (still to confirm with networking fully off)
- [x] APK builds: `Build/SmashFest-cabinet.apk` (79 MB, arm64/IL2CPP)
- [x] Installed and launched on the Pi (`com.pj.smashfest` under Waydroid)

## Phase 3 — Joystick crosshair

- [x] Add on-screen crosshair (big, bright, kid-friendly) — `Cabinet/CabinetDirector.cs`, procedural red/white reticle
- [x] Joystick moves crosshair with acceleration; clamped — `Cabinet/CabinetInput.cs`
- [x] Feed crosshair position into `Cannon` aim instead of `Input.mousePosition`
- [x] Button fires (any joystick button / space / enter / click)
- [x] Cannon turns live to follow the crosshair
- [x] Keyboard + mouse fallback for bench testing
- [x] Verified on the Pi with injected key events (same kernel input path an arcade encoder uses): button starts a game, left/right move the crosshair, shots fire on and off target
- [ ] Test with the real arcade joystick encoder — **none attached to the Pi yet** (only USB keyboard + mouse)
- [x] Fixed: shots were refused when the crosshair sat beside the targets (`BoundingPlane` had no forward-facing fallback plane) — the button now always fires

## Phase 4 — Level selection & game flow

- [~] Levels 1, 2, 3 chosen (difficulty 0, 16–21 objects, 20 balls) — confirm by playtest; backup: 22
- [x] 3 levels per game
- [x] Balls: keep the original 20 per level (already generous)
- [x] Never "fail" hard: out of balls → "BOA TENTATIVA!" → next level
- [x] Session end → "PARABÉNS!" → back to attract mode
- [x] Splash goes straight to Gameplay; tutorials, lives, coins, win/fail popups bypassed

## Phase 5 — Sound

- [x] Reusing the game's own clips; win jingle on every level end, start sound on button press
- [ ] Listen to the audio mix on the cabinet (not verifiable headless)
- [ ] Make sure music is upbeat and loud enough for an arcade; add cheerful stingers if needed
- [ ] Attract-mode music/jingle (not too annoying on loop)

## Phase 6 — Attract mode (nobody playing)

- [x] Instruction messages written (Portuguese), Fredoka Bold font (OFL) for accents
- [x] Cycling messages, big text, pop animation, pulsing "APERTE O BOTÃO PARA JOGAR!"
- [x] Level 1 shown dimmed behind the attract screen
- [x] Idle timeout 45 s → attract mode
- [x] Any button press starts a game

## Phase 7 — "Claw busy" screen (after the game is done)

- [?] **How does the Pi know the claw is in use?** (GPIO signal from claw board, relay, network, button?) — blocker
- [x] Game side ready: `CabinetDirector.StopGame(msg)` / `Resume()`
- [ ] Read that signal on the Pi and pass it to the game (Waydroid → Android needs a bridge: file, socket, or broadcast)
- [ ] Overlay "Aguardando jogador terminar sua jogada" while active; block game start
- [x] Claw starts mid-game → stop the game

## Phase 8 — Cabinet integration

- [x] Cursor hidden, 60 fps target, screen never sleeps (in `CabinetDirector`)
- [x] Autostart verified: from a stopped session the launcher brings up Waydroid and starts the game in ~28 s, first attempt, straight to the attract screen
- [ ] Auto-restart if the game crashes
- [ ] Soak test: run 24h in attract mode, watch memory/temperature
- [ ] Record demo video for client approval

## Playtest status (2026-09-22)

Verified on the build host with an automated run (Linux build of the same code, portrait
1080×1920, screenshots in the scratchpad): attract screen with cycling messages → button
starts → FASE 1/3 → 2/3 → 3/3 → "PARABÉNS!" → back to attract. "BOLAS" counter, crosshair,
cannon aim and physics all behave.

Build environment notes (for the next session):
- `. ~/.local/unity-sdk/env-2022.sh` then
  `$UNITY_HOME/Editor/Unity -batchmode -nographics -projectPath . -buildTarget Android -executeMethod PiBuild.BuildAndroidArm64 -logFile Logs/build.log`
- `PiBuild.BuildLinuxTest` builds a desktop copy; run it with `-cabinetTest <dir>` to
  self-play and capture screenshots (needs `xvfb-run`)
- Gradle needed `~/.gradle/gradle.properties` pointing at the JDK trust store
  (Debian's OpenJDK default path doesn't exist here)
- Android SDK additions: build-tools 34.0.0, cmdline-tools 6.0, NDK r23b

## On-hardware verification (2026-09-22, Pi `garra`)

- SSH key installed on the Pi, so this session can reach it non-interactively
- APK installed via `waydroid app install`; app runs as `com.pj.smashfest`
- Attract screen → button → FASE 1/3 → aim → fire, all confirmed by screenshots
- Performance: 52 °C, no throttling, ~550 MB RAM, load fine — the Pi 5 is comfortable
- `waydroid shell` and `/dev/uinput` need root (sudo wants a password), so input was
  injected by writing to `/dev/input/by-id/...-event-kbd` (pi5 is in the `input` group).
  Helper: `/tmp/key.py` on the Pi — `python3 /tmp/key.py space|left|right|hold left 0.7`
- Screenshots: `grim /tmp/x.png` on the Pi, then `scp` back
- The bench TV is landscape, so the portrait game is pillarboxed; the cabinet screen is portrait

## Portrait rotation (2026-09-22)

The cabinet TV gets mounted rotated 90°, so the Pi now outputs a portrait image:

- `~/.config/kanshi/config` and `config.init`: `transform normal` → **`transform 270`**
  (suits a TV rotated **clockwise**; use `90` if it is turned the other way).
  Backups: `config.bak.landscape`, `config.init.bak.landscape`
- Waydroid was sizing its window from the TV's physical mode and picked 608×1080,
  which left the game in a small centred box. Fixed with
  `waydroid prop set persist.waydroid.width 1080` / `...height 1920`
  (run **without sudo**, with the session up — sudo loses the session env and fails).
  `waydroid.cfg [properties]` entries alone did **not** take effect.
- Result: game fills 1080×1920 edge to edge, controls verified after rotating
- **Reboot verified (2026-09-22)**: after `systemctl reboot` the Pi came back with
  `Transform: 270`, Waydroid at 1080×1920 and the attract screen up on its own —
  ~90 s from reboot to playable, no login or keyboard needed
- Rotation direction still unverified until the TV is physically turned (flip `270`→`90`
  in `~/.config/kanshi/config` **and** `config.init` if it comes up upside down)

## Cabinet handover state (2026-09-22)

- The Pi boots into SmashFest's attract screen with no desktop furniture visible
- The Catapult is stopped but still installed, and `cabinet-start.sh.catapult.bak` restores
  the old launcher, so reverting is two commands
- This session needed `.claude/settings.local.json` allow-rules for `ssh`/`scp` to the Pi;
  the Pi's sudo password lives in `credential.md` (line format: `... password: <pw>`)

## Decisions (2026-09-22)

- Order: **build the game first**, claw integration afterwards
- Claw "in use" signal: unknown yet — Phase 7 on hold
- Attract messages: client doesn't need to write them — we write them (Portuguese)
- Game starts on **any button press**
- **3 levels** per game
- Claw becomes busy mid-game → **stop** the game, show the waiting screen
- Target: **Android APK under Waydroid** — Unity 2022.3.62f2 has no Linux ARM64 player
- Deleting The Catapult: **not confirmed yet** — don't delete
