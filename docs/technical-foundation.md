# Reusable Unity Adventure Foundation

This repository includes a story-neutral technical foundation for a cinematic 2D point-and-click adventure. It is designed to work in plain Unity first, then route into PowerQuest once PowerQuest rooms are authored.

## Runtime Namespace

All new reusable systems live under:

`GameFoundation`

Core scripts are in:

`Assets/Game/Scripts`

## Core Systems

- `GameTypes.cs`: shared enums and serializable data.
- `GameGlobals.cs`: persistent progress, flags, visits, inventory ownership, and settings.
- `GameSceneManager.cs`: new game, continue, scene changes, fades, autosave.
- `GameSaveSystem.cs`: JSON save/load/reset through `Application.persistentDataPath`, with WebGL `PlayerPrefs` storage.
- `GameInventory.cs`: neutral inventory/case-file/evidence catalog.
- `GameDialogue.cs`: compact cinematic subtitle flow.
- `GameNarration.cs`: optional narrator playback for pre-generated clips, subtitles, and future private TTS proxy integration.
- `GameHotspot.cs`: contextual click/inspect/use-item room interactions.
- `GameRoom.cs`: room definition holder.
- `GameRoomInput.cs`: hotspot routing without a visible verb bar.
- `GameUI.cs`: title/menu hooks, compact labels, subtitles, inventory strip, fade overlay, debug text.
- `GameSettings.cs`: browser-safe audio unlock and settings.
- `GameAtmosphere.cs`: room tone, overlays, flicker, shake, sting, UI click hooks.
- `GamePowerQuestBridge.cs`: reflection-only optional PowerQuest bridge.
- `GameDebug.cs`: F1 panel, room jumps, grant item, reset save/progress.
- `GameBattle.cs`: optional removable overlay battle module.

## PowerQuest Strategy

The foundation compiles without PowerQuest.

`GamePowerQuestBridge` detects `PowerTools.Quest.PowerQuest` by reflection. If PowerQuest is missing, scene changes fall back to Unity `SceneManager`. If PowerQuest is present, room changes pass through the bridge, where project-specific room lookup can be added once final PowerQuest rooms exist.

This keeps the room architecture PowerQuest-compatible without making PowerQuest a hard compile dependency.

## Room Structure

Each room should have:

- a `GameRoom` with a `GameSceneDefinition`
- a camera
- an event system
- UI canvas
- placeholder or final background art
- one or more `GameHotspot` objects
- optional `GameAtmosphere`

Starter room names:

- `RoomTitle`
- `RoomPrototypeA`
- `RoomPrototypeB`
- `RoomPrototypeC`

## Hotspot Structure

`GameHotspot` supports:

- contextual left-click interactions
- right-click inspect
- selected item use
- required flags
- set flags
- item collection
- room transition
- dialogue beat
- hover label
- direct calls from future PowerQuest hotspot scripts

## Flags

Use flags for story, puzzle, and dialogue state.

- `GameGlobals.HasFlag(flag)`
- `GameGlobals.SetFlag(flag)`
- `GameGlobals.SetPuzzleFlag(flag)`
- `GameGlobals.SetDialogueFlag(flag)`

Dialogue one-shot lines use dialogue flags so repeated subtitle playback can be controlled without special-case scripts.

## Inventory / Case File

`GameInventory` is deliberately neutral. Items can represent inventory objects, evidence, case files, clues, or keys.

Define catalog entries as `InventoryItemData`, then collect by item id:

`GameInventory.Instance.Collect("item-id")`

## Save / Load

`GameSaveSystem` serializes `GameSaveData` with `JsonUtility`.

Desktop/editor saves write to:

`Application.persistentDataPath/save.json`

WebGL saves use browser-backed `PlayerPrefs`.

## Narration / Voice

`GameNarration` is intentionally browser-safe:

- It can play imported `AudioClip` voice lines by `lineId`.
- It falls back to cinematic subtitles if a clip is missing.
- It exposes an `ExternalTextToSpeechProxy` mode as an integration point only.
- It does not include an OpenAI API key, SDK dependency, or direct browser call.

For OpenAI narrator voices, generate audio with the text-to-speech API in a private environment, then import the files into `Assets/Game/Audio`, or route requests through a private backend. Whisper is useful for player speech input or transcription later, but it is not the narrator voice system.

## WebGL / Pages

Use the editor menu:

`Game Foundation -> WebGL -> Build WebGL and Copy to Docs`

The build process:

1. switches to WebGL
2. disables WebGL compression for static hosts without custom headers
3. builds to `Builds/WebGL`
4. patches the shell for responsive canvas scaling
5. copies output to `docs`

For GitHub Pages, either publish `docs` from `main`, or use the included GitHub Actions workflow with a `UNITY_LICENSE` secret.

## Bootstrap

Use:

`Game Foundation -> Bootstrap -> Run Full Bootstrap`

This ensures folders, creates placeholder art, generates starter scenes, adds build scenes, and sets the WebGL target.

## Replacing Placeholder Content

Keep the systems and replace:

- room backgrounds
- character sprites
- hotspot ids/text
- inventory catalog
- dialogue beats
- atmosphere hooks
- room transition graph

The foundation is intentionally story-neutral so any final GDD can be layered on top.
