using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarSpawn : MonoBehaviour {

    public GameObject prefab;

    private static ArrayList starStarts;
    private static UnityEngine.Random random = new UnityEngine.Random();
    private SerialController starSpawnSerialController;
    private StripPosition stripPositions;
	public OSCSender oscSenderObject;
	private int stripNumber;

    // empty game object as parent for spwned stars
    static private GameObject parent;
    int starCount = 0;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyTimeToSpawn = 0;
    [Range(0.1f, 10.0f)]
    public float hardTimeToSpawn = 0;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyFallDuration = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardFallDuration = 1.0f;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyLingerTime = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardLingerTime = 1.0f;

    [Range(0.1f, 10.0f)]
    public float starDropYScale = 3.0f;


    private float easyTimeToNextSpawn = 0;
    private float hardTimeToNextSpawn = 0;

    bool betweenWaves = false;
    public float waitWaves = 20;
    public int ShowerCount = 10;

    // to test all LED strips spawning in sequence;
    public bool testSend = false;
    public int maxStripNumer = 10;
    int testStripNumber = 0;

    // Use this for initialization
    void Start () {

        parent = GameObject.FindGameObjectWithTag("STARS");
        //InvokeRepeating("Spawn", 0.5f, spawnRate);

        starSpawnSerialController = SerialController.FindObjectOfType<SerialController>();
        if (null == starSpawnSerialController)
        {
            Debug.Log("ERROR: no serial controller found");
        }

        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        if (stripPositions == starSpawnSerialController)
        {
            Debug.Log("ERROR: no StripPosition found");
        }

       // StartCoroutine(SpawnShowers());
      
    }

    public static void DestroyStars()
    {

        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        return;
    }


    public void Update()
    {
        
        // burn down time since last update
        easyTimeToNextSpawn -= Time.deltaTime;
        //hardTimeToNextSpawn -= Time.deltaTime;

        if (easyTimeToNextSpawn <= 0)
        {
            SpawnEasy();
            easyTimeToNextSpawn = easyTimeToSpawn;
        }

        // Debug.Log("spawned" + ShowerCount);
        //  betweenWaves = true;
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine("BetweenWaves");
        }

        
        if (hardTimeToNextSpawn <= 0)
        {
           // SpawnHard();
            hardTimeToNextSpawn = hardTimeToSpawn; ;
        }
        


    }

    IEnumerator BetweenWaves()
    {
        
        yield return new WaitForSeconds(waitWaves);
        Debug.Log("between showers"+ Time.time);
        betweenWaves = false;
    }
    
    private void SpawnHard()
    {
        Spawn(hardFallDuration, hardLingerTime, UnityEngine.Random.ColorHSV(0.0f, 0.5f));

    }

    
    private void SpawnEasy()
    {
        Spawn(easyFallDuration, easyLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f));
    }


    public void Spawn()
    {

        SpawnEasy();

    }


    private void Spawn(float duration, float lingerTime, Color color)
    {


        // get position
        Strip strip = GetStrip();
        Vector3 spawnPoint = strip.starStartPoints;

        // make star
        GameObject star = Instantiate(prefab, spawnPoint, Quaternion.identity) as GameObject;

        // parent to STARS 
        star.transform.parent = parent.transform;
        starCount++;
        string starNumber = starCount.ToString();
        star.name = "star-" + starNumber;

        // configure Star Mover
        StarMove mover = star.GetComponent<StarMove>();
        //mover.speed = speed;
        mover.stripLength = new Vector3(0, strip.stripLength, 0);
        mover.fallDuration = duration;
        mover.lingerTime = lingerTime;
        mover.starDropScale = new Vector3(1, starDropYScale, 1);


        // create unique Star Gen config
        HU_Star starComponent = star.GetComponent<HU_Star>();
        starComponent.MakeUniqueRunTime();

        // change a color
        starComponent._color = color;
        starComponent._color2 = UnityEngine.Random.ColorHSV(0.0f, 1.0f);

		// send a string through serial everytime a star is spawned
        starNumber = "a";
        starSpawnSerialController.SendSerialMessage(starNumber);

        // send an OSC message with argurments for strip number, durationg and linger time

        if (testSend)
        {
            oscSenderObject.SendOSCStarMessage("/starfall", testStripNumber, (int)duration * 1000);
            if (testStripNumber < maxStripNumer-1)
            {
                testStripNumber++;
            }
            else
                testStripNumber = 0;
        }
        else
        {
            oscSenderObject.SendOSCStarMessage("/starfall", stripPositions.stripNumber, (int)(duration * 1000));
        }
    
        Debug.Log(star.name + " fell");

        return;
    }



    private Strip GetStrip()
    {
		return stripPositions.getRandomStrip ();
    }


}


