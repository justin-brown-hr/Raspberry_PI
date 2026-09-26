# SmashFest — Next steps (from client WhatsApp, 2026-09-23)

Source: `chathistory/image1-5.png`. Client writes in Portuguese; translations below are ours.

## The big picture the client explained

The game is **not** a standalone toy — it is the **entry gate to the plush-toy claw machine**:

> "A pessoa paga, joga e de acordo com o resultado no jogo, ganha 1, 2 ou 3 tentativas na grua.
> Isso é o mais importante." — *The person pays, plays, and depending on their score wins
> 1, 2 or 3 tries on the claw. This is the most important part.*

Flow the client's own mock-ups show ("COMO JOGAR?"):
1. **Aproxime sua pulseira** — guest taps their wristband, which releases one game
2. **Jogue na tela** — play, score as many points as possible
3. **Ganhe tentativas** — the score is worth 1, 2 or 3 tries on the claw

Hardware the client describes:
- A **second Raspberry** closes a **relay** when the guest's wristband is read → that is the credit
- **One button starts the game** (credit), **a different button fires the ball** — must be two buttons
- Difficulty target: **easy to win 1 try, very hard to win 3**

Client's stated fears:
- People have never seen a machine like this → the TV must **explain itself** with an animation
- Two people at the machine at once → while someone is at the claw, the game must be blocked
- Credits "leaking" to the wrong person (someone finishes the game, someone else takes the tries)

---

## Step 0 — Update the game (DONE on the build host, 2026-09-23)

The client listed these at 4:21 PM as "modify the parts I asked for":

- [x] **Pontuação** — score shown during play ("PONTOS" + running total), "+10/+20"
      floats up from every object knocked off. 10 points normally, 20 for stone/ice/box,
      +200 for clearing a phase, +50 per unused ball
- [x] **Ter 3 bolinhas apenas** — 3 balls per phase
- [x] **A mira deve começar fora da área** — crosshair now starts low and left of the
      stack (viewport 0.14, 0.28), so the player must aim before firing
- [x] **Mensagens mais alegres e coloridas** — gradient title, colour-cycling attract
      messages, darker backdrop so text pops, bigger pop animations
- [x] **Result screen shows the claw tries won** — "VOCÊ GANHOU N TENTATIVAS NA GRUA!"
      after a "FIM DE JOGO" screen with the total score
- [x] **Bigger targets** (our addition — the old levels were too sparse for 3 balls):
      levels **45 / 125 / 119** (73, 82 and 182 objects) instead of 1 / 2 / 3
- [~] Difficulty: thresholds set to **2200 → 2 tries, 3800 → 3 tries**, calibrated from
      three automated runs scoring 2760 / 2690 / 2270. Needs the client's sign-off
- [x] Rebuilt, deployed to the Pi and verified on the real screen (portrait, full-screen)
- [x] Added a dark plate behind the FASE/PONTOS HUD — the taller stacks reach the top of
      the screen and made the text unreadable without it

## Step 1 — Scoring → claw tries (built; needs the client's numbers)

- [x] Points model: 10/object, 20 for stone-ice-box, +200 cleared phase, +50 per unused ball
- [x] Running total during play, final total on the result screen
- [x] Result screen states the prize: "VOCÊ GANHOU N TENTATIVAS NA GRUA!"
- [x] **3 tries lowered to 3300** on the client's instruction (was 3800)
- [ ] 2 tries is still our number (2200) — he has not commented on it
- [ ] Watch real children play and adjust (a child will score well below our test bot)

## How the hardware actually works (client answered 2026-09-24, image5)

This replaces our earlier guesses:

- **Credit comes in as a button press.** The other Raspberry only reads the wristband and
  asks the hotel server whether it has money. If yes, **it closes a relay for 1 second**,
  and that relay is **wired in parallel with the button we designate as START**.
  So our game sees an ordinary button press — no network, no GPIO input to write.
