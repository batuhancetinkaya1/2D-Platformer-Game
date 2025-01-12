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
     
     ![image](https://github.com/user-attachments/assets/13d54304-8528-4ebb-bf33-6c46d0aa1ba9)

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

  ![WhatsApp Görsel 2024-12-27 saat 19 39 55_3508a9a9](https://github.com/user-attachments/assets/1fc0e7ef-53ee-4620-9714-ec154bd5f06c)
  ![WhatsApp Görsel 2024-12-27 saat 19 39 28_abcb07c3](https://github.com/user-attachments/assets/8b5fdefb-8609-4fa9-97c7-412ec8455b1b)
  ![WhatsApp Görsel 2024-12-27 saat 19 38 59_c2b794b3](https://github.com/user-attachments/assets/3910c1d6-02ac-4ef6-b654-4a1df1d1cba8)
  ![WhatsApp Görsel 2024-12-27 saat 19 38 29_d1851c66](https://github.com/user-attachments/assets/059e85a0-e4ed-4daf-a78b-2fb50ddb5239)




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
The key and door sprites, were carefully crafted by hand to give the game a unique and personalized aesthetic.

### Key Sprite
![image](https://github.com/user-attachments/assets/66c32336-d24c-4807-8550-dfc1ef9a747f)


### Door Sprite
![image](https://github.com/user-attachments/assets/1be2e48a-08d2-4a07-bcb8-d9210323c465)

---

## Technologies Used
- **Unity**: Game engine for development.
- **C#**: Programming language for scripting.
- **TextMeshPro**: For UI text.
- **Version Control**: Git and GitHub for collaboration.

---

## License
This project is licensed under the [MIT License](LICENSE).


