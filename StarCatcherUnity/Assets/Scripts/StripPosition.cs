using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strip
{
    public Vector3 starStartPoints;
    public float stripLength;

    public Strip(Vector3 stripBasePoint, float length)
    {
        starStartPoints = stripBasePoint;
        stripLength = length;
    }
}

public class StripPosition : MonoBehaviour {

    public GameObject prefab;
    private ArrayList starStrips;
    private static ArrayList starStarts;
    private static ArrayList stripLengths;
    
    private UnityEngine.Random random = new UnityEngine.Random();

    // empty game object as parent for strips
    private GameObject parent;
    int starStripCount;

    Strip randomStrip; // instantiate random strip
    Strip lastRandomStrip;

    // Use this for initialization
    void Start() {

        parent = GameObject.FindGameObjectWithTag("STRIPS");

        // create and arraylist of Strip objects
        starStrips = new ArrayList();
        //Debug.Log("star strip array instantiated" + starStrips);

        starStrips.Add(new Strip(new Vector3(-1.022f, 2.5908f, -0.069f), 0.5f));
        starStrips.Add(new Strip(new Vector3(-0.94f, 2.5908f, 0.42f), 0.5f));
        starStrips.Add(new Strip(new Vector3(-0.49f, 2.5908f, -0.29f), 0.5f));
        starStrips.Add(new Strip(new Vector3(-0.46f, 2.5908f, 0.8f), 0.5f));
        starStrips.Add(new Strip(new Vector3(-0.05f, 2.4384f, -0.88f), 1f));
        starStrips.Add(new Strip(new Vector3(0.13f, 2.5908f, 0.01f), 0.5f));
        starStrips.Add(new Strip(new Vector3(-0.09f, 2.5908f, 0.89f), 0.5f));
        starStrips.Add(new Strip(new Vector3(1.06f, 2.5908f, -0.92f), 0.5f));
        starStrips.Add(new Strip(new Vector3(0.97f, 2.5908f, 0.02f), 0.5f));
        starStrips.Add(new Strip(new Vector3(0.94f, 2.4384f, 0.84f), 1f));
        starStrips.Add(new Strip(new Vector3(1.75f, 2.4384f, -1.19f), 1f));
        starStrips.Add(new Strip(new Vector3(1.78f, 2.5908f, -0.47f), 0.5f));
        starStrips.Add(new Strip(new Vector3(1.67f, 2.5908f, 0.17f), 0.5f));
        starStrips.Add(new Strip(new Vector3(1.7f, 2.5908f, 0.5f), 0.5f));
        starStrips.Add(new Strip(new Vector3(1.62f, 2.5908f, 1.1f), 0.5f));

        //oldStarStarts();
        starStripCount = starStrips.Count;
        lastRandomStrip = new Strip(new Vector3(10f, 10f, 10f), 0.5f);
    }

    public Strip getRandomStrip()
    {
        int startPosition = UnityEngine.Random.Range(0, starStripCount);
        randomStrip = (Strip)starStrips.ToArray()[startPosition];
        while (Vector3.Distance(randomStrip.starStartPoints, lastRandomStrip.starStartPoints) < 0.5f)
        {
            GenerateNewStrip();
        }
        lastRandomStrip = randomStrip;
        return randomStrip; 
    }

    private void GenerateNewStrip()
    {
        Debug.Log("too close");
        int nextPosition = UnityEngine.Random.Range(0, starStripCount);
        randomStrip = (Strip)starStrips.ToArray()[nextPosition];
       // lastRandomStrip = randomStrip;
        return;
    }

    private void oldStarStarts()
    {

        starStarts = new ArrayList();
        starStarts.Add(new Vector3(-1.022f, 2.048f, -0.069f));
        starStarts.Add(new Vector3(-0.94f, 2.048f, 0.42f));
        starStarts.Add(new Vector3(-0.49f, 2.048f, -0.29f));
        starStarts.Add(new Vector3(-0.46f, 2.048f, 0.8f));
        starStarts.Add(new Vector3(-0.05f, 2.048f, -0.88f));
        starStarts.Add(new Vector3(0.13f, 2.048f, 0.01f));
        starStarts.Add(new Vector3(-0.09f, 2.048f, 0.89f));
        starStarts.Add(new Vector3(1.06f, 2.048f, -0.92f));
        starStarts.Add(new Vector3(0.97f, 2.048f, 0.02f));
        starStarts.Add(new Vector3(0.94f, 2.048f, 0.84f));
        starStarts.Add(new Vector3(1.75f, 2.048f, -1.19f));
        starStarts.Add(new Vector3(1.78f, 2.048f, -0.47f));
        starStarts.Add(new Vector3(1.67f, 2.048f, 0.17f));
        starStarts.Add(new Vector3(1.7f, 2.048f, 0.5f));
        starStarts.Add(new Vector3(1.62f, 2.048f, 1.1f));

        stripLengths = new ArrayList();
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(1f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(1f);
        stripLengths.Add(1f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);
        stripLengths.Add(0.5f);

        for (int i = 0; i < starStarts.Count; i++) {
            Strip starStrip = new Strip((Vector3)starStarts.ToArray()[i], (float)stripLengths.ToArray()[i]);
        }
       
    }

    public void Update()
    {

       

    }




    


}


