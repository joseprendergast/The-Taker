# The Taker

Desktop Unity + PowerQuest prototype based on the supplied GDD.

## What is implemented

- PowerQuest is imported under `Assets/PowerQuest`.
- The PowerQuest default template is installed under `Assets/Game`.
- The demo has been rethemed into a playable narrative prototype for Fanto.
- The prototype covers all nine ghosts from the GDD.
- Each ghost has an explore/evidence/confront/judge loop.
- Heaven/Hell choices are tracked.
- Doll encounters, Hell count, Seelvia's judgement, self-judgement, and endings are tracked.
- Desktop build menu items are available under `The Taker/Build Desktop`.

## How to open

Open this folder in Unity Hub as a Unity project:

`/Users/jprendergast/Documents/The Taker`

Recommended editor family: Unity 2022 LTS or newer. If Unity asks to upgrade metadata, allow it.

## How to play the prototype

- Start from `Assets/Game/Rooms/Title/SceneRoomTitle.unity`.
- Click `NEW`.
- Left click to walk or interact.
- Right click to inspect.
- Click the forest/hotspots to gather evidence and context.
- Click the well to confront and judge the current ghost.
- Use the sky/status hotspot to view progress or the unlocked ending.

## Build

In Unity, use:

- `The Taker/Build Desktop/Mac`
- `The Taker/Build Desktop/Windows`
- `The Taker/Build Desktop/Linux`

The build output goes into `Builds/`.
