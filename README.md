## Manual Controls

* WS for pitching forward and back.
* AD for rolling left and right.
* Hold Space for thrust.
* Hold LeftControl for flaps.
* Arrow keys for camera tilt and pan.



## Plan 

1. Put two planes in a scene, migrate social forces codes, use SF to navigate the planes to a **goal position**
2. Add Obstacles
3. Add more planes



## Plans_todo

- [ ] Two planes in a scene, guide to a <u>desired position</u> 
  - [ ] Desired Position
    - [ ] **Random Vec3 in a scale?**
    - [ ] Generate random cubes in the scene and hit-raycast?
  - [ ] **Navigation? Social Forces** here? Avoid collision (planes and obstacles)
  - [ ] Forces?
    - [ ] gravity? For convenience remove it first?
    - [ ] aerodynamic force?
    - [ ] Apply behavior forces only on the direction they are facing
      - [ ] Migrate the "space" behavior to plane engine?
- [ ] Obstacles in 3D spaces and test