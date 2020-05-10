## Manual Controls

* WS for pitching forward and back.
* AD for rolling left and right.
* Hold Space for thrust.
* Hold LeftControl for flaps.
* Arrow keys for camera tilt and pan.



## ~~Plan~~ 

1. ~~Put one planes in a scene, migrate social forces codes, use SF to navigate the planes to a **goal position**~~
2. ~~Add more planes. Do the same thing~~
3. ~~Add Obstacles. Do the same thing~~
4. ~~Implement other things show up in proposal~~

## ~~Plans_todo~~

- [ ] ~~Two planes in a scene, guide to a <u>desired position</u>~~ 
  - [ ] ~~Desired Position~~
    - [ ] ~~**Random Vec3 in a scale?**~~
    - [ ] ~~Generate random cubes in the scene and hit-raycast?~~
  - [ ] ~~**Navigation? Social Forces** here? Avoid collision (planes and obstacles)~~
    - [ ] ~~Still need NavMesh? https://answers.unity.com/questions/1471149/how-to-use-navmeshagent-for-space-ship-in-3d.html~~
  - [ ] ~~Forces?~~
    - [ ] ~~gravity? For convenience remove it first?~~
    - [ ] ~~aerodynamic force?~~
    - [ ] ~~Apply behavior forces only on the direction they are facing~~
      - [ ] ~~Migrate the "space" behavior to plane engine?~~
- [ ] ~~Obstacles in 3D spaces and test~~

## Plan B

After tried out the navigation part, I feel like it might be too complex for us to generate a 3D NavMesh or write a new engine to make it works (As is stated in https://answers.unity.com/questions/1471149/how-to-use-navmeshagent-for-space-ship-in-3d.html)

Here is my plan B. Instead of "Navigation by click to move", we can implement something like Autopilot: user can have some controls, but in the mean time SF helps us with getting avoid of obstacle and other planes automatically.

#### TODO 

- [x] Plane control [Daniel has worked it out]
- [ ] SF migration (rb.AddForce if plane in a perceptionRadius of the obstacle or other aircraft)
- [ ] Add obstacles and other aircrafts

