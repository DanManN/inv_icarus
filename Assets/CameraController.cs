﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
     public static List<Camera> cameras;
     private int currentCameraIndex;
     
     // Use this for initialization
     void Start () {
        var agm = GameObject.Find("Manager").GetComponent<AgentManager>();
        cameras = agm.cams;

         currentCameraIndex = 0;
         
         //Turn all cameras off, except the first default one
         for (int i=1; i<cameras.Count; i++) 
         {
             cameras[i].gameObject.SetActive(false);
         }
         
         //If any cameras were added to the controller, enable the first one
         if (cameras.Count>0)
         {
             cameras [0].gameObject.SetActive (true);
             Debug.Log ("Camera with name: " + cameras [0].GetComponent<Camera>().name + ", is now enabled");
         }
     }
     
     // Update is called once per frame
     void Update () {
         //If the c button is pressed, switch to the next camera
         //Set the camera at the current index to inactive, and set the next one in the array to active
         //When we reach the end of the camera array, move back to the beginning or the array.
         if (Input.GetKeyDown(KeyCode.C))
         {
             currentCameraIndex ++;
             Debug.Log ("C button has been pressed. Switching to the next camera");
             if (currentCameraIndex < cameras.Count)
             {
                 cameras[currentCameraIndex-1].gameObject.SetActive(false);
                 cameras[currentCameraIndex].gameObject.SetActive(true);
                 Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
             }
             else
             {
                 cameras[currentCameraIndex-1].gameObject.SetActive(false);
                 currentCameraIndex = 0;
                 cameras[currentCameraIndex].gameObject.SetActive(true);
                 Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
             }
         }
         // Main Camera
         if (Input.GetKeyDown(KeyCode.M))
         {
             Debug.Log ("M button has been pressed. Switching to Main Camera");
            cameras[currentCameraIndex].gameObject.SetActive(false);
            currentCameraIndex = 0;
            cameras[currentCameraIndex].gameObject.SetActive(true);
            Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");

         }
     }
 }