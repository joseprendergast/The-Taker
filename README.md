# The Taker

Desktop Unity + PowerQuest prototype based on the supplied GDD.

Repository: <https://github.com/joseprendergast/The-Taker>

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
- Concept art has been extracted from the supplied GDD into `Assets/Game/ConceptArt/ExtractedFromGDD`.
- The title screen now uses extracted GDD art as a temporary landing screen.
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
- GitHub Pages requires a Unity WebGL build. The raw Unity project source is not directly playable in Pages.

## How to open

Open this folder in Unity Hub as a Unity project:

`/Users/jprendergast/Documents/The Taker`

Recommended editor family: Unity 2022 LTS or newer. If Unity asks to upgrade metadata, allow it.

## How to play the prototype

- Start from `Assets/Game/Rooms/Title/SceneRoomTitle.unity`.
- Click `PRESS START`.
- Click `NEW GAME`, or `CONTINUE` if a save exists.
- Left click to walk or interact.
- Right click to inspect.
- Click the forest/hotspots to gather evidence and context.
- Click the well to confront and judge the current ghost.
- Use the sky/status hotspot to view progress or the unlocked ending.

## Desktop build

In Unity, use:

- `The Taker/Build Desktop/Mac`
- `The Taker/Build Desktop/Windows`
- `The Taker/Build Desktop/Linux`

The build output goes into `Builds/`.

## GitHub Pages / WebGL publishing

GitHub Pages can host a Unity WebGL build, but it cannot run the Unity project source directly.

To publish a playable Pages version:

1. Open the project in Unity.
2. Install WebGL Build Support for the selected Unity version if needed.
3. Switch platform to `WebGL` in Unity Build Settings.
4. Build the WebGL player into a folder such as `Builds/WebGL`.
5. Copy the WebGL build output into a Pages branch or a docs folder, depending on the Pages setup.
6. In GitHub repo settings, enable Pages for that branch/folder.

Recommended Pages setup for this repo:

- Keep Unity source on `main`.
- Publish WebGL output from a separate `gh-pages` branch.
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
