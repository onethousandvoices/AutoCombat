# AutoCombat

A Unity 6 auto-combat prototype where a player navigates a closed arena and automatically engages nearby enemies with melee attacks.

## Overview

The player moves freely using WASD with a third-person orbital camera. When the player stops and an enemy is within attack range, combat triggers automatically - the character lunges toward the closest target, deals damage, and returns to its original position. A visual indicator under the player's feet shows the attack radius, turning red when hostiles are in range.

Enemies patrol the arena along randomized waypoint paths. They avoid each other and the player using velocity-based obstacle avoidance (ORCA). When killed, enemies play a death animation and respawn after a configurable delay. A kill counter tracks progress on screen.

## Key Features

- **Auto-combat system** - attacks trigger automatically when idle and enemies are nearby
- **Orbital camera** - mouse-controlled camera orbiting the player with smooth follow
- **Camera-relative movement** - directional input adapts to current camera orientation
- **Enemy AI** - waypoint patrol with collision avoidance between all agents
- **Object pooling** - enemies are recycled, not destroyed and reinstantiated
- **Hit feedback** - enemies flash and shake on damage
- **Reactive UI** - kill counter updates via data binding, not polling
- **Hot-reload configs** - all gameplay parameters are ScriptableObjects editable at runtime

## Tech Stack

| Technology | Role |
|---|---|
| Unity 6 (6000.0.59f2) | Engine |
| Universal Render Pipeline | Rendering |
| VContainer | Dependency injection |
| DOTween | Animation and tweening |
| UniRx | Reactive data binding |
| New Input System | Player input |

## Project Structure

The codebase follows an MVC pattern:

- **Models** hold reactive game state with no logic
- **Views** are passive MonoBehaviours that only handle visuals
- **Controllers** are plain C# classes managed by the DI container that drive all gameplay logic

All controllers are injected by interface, enabling clean separation of concerns and testability.