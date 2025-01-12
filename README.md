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
The **2D Platformer Game** is an engaging Unity-based platformer featuring challenging level, dynamic backgrounds, and an exciting final boss fight. Players navigate through various obstacles, defeat enemiess to progress through the game.

---

## How to Install
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/batuhancetinkaya1/2D-Platformer-Game.git
   ```
   - If you are not using Git or familiar with it, you can directly download the project as zip. After unzipping, use "2D Platformer Game" file to run on Unity.
2. **Open the Project in Unity**:
   - Ensure you have Unity (version 2022.3.44f1) installed. Other versions may not be competible with our game.
   - Open Unity Hub and click on `Add` > `Add project from disk`.
     
     ![image](https://github.com/user-attachments/assets/13d54304-8528-4ebb-bf33-6c46d0aa1ba9)

   - Navigate to the folder where you cloned the repository and select it.

3. **Install Dependencies**:
   - Check the Unity Package Manager and install any missing dependencies.

4. **Build and Play**:
   - Go to `File > Build Settings`, select your target platform, and click `Build`.

---

## How to Play
### Starting the Game in Unity Editor
1. **Open the Menu Scene**:
   - Navigate to `Assets > Scenes` and double-click on `MenuScene` to open it in the Unity Editor.
     
     ![image](https://github.com/user-attachments/assets/427c0f70-66eb-4ce5-bff4-d0befd1a38d7)



2. **Play the Game**:
   - In the Unity Editor, click the **Play** button (not the scene's play button).
     
     ![image](https://github.com/user-attachments/assets/ed04fc84-2398-4186-bf0e-c86165afd029)


3. **Enjoy the Game**:
   - The main menu will load, and you can begin playing.

### Running the Built Game
1. Launch the game through the Unity Editor or the built executable. If you are not familiar with Unity do not try to get built.
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
- **Star Collection**: Players collect stars to give the CAT. Collecting five stars causes the guarding cat to move away, allowing access to the key room.
- **Dynamic Backgrounds**: The game's background changes based on the system's current time for an immersive experience. ( +50 puanımızı isteriz :) )

  **Morning**
  ![WhatsApp Görsel 2024-12-27 saat 19 38 29_d1851c66](https://github.com/user-attachments/assets/059e85a0-e4ed-4daf-a78b-2fb50ddb5239)
  **Noon**
  ![WhatsApp Görsel 2024-12-27 saat 19 38 59_c2b794b3](https://github.com/user-attachments/assets/3910c1d6-02ac-4ef6-b654-4a1df1d1cba8)
  **Evening**
  ![WhatsApp Görsel 2024-12-27 saat 19 39 28_abcb07c3](https://github.com/user-attachments/assets/8b5fdefb-8609-4fa9-97c7-412ec8455b1b)
  **Night**
  ![WhatsApp Görsel 2024-12-27 saat 19 39 55_3508a9a9](https://github.com/user-attachments/assets/1fc0e7ef-53ee-4620-9714-ec154bd5f06c)
  
  
  




- **Final Boss Fight**: The game culminates in an intense Player vs Bot showdown, with the bot using basic AI mechanics.

### Level Progression
- **Key Mechanism**: The key must be retrieved to unlock the final door. Unlocking the final door triggers the arena fight, the last step of the game.

---

## Enemy Behavior
### Mushroom
![Idle](https://github.com/user-attachments/assets/6bcd5b37-6349-4061-bb73-bc470d7ca9c7)

- Patrols predefined paths.
- Engages the player by chasing them and performing melee attacks upon contact.

### Flying Eye
![Attack1](https://github.com/user-attachments/assets/9388426c-03f3-459c-80a0-3a1bff701927)

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


