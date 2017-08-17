using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class Strip
{
    public Vector3 starStartPoints;
    public float stripLength;
    public int stripNumber;

    public Strip(Vector3 stripBasePoint, float length, int strip)
    {
        starStartPoints = stripBasePoint;
        stripLength = length;
        stripNumber = strip;
    }
}

public class StripPosition : MonoBehaviour {

    public GameObject prefab;
    private ArrayList starStrips;
    private static ArrayList starStarts;
    private static ArrayList stripLengths;
    private int setStripNumber = 0;
    
    private UnityEngine.Random random = new UnityEngine.Random();

    // empty game object as parent for strips
    private GameObject parent;
    public int starStripCount = 0;
    [HideInInspector]
    public int stripNumber;
    public bool clearStripArray = false;
    public static StripPosition thisStripPosition;

    // to test all LED strips spawning in sequence;
    [Header ("test strip conditions")]
    public int maxStripNumer = 10;
    public int testStripNumber = 0;

    Strip randomStrip; // instantiate random strip
    Strip lastRandomStrip;
    private Vector3 lastTriggerPosition = new Vector3(0, 0, 0);
    private Vector3 triggerPosition = new Vector3(1,0,0);

    //public Text stripCountText;


    void Awake ()
    {
        if (thisStripPosition)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            thisStripPosition = this;
        }
    }

    // Use this for initialization
    void Start() {

        parent = GameObject.FindGameObjectWithTag("STRIPS");

        // create and arraylist of Strip objects
        starStrips = new ArrayList();
        //Debug.Log("star strip array instantiated" + starStrips);

        starStrips.Add(new Strip(new Vector3(-1.022f, 2.5908f, -0.069f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(-0.94f, 2.5908f, 0.42f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(-0.49f, 2.5908f, -0.29f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(-0.46f, 2.5908f, 0.8f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(-0.05f, 2.4384f, -0.88f), 1f, 4));
        starStrips.Add(new Strip(new Vector3(0.13f, 2.5908f, 0.01f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(-0.09f, 2.5908f, 0.89f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(1.06f, 2.5908f, -0.92f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(0.97f, 2.5908f, 0.02f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(0.94f, 2.4384f, 0.84f), 1f, 9));
        starStrips.Add(new Strip(new Vector3(1.75f, 2.4384f, -1.19f), 1f, 10));
        starStrips.Add(new Strip(new Vector3(1.78f, 2.5908f, -0.47f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(1.67f, 2.5908f, 0.17f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(1.7f, 2.5908f, 0.5f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(1.62f, 2.5908f, 1.1f), 0.5f, 14));


        //oldStarStarts();

		// set a random starting strip position
        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);



    }

    void Update()
    {
       // clear the array of strip positions to repopulate;
       if (clearStripArray == true)
        {
            LogStripPositons();

            starStrips.Clear();
        //    stripCountText.text = "Clear";
            clearStripArray = false;
        }

        starStripCount = starStrips.Count;
    }

    public int Count()
    {
        return starStrips.Count;
    }

    public void SetStripPosition(Vector3 triggerPosition)
    {
  
       // lastTriggerPosition = triggerPosition;
        starStrips.Add(new Strip(triggerPosition, 0.5f, starStripCount));

        Debug.Log("setting strip number " + setStripNumber + " at position of: "+ triggerPosition);
      //  stripCountText.text = (starStripCount).ToString();
        starStripCount++;

        // GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

    }


    // return a randrom stip object , not position
    public Strip getRandomStrip()
    {
        
        randomStrip = FindRandomStrip();

		// make sure stars are not spawning too close
        if (Vector3.Distance(randomStrip.starStartPoints, lastRandomStrip.starStartPoints) < 1f)
        {
            FindRandomStrip();
        }
        lastRandomStrip = randomStrip;

        // test first strip
       //randomStrip = (Strip)starStrips.ToArray()[0];
      
        return randomStrip; 
        
    }

    private Strip FindRandomStrip()
    {
        int nextPosition = UnityEngine.Random.Range(0, (starStrips.Count - 1));
        randomStrip = GetStrip(nextPosition);
        return randomStrip;
    }

    public Strip GetStrip(int position)
    {
        if (position >= starStrips.Count)
        {
            position = 0;
        }
        return (Strip)starStrips.ToArray()[position];
    }

    // having a running strip number that counts up for testing 
    public Strip getTestStrip()
    {
        Strip testStrip = (Strip)starStrips.ToArray()[testStripNumber];
        if (testStripNumber < (starStripCount-1))
        {
            testStripNumber++;
        }
        else
        {
            testStripNumber = 0;
        }
        return testStrip;
    }

    private void LogStripPositons()
    {
        string msg = "        starStarts = new ArrayList();";
        for (int i =0; i<starStarts.Count;i++) 
        {
            Strip strip = (Strip)starStarts.ToArray()[i];
            msg = "        starStrips.Add(new Strip(new Vector3("+ strip.starStartPoints.x+ "f, "+ strip.starStartPoints.y+ "f, "+ strip.starStartPoints.z+ "f), 0.5f, "+i+"));";
        }
    }
    
}


