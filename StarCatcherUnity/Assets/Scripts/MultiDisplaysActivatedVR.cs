using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiDisplaysActivatedVR : MonoBehaviour
{
    public Camera projectorLoadCamera;
    static bool display1 = false;
    static bool display2 = false;


    void Start()
    {
        // GUI is rendered with last camera.
        // As we want it to end up in the main screen, make sure main camera is the last one drawn.
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
    
        projectorLoadCamera.enabled = true;

        if (Display.displays.Length > 1 && display1 == false)
        {
            Display.displays[1].Activate();
            display1 = true;
        }
        if (Display.displays.Length > 2 && display2 == false)
        {
            Display.displays[2].Activate();
            display2 = true;
        }
        if (Display.displays.Length > 3)
            Display.displays[3].Activate();
        


     }
    
}


