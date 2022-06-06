using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniOSC;


public class StarAnimations : MonoBehaviour {

    OSCSenderComplete oscSenderObject;
    private SoundManager soundManager;

    bool allFull = false;


    // Use this for initialization
    void Start () {

        oscSenderObject = (OSCSenderComplete)FindObjectOfType<OSCSenderComplete>();
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();

    }

    public void FullAnimation()
    {

        oscSenderObject.SendOSCCompleteMessage("/constellationfull", 0);
        SoundConstellationFull();

    }

    private void SoundConstellationFull()
    {
        if (null != soundManager.constellationFull)
        {
            AudioSource.PlayClipAtPoint(soundManager.constellationFull, gameObject.transform.position);
        }
    }

}

