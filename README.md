# RidgeRunner

RidgeRunner is a Unity 6 time-trial racing prototype built around a complete race loop: vehicle selection, checkpoint validation, lap timing, off-track penalties, live HUD feedback, minimap tracking, and end-of-race telemetry.

## Demo

Watch the [RidgeRunner gameplay demo](https://github.com/Yohanes-Mk/RidgeRunner/releases/download/v1.0.0/ridge-runner-gameplay-demo.mov), or visit the [v1.0.0 release](https://github.com/Yohanes-Mk/RidgeRunner/releases/tag/v1.0.0).

## Systems implemented

- Vehicle selection that spawns the selected car and connects the camera, HUD, minimap, and speed display.
- Ordered checkpoint progression with three-lap timing, split capture, best-lap persistence, and a retry flow.
- Off-track detection that applies and removes a 30% torque and top-speed penalty in real time.
- Session telemetry for max speed, lap times, checkpoint splits, total time, and off-track time.
- A minimap that follows the selected vehicle and aligns the player indicator to heading.

## Project structure

```text
Assets/
  Scripts/                Race-flow, UI, checkpoint, telemetry, and minimap scripts
  OffTrackManager.cs      Real-time off-track penalty system
Packages/                 Unity package manifest and lockfile
ProjectSettings/          Unity 6 project configuration
```

## Opening the project

The source targets **Unity 6.0.0.59f2**. Open the repository in Unity Hub and let Unity resolve the packages listed in `Packages/manifest.json`.

This public repository intentionally excludes the playable build, generated Unity folders, and third-party Asset Store content. To reproduce the full scene, install equivalent vehicle-controller, road/terrain, and environment assets, then wire the included scripts to the scene objects.

## Tech

C#, Unity 6, Universal Render Pipeline, Cinemachine, Unity Input System, Unity UI, and PlayerPrefs.

## Attribution

RidgeRunner's original race-flow and telemetry scripts are included here. Third-party Asset Store packages and their assets are excluded and remain subject to their respective licenses.
