using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarMove : MonoBehaviour {

    public float speed = 0F;
    public float fallDuration = 0f;
    public Vector3 stripLength = new Vector3(0, 0.5f, 0);
    public Vector3 starDropScale = new Vector3(1, 3.0f, 1);
    public float timeToDestroyStar;
    [HideInInspector]
    public float lingerTime = 2f;

    public int StripNumber = 0;

    Vector3 startMarker;
    Vector3 endMarker;
    Vector3 nearlyEndMarker;
    private float startTime;
    private float journeyLength;
    bool timeRecorded = false;
    private bool lingerSent = false;
    OSCSenderLinger oscSenderObject;
    private AudioSource source;
    public AudioClip starFall;
    //private StripPosition stripPosition;
    
    
    void Awake()
    {

    }
    void Start()
    {
        oscSenderObject = (OSCSenderLinger)FindObjectOfType<OSCSenderLinger>();
        source = GetComponent<AudioSource>();
        //stripPosition = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StripPosition>();

        // turn off trigger component so players can't catch stars at the start
        //gameObject.GetComponent<SphereCollider>().isTrigger = false;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        startTime = Time.time;
        // Debug.Log("strip number " + StripNumber + " ,star spawned at " + startTime + " ,fall for " + fallDuration);
        endMarker = transform.localPosition;
        startMarker = endMarker + Vector3.Scale(stripLength, starDropScale);
        journeyLength = Vector3.Distance(startMarker, endMarker);
        // set speed of fall according to fall time and distance travelled
        speed = journeyLength / fallDuration;

        timeToDestroyStar = fallDuration + lingerTime;

        SoundFall();
    }

    private void SoundFall()
    {
        if (null != starFall)
        {
            source.clip = starFall;
            source.pitch = source.clip.length * 1 / fallDuration;
            source.Play();
        }
    }

    void LateUpdate()
    {

        /*
        //when speed is variable, use this to calculate fall duration
        else if (transform.position == endMarker && !timeRecorded)
        {
            fallDuration = Time.time - startTime;
            Debug.Log("time the star took to fall " + fallDuration);
            timeRecorded = true;
        }
           */
        
        // check if star nearly reaches bottom point of fall to turn off trails and allow star to be able to be caught
        if (Time.time >= startTime + fallDuration * 0.95)
        {
           // gameObject.GetComponent<SphereCollider>().isTrigger = true;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
            HU_Star starEffects = gameObject.GetComponent<HU_Star>();
            starEffects._coronaTrails = false;
            
        }

       

        StarCollider starCollider = gameObject.GetComponent<StarCollider>();
        starCollider.caughtStripNumber = StripNumber;

        if (starCollider.isStarCaught == false)
        {
    
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);


            // when star reaches bottom point of fall, send linger time OSC message once
            if ((Time.time >= (startTime + fallDuration)) && !lingerSent)
            {
                // Debug.Log("strip number " + StripNumber + " ,start stamp " + startTime + " ,fall duration "+ fallDuration + " ,linger for " +lingerTime);

                oscSenderObject.SendOSCLingerMessage("/starlinger", StripNumber, (int)(lingerTime * 1000));
                lingerSent = true;
            }

            // set countdown to destroy star starting from spawn time, if star is caught, this is halted
            timeToDestroyStar -= Time.deltaTime;
            if (timeToDestroyStar <= 0)
            {
                Destroy(gameObject);
            }
        } else
        {
            // if caught stop playing audio
            source.Stop();
        }
        //Debug.Log(gameObject.GetComponent<SphereCollider>().isTrigger);


    }
}
