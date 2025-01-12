# 2D Platformer Game

Welcome to the **2D Platformer Game** repository! This document provides a detailed guide to the project, including how to set it up, play, and additional information about the methods, systems, and characters used in the game.

## Table of Contents
1. [Introduction](#introduction)
2. [How to Install](#how-to-install)
3. [How to Play](#how-to-play)
4. [Controls](#controls)
5. [Features and Systems](#features-and-systems)
6. [Enemy Behavior](#enemy-behavior)
7. [Game Modes](#game-modes)
8. [Handcrafted Assets](#handcrafted-assets)
9. [Technologies Used](#technologies-used)
10. [License](#license)

---

## Introduction
The **2D Platformer Game** is an engaging Unity-based platformer featuring challenging levels, dynamic backgrounds, and an exciting final boss fight. Players navigate through various obstacles, defeat enemies, and unlock secrets to progress through the game.

---

## How to Install
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/batuhancetinkaya1/2D-Platformer-Game.git
   ```
2. **Open the Project in Unity**:
   - Ensure you have Unity (version 2022.3.44f1) installed.
   - Open Unity Hub and click on `Add` > `Add project from disk`.
     
     ![Unity Hub Add Project Screenshot](./screenshots/unity_hub_add_project.png)
   - Navigate to the folder where you cloned the repository and select it.

3. **Install Dependencies**:
   - Check the Unity Package Manager and install any missing dependencies.

4. **Build and Play**:
   - Go to `File > Build Settings`, select your target platform, and click `Build`.

---

## How to Play
1. Launch the game through the Unity Editor or the built executable.
2. To play the game directly, visit the following link:
   [Play on Itch.io](https://batuhancetinkaya.itch.io/2d-platformer).
3. Navigate through levels, collect stars, defeat enemies, and unlock new areas.

---

## Controls
| **Action**         | **Key**          |
|--------------------|------------------|
| Move Right         | `D`              |
| Move Left          | `A`              |
| Jump               | `Space`          |
| Attack             | `Mouse Left`     |
| Block              | `Mouse Right`    |
| Roll               | `X`              |

---

## Features and Systems
### Key Mechanics
- **Star Collection**: Players collect stars to unlock new areas. For example, collecting five stars causes the guarding cat to move away, allowing access to the key room.
- **Dynamic Backgrounds**: The game's background changes based on the system's current time for an immersive experience.
  
  ![Dynamic Background Screenshot](./screenshots/dynamic_background.png)

- **Final Boss Fight**: The game culminates in an intense Player vs Bot showdown, with the bot using basic AI mechanics.

### Level Progression
- **Key Mechanism**: The key must be retrieved to unlock the final door. Completing the final door triggers the end of the game.

---

## Enemy Behavior
### Mushroom
- Patrols predefined paths.
- Engages the player by chasing them and performing melee attacks upon contact.

### Flying Eye
- Patrols predefined paths.
- Avoids the player when engaged.
- Attacks from a distance by shooting fireballs.
- Performs melee attacks if the player gets too close.

---

## Game Modes
- **Standard Mode**: Progress through levels, defeat enemies, and complete objectives.
- **Arena Mode**: Choose from the following options:
  - **PvB**: Player vs Bots.
  - **PvP**: Player vs Player.
  - **AI vs AI**: Watch AI-controlled characters battle it out.

---

## Handcrafted Assets
All in-game assets, including the key and door sprites, were carefully crafted by hand to give the game a unique and personalized aesthetic.

### Key Sprite
![Key Sprite](2D Platform Game/Assets/Prefabs/ImagesSprite-0002.png)

### Door Sprite
![Door Sprite](./Assets/Prefabs/Images/Sprite-0003.png)

---

## Technologies Used
- **Unity**: Game engine for development.
- **C#**: Programming language for scripting.
- **TextMeshPro**: For UI text.
- **Version Control**: Git and GitHub for collaboration.

---

## License
This project is licensed under the [MIT License](LICENSE).


