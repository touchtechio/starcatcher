using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class ReturnStarDestroyer : MonoBehaviour {

    public float timeToDestroyStar;

    private float startTime;

    
    void Start()
    {
       
        startTime = Time.time;

    }


    void LateUpdate()
    {


        // set countdown to destroy star starting from spawn time, if star is caught, this is halted
        timeToDestroyStar -= Time.deltaTime;
        if (timeToDestroyStar <= 0)
        {
            Destroy(gameObject);
        }

    }
        
}