- **That other board does NOT know when the claw is in use.** ("Não, o outro raspberry não
  saberá quando alguém estará usando a grua.")
- **We are the ones who credit the claw.** Our Pi must pulse a relay **1, 2 or 3 times** to
  give the player the tries they won. Client sent a photo of the relay module.
- **The claw has a programmable time per try, he is planning 20 seconds.** So after the
  game we hold a **wait screen of tries × 20 s** (1 try = 20 s, 2 = 40 s, 3 = 60 s) during
  which the game cannot be started. That is how we stop two people using the machine at once.
- **Score for 3 tries: lower it to 3300** ("imagino que você deva reduzir para 3300 pontos").
- He says a few things in the game are still wrong and he will tell us later.

## Step 2 — Two buttons: START/credit and FIRE (built, mapping unconfirmed)

- [x] **START button** starts a game — the game no longer starts on any button
- [x] **FIRE button** launches the ball, separate from START
- [x] Codes accepted: START = JoystickButton1 / Enter / key 1; FIRE = JoystickButton0 /
      Space / Ctrl / Z. Verified on the Pi: Enter starts, Space fires
- [ ] **Confirm which physical button on the cabinet sends which code** — needs someone to
      press each one at the machine (see "I need your action" below)

## Step 3 — Award the claw tries (relay out) — built, waiting on hardware

- [x] Bridge built: the game sends `AWARD <tries> <score>` by UDP to the host across the
      Waydroid bridge (192.168.240.1:47801); `device/cabinet-award-listener.py` pulses the
      relay once per try. Runs as the pi5 user - no root, no systemd
      (Waydroid's shared storage is root-only, so the file route needed root; UDP does not)
- [x] Verified end to end on the cabinet: a real game sent `AWARD 1 1770` from the Android
      container and the listener logged one pulse
- [x] Launcher starts the listener at boot (currently with `--dry-run`)
- [x] Pulse 300 ms, 0.7 s gap, GPIO line 17 on `gpiochip0` (Pi 5 header), max 3 credits
- [ ] **Wire the relay and drop `--dry-run`** (see "I need your action")
- [ ] An audit copy of each award is also written inside the app's storage

## Step 4 — Wait screen while the player is at the claw (done)

- [x] After awarding, the screen holds for **tries × 20 s** and no game can be started
- [x] "AGUARDE SUA VEZ — Tem gente pegando os bichinhos na grua!" with a live countdown
- [x] Verified on the cabinet (1 try → 20 s countdown shown)
- [x] Seconds per try is one constant (`ClawSecondsPerTry`) if he reprograms the claw

## Step 5 — "How to play" animation in attract mode

The client asked for this twice; it matters as much to him as the game:

- [ ] Animated 3-step explainer looping when idle, following his own mock-up:
      tap wristband → play → win 1-3 tries
- [ ] Show the "= 1 TENTATIVA / = 2 TENTATIVAS / = 3 TENTATIVAS" idea visually
- [ ] Keep it readable from a few metres away, in Portuguese, colourful
- [ ] Client suggested a video; an in-game animation is better (no video player, always sharp)

## Step 6 — Cabinet hardening

- [ ] Auto-restart if the game crashes
- [ ] 24 h soak test in attract mode (memory, temperature)
- [ ] Demo video for the client once Steps 0–2 are in

---

## I need your action

1. **Press each cabinet button** while I watch the codes, so START and FIRE land on the
   right physical buttons. The wristband relay must be wired across whichever button
   ends up as START.
2. **Wire the relay** that credits the claw: module IN to GPIO line 17 (header pin 11),
   5 V and GND, contacts across the claw's credit input. Then I remove `--dry-run`.

## Still open with the client

1. **Which physical button is START and which is FIRE** — needs someone at the cabinet to
   press each one while we read the codes
2. **The relay that credits the claw** — which GPIO pin, and who wires it
3. **Confirm 20 s per try** once the claw is actually programmed
4. **2 tries at 2200 points** — he only gave us the number for 3 tries
5. "Algumas coisas no jogo ainda estão erradas" — waiting for his list
