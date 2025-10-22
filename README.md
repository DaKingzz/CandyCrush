# CandyCrush King Version
This game project is meant to simulate the original mobile game Candy Crush Saga for educational purposes. This game uses Unity for the task. This is an assignment for the class of COMP376 - Introduction to Game Development.

## To Run
* Install Unity 2022.3.6f2 (With the new vital security patch) <br>
* Once opened, set the start up order of the Unity project to Boot->MainMenu->Game
* Open Boot Scene <br>
* Press on Play (ESC to Quit Game while on Main Menu)

## Characteristics/Behaviours of the Game
* No reshuffling due to its highly unlikely nature (Worst case scenario, one can always restart the level) <br>
* The Pause menu has a _restart/replay_ button besides _Resume_ and _Return to Menu_<br>
* During cascading, there is a fixed mulptiplier of _x2_, 100pts per regular match (no cascading) <br>
* Candy matches are not limited to 3, could be four or five and so on in one direction <br>
* Algorithm calculates and generates new candies after clearing them, applying gravity <br>
* The level 1 & 2 algorithms were applied on the tiles that drop from above using each level's rules of probabilities <br>
* "L" and "+" events are merged and cleared as a whole match during gameplay (counts as one match though) <br>
- **On an important note:** Clean and Structured project structure was applied to this project based on collective organizational standards <br>

Due to the somewhat _simple_ nature of this game, no undirected graph will be provided. I am not even sure I understand what exactly wants to be shown from that.

## Inspiration/Sources
* The Official's Candy Crush candies and similar candy-like UI assets
* Unity's Official Documentation
* Online forums (exchanges on how to apply certain game mechanics, e.g. Unity Discussions)
* AI to provide insight on problems and learn more about Unity's functionalities and rules
  
## Author
Arturo Sanchez E - 40283236
