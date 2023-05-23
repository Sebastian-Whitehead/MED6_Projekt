# BCI Turn-Based Strategy Game
**Making Training Engaging for CP Rehabilitation**

[AV-production showcasing project](https://youtu.be/WKs0KS8mpzA)

---

Key Abbreviations:
- BCI: Brain-Computer Interface
- MI: Motor Imagery

---

## This project aims to address the following problem formulation:
**"How does 'charging/battery'- and 'interval'-based pacing compare, in terms of the player's engagement, in a video game designed for upper limb motor imagery training in children with cerebral palsy?"**
MI is to imaging moving a limb, that the BCI will read. This projects MI is open and closing ones hand.
The pacing "battery" is focusing on groupings of MI events, with longer rest periods, while "interval" focuses on more rapid singular MI events.

The development of this video game was undertaken as a bachelor project during the 6th semester of Medialogi at Aalborg University, from February to May 2023. It is a turn-based, point-and-click, fantasy-themed, top-view, action strategy game that can be controlled using a touch screen or a mouse, as well as utilizing BCI technology.

## Project Contributors:
- Charlotte Johansen
- Sebastian Whitehead
- Tobias Niebuhr Kiholm
- Tonko Emil Westerhof Bossen

Supervisor: Bastian Ilsø Hougaard

## Usage and Installation Instructions:
The Unity project is the folder called "BCI Training".
In the "Builds" folder, you will find two different builds of the game, "Battery" and "Interval". These are both simulated versions of the BCI and will work without BCI equipment.
The mode can be changed within the Unity project under the "Game Manager" script.

### BCI Installation Instructions:
To add BCI equipment, the following steps must be taken in the Unity project:
- Enable the "Open BCI" script
- Disable the "BCI sim" script
- Set the variable "Wizard of Oz" in the "BCI slider" script to false
