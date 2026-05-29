# The Taker

Desktop Unity + PowerQuest prototype based on the supplied GDD.

Repository: <https://github.com/joseprendergast/The-Taker>

Shareable Pages URL: <https://joseprendergast.github.io/The-Taker/>

## Project status

This is an early playable narrative prototype, not a finished production game. It establishes the project in Unity, imports PowerQuest, and converts the starter adventure template into the first interactive version of **The Taker**.

The current goal is to prove the core structure:

- point-and-click desktop adventure controls
- Fanto's nine-ghost judgment loop
- evidence gathering before judgment
- Heaven/Hell choice tracking
- ending logic based on moral choices
- a build path for desktop targets

## Production branch

`main` is treated as the production branch for this repo. Large completed changes should be committed and pushed to `main` unless there is a specific reason to use a feature branch first.

## What is implemented

- PowerQuest is imported under `Assets/PowerQuest`.
- The PowerQuest default template is installed under `Assets/Game`.
- The demo has been rethemed into a playable narrative prototype for Fanto.
- A reusable, story-neutral Unity adventure foundation has been added under `Assets/Game/Scripts`.
- The foundation compiles without PowerQuest and detects PowerQuest later through `GamePowerQuestBridge`.
- Concept art has been extracted from the supplied GDD into `Assets/Game/ConceptArt/ExtractedFromGDD`.
- The GitHub Pages site now runs a lightweight browser prototype with `PRESS START`, `NEW GAME`, `CONTINUE`, ghost judgments, endings, and browser-local saves.
- Every new browser run starts with clear how-to-play instructions before the first ghost.
- The Pages prototype includes a browser-native narrator toggle for the instructions and story beats.
- The Pages prototype now uses extracted GDD concept art for the case screen, judgement screen, character moments, and scene backgrounds.
- The prototype covers all nine ghosts from the GDD.
- Each ghost has an explore/evidence/confront/judge loop.
- Heaven/Hell choices are tracked.
- Doll encounters, Hell count, Seelvia's judgement, self-judgement, and endings are tracked.
- Desktop build menu items are available under `The Taker/Build Desktop`.

## Story prototype flow

The prototype compresses the GDD into a single playable loop:

1. Start on the title screen.
2. Press start.
3. Choose `NEW GAME` or `CONTINUE` when saves are available.
4. Enter the graveyard prototype room.
5. Explore hotspots to gather evidence and moral context.
6. Use the well as the judgment mirror.
7. Confront the current ghost.
8. Send the ghost to Heaven or Hell.
9. Repeat until all nine ghosts are judged.
10. Judge Fanto himself.
11. Unlock the ending that matches the run.

The nine ghosts currently represented are:

- The Innocent
- The Everlasting Valentine
- The Poltergeist
- The Doppelganger
- The Faceless Lady
- The Forgotten
- The Guilty
- The Evil
- Seelvia

## Current limitations

- The title screen uses extracted GDD art, but most in-game room/player art is still PowerQuest demo placeholder content.
- The ghost encounters are narrative/prompt prototypes, not full unique battles or puzzles yet.
- Fanto, Doll, Seelvia, demons, and custom ghost art still need final sprites and animation.
- Audio is still placeholder/template audio.
- The desktop build menu exists, but a Unity Editor installation is required to produce executable builds.
- GitHub Pages currently serves a lightweight browser prototype. A true Unity-powered browser build still requires a Unity WebGL export.

## How to open

Open this folder in Unity Hub as a Unity project:

`/Users/jprendergast/Documents/The Taker`

Recommended editor family: Unity 2022 LTS or newer. If Unity asks to upgrade metadata, allow it.

## Reusable technical foundation

The reusable architecture is documented here:

`docs/technical-foundation.md`

Main editor entry point:

`Game Foundation` -> `Bootstrap` -> `Run Full Bootstrap`

Main runtime namespace:

`GameFoundation`

The foundation includes neutral systems for scene definitions, contextual hotspots, subtitles, browser-safe saves, inventory/case files, compact cinematic UI, atmosphere hooks, debug tools, optional battle overlay, and a reflection-based PowerQuest bridge.

## How to play the prototype

