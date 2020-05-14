## Instructions

Demo Link: https://drive.google.com/file/d/1r16sIdf3OTkVps3sYQgm9DgTRCm9cVA2/view?usp=sharing

In this game, you (players) are suppose to control drones to navigate to a destination in the 3D space.

* Camera Control
  * Main Camera: WASD, QE and mouse scrolling. Press "M" to set main camera
  * Following Camera: Press "C" to switch cameras.
* Drone selection:
  * Press "Space" to select and deselect aircrafts when they are highlighted. 



### Navigation:

* If we have drones selected, left click on a planet or space trash to navigate the drones to that place. 
* Path Planning
  * We implemented a 3D RRT algorithm in order to plan the path of the drones.  
  * The path exapands randomly within the radius of the scene while avoiding obstacles
  * To speed up execution, with a 10% chance, the path will attempt to expand towards the goal destination
* Applied torques and thrust to the drones to move them towards the desired points along the RRT path
  * Three main torques are applied to steer the ships
    1. point towards the next rrt node or perpendicular to it depending on the current velocity.
    2. rotate away from obstacles if they are in the vision cone
    3. rotate away from agents if they are in the vision cone
  * Thurst is applied in local z direction (positive or negative) with intensity depending on the angle so as to optimize momentum towards the goal and minimize momentum away from goal.






