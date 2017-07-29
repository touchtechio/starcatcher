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
    float fallTime = 0f;
    void Start()
    {
        startTime = Time.time;
        startMarker = transform.localPosition;
        endMarker = startMarker + new Vector3(0, -3.0f, 0);
        //nearlyEndMarker = startMarker + new Vector3(0, 0.9f * endMarker[1], 0);
        nearlyEndMarker = startMarker + new Vector3(0, -2.5f, 0);
        journeyLength = Vector3.Distance(startMarker, endMarker);
        gameObject.GetComponent<SphereCollider>().isTrigger = false;
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);

        // adding accelaration
        if (transform.position != endMarker)
        {
            speed += .1f/2;
        }
        else if (transform.position == endMarker && !timeRecorded)
        {
            fallTime = Time.time - startTime;
            Debug.Log("time the star took to fall " + fallTime);
            timeRecorded = true;
        }
         if (transform.position[1] <= nearlyEndMarker[1])
        {
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
            
        }
        Destroy(gameObject, fallTime + 5.0f);
        //Debug.Log(gameObject.GetComponent<SphereCollider>().isTrigger);


    }
}
