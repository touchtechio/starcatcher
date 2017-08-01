using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMove : MonoBehaviour {

    Vector3 startMarker;
    Vector3 endMarker;
    Vector3 nearlyEndMarker;
    public float speed = 0F;
    private float startTime;
    private float journeyLength;
    bool timeRecorded = false;
    public float fallDuration = 0f;
    public Vector3 stripLength = new Vector3(0, 0.5f, 0);
    public Vector3 starDropScale =  new Vector3(1, 3.0f, 1);
    public float timeToDestroyStar = 0f;
    public float lingerTime = 2f;
    
    void Start()
    {
        // turn off trigger component so players can't catch stars at the start
        gameObject.GetComponent<SphereCollider>().isTrigger = false;
        startTime = Time.time;

        endMarker = transform.localPosition;
        startMarker = endMarker + Vector3.Scale(stripLength, starDropScale);
        journeyLength = Vector3.Distance(startMarker, endMarker);
        // set speed of fall according to fall time and distance travelled
        speed = journeyLength / fallDuration;

        // set time when the trigger component is turned on
        nearlyEndMarker = endMarker + new Vector3(0, +0.1f, 0);
        //nearlyEndMarker = startMarker + new Vector3(0, 0.9f * endMarker[1], 0);


        timeToDestroyStar = fallDuration + lingerTime;
    }

    void Update()
    {

        /*
        // adding accelaration
        if (transform.position != endMarker)
        {
            speed += .1f/2;
        }
     
        //when speed is variable, use this to calculate fall duration
        else if (transform.position == endMarker && !timeRecorded)
        {
            fallDuration = Time.time - startTime;
            Debug.Log("time the star took to fall " + fallDuration);
            timeRecorded = true;
        }
           */
        if (transform.position[1] <= nearlyEndMarker[1])
        {
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
            HU_Star starEffects = gameObject.GetComponent<HU_Star>();
            starEffects._coronaTrails = false;
            
        }
 
        StarCollider starCollider = gameObject.GetComponent<StarCollider>();
        if (starCollider.isStarCaught == false)
        {

            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);

            // set countdown to destroy star, if star is caught, this is halted
            timeToDestroyStar -= Time.deltaTime;
            if (timeToDestroyStar <= 0)
            {
                Destroy(gameObject);
            }
        }
        //Debug.Log(gameObject.GetComponent<SphereCollider>().isTrigger);


    }
}
