using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniOSC;


public class StarAnimations : MonoBehaviour {

    OSCSenderComplete oscSenderObject;
    OSCSenderAllCaught oscSenderCaughtObject;
    private SoundManager soundManager;

    bool allFull = false;


    // Use this for initialization
    void Start () {

        oscSenderObject = (OSCSenderComplete)FindObjectOfType<OSCSenderComplete>();
        oscSenderCaughtObject = (OSCSenderAllCaught)FindObjectOfType<OSCSenderAllCaught>();
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();

    }

    public void FullAnimation()
    {

        oscSenderObject.SendOSCCompleteMessage("/constellationfull", 0);
        SoundConstellationFull();

    }

    public void FullCaughtAnimation()
    {
        oscSenderCaughtObject.SendOSCAllCaughtMessage("/allcaught", 0);
        SoundConstellationFull();
    }


    private void SoundConstellationFull()
    {

        soundManager.PlayStateTransition();
        
    }
}