- Start from `Assets/Game/Rooms/Title/SceneRoomTitle.unity`.
- Click `PRESS START`.
- Click `NEW GAME`, or `CONTINUE` if a save exists.
- New games first show a how-to-play screen. Read it, then choose `START JUDGEMENT`.
- In the Pages prototype, use `NARRATOR ON` if you want the browser to read instructions and story text aloud.
- Left click to walk or interact.
- Right click to inspect.
- Click `SEARCH` to gather evidence and context.
- Once all three evidence marks are filled, open the judgement screen and choose `DAMNATION` or `SALVATION`.
- Use the sky/status hotspot to view progress or the unlocked ending.

## Narration and voice

The browser prototype uses the built-in Web Speech API for quick narration. It does not send text to OpenAI and does not need an API key. When the browser exposes several voices, the prototype prefers a low British male voice and lowers pitch/rate for a darker narrator tone.

For the Unity game, `Assets/Game/Scripts/GameNarration.cs` provides the production path:

- Use pre-generated narrator clips for GitHub Pages/WebGL whenever possible.
- Keep narration optional and user-triggered so browser audio rules are respected.
- Store OpenAI-generated files as imported audio assets, or call OpenAI through a private server/proxy.
- Never put an OpenAI API key inside `index.html`, Unity WebGL code, or any public GitHub Pages file.

OpenAI Whisper is for speech-to-text. For a narrator voice, use OpenAI text-to-speech instead, then import the generated audio into Unity or serve it through a secure backend. The current future-facing voice id is `onyx` for a darker male tone; `alloy` remains available if we want a more neutral voice later.

## Desktop build

In Unity, use:

- `The Taker/Build Desktop/Mac`
- `The Taker/Build Desktop/Windows`
- `The Taker/Build Desktop/Linux`

The build output goes into `Builds/`.

## GitHub Pages publishing

The current URL is:

<https://joseprendergast.github.io/The-Taker/>

Right now, the root `index.html` runs a hand-built browser prototype so the project can be shared immediately. That is not Unity or PowerQuest running in the browser.

For the real Unity game in GitHub Pages, use the included GitHub Actions workflow:

`.github/workflows/deploy-webgl-pages.yml`

Required GitHub Pages settings for the real Unity WebGL build:

- Source: `GitHub Actions`
- Custom domain: none
- HTTPS: enabled

Required repository secret:

- `UNITY_LICENSE`

The workflow cannot build until the repo has a valid Unity license secret. This is a Unity/GameCI requirement, not a project-code issue.

To add the license:

1. Go to GitHub repo `Settings`.
2. Open `Secrets and variables` -> `Actions`.
3. Create a repository secret named `UNITY_LICENSE`.
4. Paste a valid Unity license file/string for the Unity account.
5. Go to `Actions`.
6. Run `Build Unity WebGL and deploy Pages`.

The workflow is manual on purpose right now. After `UNITY_LICENSE` is configured and one WebGL deployment succeeds, the workflow can be changed to run automatically on pushes to `main`.

## Unity WebGL publishing

GitHub Pages can host a Unity WebGL build, but it cannot run the Unity project source directly. To build locally instead of through GitHub Actions:

1. Open the project in Unity.
2. Install WebGL Build Support for the selected Unity version if needed.
3. Switch platform to `WebGL` in Unity Build Settings.
4. Use `The Taker` -> `Build WebGL` -> `GitHub Pages`.
5. Deploy the generated `Builds/WebGL` folder through GitHub Pages, or use `Game Foundation` -> `WebGL` -> `Build WebGL and Copy to Docs`.

Recommended Pages setup for this repo:

- Keep the source repo public.
- Use `GitHub Actions` as the Pages source for the Unity WebGL build.
- Do not commit Unity `Library`, `Temp`, `Obj`, or generated desktop build folders.

## Suggested next milestones

- Replace demo characters with Fanto and Archangel placeholder sprites.
- Add a custom graveyard room background.
- Split the nine ghosts into separate rooms or encounter scenes.
- Add a real evidence inventory.
- Build the Faceless Lady chase prototype.
- Build one full ghost encounter end to end, including puzzle, battle, and judgment.
- Add WebGL build automation for GitHub Pages.

## Repo hygiene

The `.gitignore` excludes Unity-generated folders such as `Library`, `Temp`, `Obj`, `Build`, `Builds`, `Logs`, and `UserSettings`.
