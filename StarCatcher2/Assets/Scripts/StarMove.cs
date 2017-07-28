using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMove : MonoBehaviour {

    Vector3 startMarker;
    Vector3 endMarker;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startTime = Time.time;
        startMarker = transform.localPosition;
        endMarker = startMarker + new Vector3(0, -2.0f, 0);
        journeyLength = Vector3.Distance(startMarker, endMarker);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
    }
}
