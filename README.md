# Little Adventurer

Little Adventurer is a 2D pixel-art action platformer prototype built with Unity. Take control of a nimble adventurer, move through the level, and fight Skeleton Knights using responsive platforming and melee combat.

## Current features

- Walking, running, jumping, and directional movement
- Melee attacks with hit detection and knockback
- Skeleton Knight enemies with patrol, target detection, attack, hurt, and death states
- Player and enemy health systems with temporary invulnerability after taking damage
- Collectible healing potions
- Health UI and floating damage/healing feedback
- Parallax scenery, background music, and sound effects
- Restart and quit flows
- Keyboard, mouse, and partial gamepad input support

## Controls

| Action | Keyboard and mouse |
| --- | --- |
| Move | `WASD` or arrow keys |
| Run | `Left Shift` |
| Jump | `Space` |
| Attack | Left mouse button |
| Restart level | `R` |
| Quit | `Esc` |

## Requirements

- Unity `2022.3.30f1` (LTS)
- A platform supported by Unity's Universal Render Pipeline

## Getting started

1. Clone the repository.
2. Open the repository folder as a project in Unity Hub.
3. Allow Unity to import the project assets and packages.
4. Open `Assets/Scenes/GeneralScene.unity`.
5. Enter Play Mode.

Unity generates the `Library`, `Temp`, `Logs`, and IDE project files locally. They are intentionally excluded from version control.

## Project structure

- `Assets/Scenes` — playable and supporting scenes
- `Assets/Scripts` — player, enemy, combat, interaction, and audio logic
- `Assets/Characters` — character prefabs, animations, and input actions
- `Assets/UI` — health and gameplay interface assets
- `Packages` — Unity package manifest and lock file
- `ProjectSettings` — shared Unity project configuration

## License

The original project code is available under the [MIT License](LICENSE).

Third-party art, audio, fonts, and other imported assets remain subject to their respective authors' licenses and terms included alongside those assets.
