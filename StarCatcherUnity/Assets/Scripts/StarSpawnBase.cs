using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarSpawnBase : MonoBehaviour
{
    public OSCSenderSpawn oscSenderObject;
    public OSCSenderWarmSpawn oscSenderWarmObject;
    public OSCSenderCoolSpawn oscSenderCoolObject;
    [Range(0.1f, 10.0f)]
    public float starDropYScale = 2.0f;
    public static GameObject parent;
    int starCount = 0;
    public GameObject warmStar;
    public float TutorialFallDuration;
    public float TutorialLingerTime;
    public StripPosition stripPositions;


    // Start is called before the first frame update
    void Start()
    {
        parent = GameObject.FindGameObjectWithTag("STARS");
        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        TutorialFallDuration = 3f;
        TutorialLingerTime = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // this function is specifically to spawn warm stars, in the tutorial scene
    public void SpawnStrip(int stripId)
    {
        Strip strip = stripPositions.getStarPositions()[stripId] as Strip;
        Spawn(strip, TutorialFallDuration, TutorialLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f), warmStar, "warm");
    }

    public void Spawn(Strip strip, float duration, float lingerTime, Color color, GameObject starType, string starColor)
    {
        // if (!randomStarsSending) {
        //     Debug.Log("instantiate star");
        // }

        Vector3 spawnPoint = strip.starStartPoints;

        // make star
        GameObject star = Instantiate(starType, spawnPoint, Quaternion.identity) as GameObject;

        // parent to STARS 
        star.transform.parent = parent.transform;
        starCount++;
        string starNumber = starCount.ToString();


        // configure Star Mover
        StarMove mover = star.GetComponent<StarMove>();
        //mover.speed = speed;
        mover.stripLength = new Vector3(0, strip.stripLength, 0);
        mover.fallDuration = duration;
        mover.lingerTime = lingerTime;
        mover.starDropScale = new Vector3(1, starDropYScale, 1);
        mover.StripNumber = strip.stripNumber;


        // send an OSC message with argurments for strip number, durationg and linger time

        if (starColor == "warm") {
            oscSenderWarmObject.SendOSCWarmStarMessage("/starwarm", strip.stripNumber, (int)(duration*1000));
        } else if (starColor == "neutral") {
            oscSenderObject.SendOSCStarMessage("/starfall", strip.stripNumber, (int)(duration*1000));
        } else if (starColor == "cool") {
            oscSenderCoolObject.SendOSCCoolStarMessage("/starcool", strip.stripNumber, (int)(duration*1000));
        }

      //  soundManager.GetComponent<AudioSource>().pitch = duration / 1 * soundManager.starFall.length;
      //  AudioSource.PlayClipAtPoint(soundManager.starFall, spawnPoint);

        return;
    }

        public void DestroyStars()
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        return;
    }
}
