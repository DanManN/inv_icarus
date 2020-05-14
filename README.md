## Instructions

In this game, you (players) are suppose to control drones to do the eliminate task by controlling them to explode on space trash.

* Camera Control
  * Main Camera: WASD, QE and mouse scrolling. Press "M" to set main camera
  * Following Camera: Press "C" to switch cameras.
* Drone selection:
  * Press "Space" to select and deselect aircrafts when they are highlighted. 
* Navigation:
  * If we have drones selected, left click on a planet or space trash to navigate the drones to that place. 
  * Path Planning
    * We implemented a 3D RRT algorithm in order to plan the path of the drones.  
    * The path exapands randomly within the radius of the scene while avoiding obstacles
    * To speed up execution, with a 10% chance, the path will attempt to expand towards the goal destination
  * Applied forces to the drones to move them towards the desired points along the RRT path
   







