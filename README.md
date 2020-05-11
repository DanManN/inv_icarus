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

Here is my plan B. Instead of "Navigation by click to move", we can implement something like **Autopilot**: user can have some controls, but in the mean time SF helps us with getting avoid of obstacle and other planes automatically.

We can design a UI Checkbox:

- [x] Using Social Force to autopilot

If it is not clicked, then the plane will not automatically get avoid of obstacles. If it hit obstacles / other planes, then it will explode

Default it is ON. We can show the difference of applying SF and not applying SF here.

### Design (checkbox means at least got a prototype, not means fully finished)

- [x] Plane: User control aircraft
- [x] Obstacle
  - [x] Planet: Stable (x,y,z)
  - [x] Space trash: Has random running path
    - [ ] Random running path? Not yet finished.
- [ ] SF Mode and Non-SF mode

#### Details and TODO 
- [ ] **[IMPORTANT] SF migration (rb.AddForce if plane in a perceptionRadius of the obstacle or other aircraft)**
- [ ] Plane collision and explode logic: Currently Daniel designed it as `if speed > threshold & crashed, explode.` Not yet changed.
- [ ] Explode camera need to be fixed. Currently an error pops out and I have not yet checked that
- [ ] Add more planets (Stable Obstacle) --> Do we want a random generation? 
- [ ] **Random path** of Space trash 
  - [x] Generate random obstacles in 3D space [Need to scale better]
    - [x] Advanced: Random object in camera view: https://answers.unity.com/questions/752253/spawn-object-at-random-points-within-camera-view.html
    - [ ] The random logic has not yet well written. 
    - [ ] Random choice of different prefab?
  - [x] Obstacle prefab: space trash [Now using Resource/prefab/Obstacle_mine1 as tmp]
  - [x] Collider:
    - [x] **[for SF]** Sphere collider for perceptionRadius detection (Is that a trigger? )
    - [x] Sphere collider for Hit and Explode (Not trigger)
- [x] Scrolling Background 3D? Use SkyBox!
  - [x] Center randomize obstacles about Main camera? 
  - [ ] Remove obstacles if they are out of camera?
