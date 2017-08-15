using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarSpawn : MonoBehaviour {

    public GameObject prefab;

    private static ArrayList starStarts;
    private static UnityEngine.Random random = new UnityEngine.Random();
    private StripPosition stripPositions;
	public OSCSenderSpawn oscSenderObject;
	private int stripNumber;
    private SoundManager soundManager;

    // empty game object as parent for spwned stars
    static private GameObject parent;
    int starCount = 0;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyTimeToSpawn = 0;
    public float mediumTimeToSpawn = 0;
    [Range(0.1f, 10.0f)]
    public float hardTimeToSpawn = 0;

    private float[] SpawnRate;
    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyFallDuration = 5.0f;
    public float mediumFallDuration = 3.0f;
    [Range(0.1f, 10.0f)]
    public float hardFallDuration = 1.0f;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyLingerTime = 3.0f;
    public float mediumLingerTime = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardLingerTime = 1.0f;

    [Range(0.1f, 10.0f)]
    public float starDropYScale = 3.0f;


    private float timeToNextSpawn = 0;
    private float hardTimeToNextSpawn = 0;

    bool betweenWaves = false;
    public float waitWaves = 20;
    public int ShowerCount = 10;
    public bool testSend = false;



    // Use this for initialization
    void Start () {

        parent = GameObject.FindGameObjectWithTag("STARS");
        //InvokeRepeating("Spawn", 0.5f, spawnRate);

        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        if (null == stripPositions)
        {
            Debug.Log("ERROR: no StripPosition found");
        }

        // StartCoroutine(SpawnShowers());
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();
      
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
        timeToNextSpawn -= Time.deltaTime;
        //hardTimeToNextSpawn -= Time.deltaTime;

        if (timeToNextSpawn <= 0)
        {
            Spawn();
            timeToNextSpawn = calcNextSpawnRate();

            // what is next?
        }

        // Debug.Log("spawned" + ShowerCount);
        //  betweenWaves = true;
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine("BetweenWaves");
        }

       

    }

    private float calcNextSpawnRate()
    {

        if (Score.LEVEL_ONE == Score.GetCurrentLevel())
        {
            // do probability calc
        }


            throw new NotImplementedException();
        // sets the corresponding spawn rate float from array
        return SpawnRate[i];
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

    private void SpawnMedium()
    {
        Spawn(mediumFallDuration, mediumLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f));
    }


    private void SpawnEasy()
    {
        Spawn(easyFallDuration, easyLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f));
    }


    public void Spawn()
    {
        if (easy)
        {
            SpawnEasy();
        }

    }


    private void Spawn(float duration, float lingerTime, Color color)
    {

        // get position of strip
        Strip strip = GetStrip();
        Vector3 spawnPoint = strip.starStartPoints;

        // make star
        GameObject star = Instantiate(prefab, spawnPoint, Quaternion.identity) as GameObject;

        // parent to STARS 
        star.transform.parent = parent.transform;
        starCount++;
        string starNumber = starCount.ToString();
        star.name = "star-" + strip.stripNumber;


        // configure Star Mover
        StarMove mover = star.GetComponent<StarMove>();
        //mover.speed = speed;
        mover.stripLength = new Vector3(0, strip.stripLength, 0);
        mover.fallDuration = duration;
        mover.lingerTime = lingerTime;
        mover.starDropScale = new Vector3(1, starDropYScale, 1);
        mover.StripNumber = strip.stripNumber;

        // create unique Star Gen config
        HU_Star starComponent = star.GetComponent<HU_Star>();
        starComponent.MakeUniqueRunTime();

        // change a color
        starComponent._color = color;
        starComponent._color2 = UnityEngine.Random.ColorHSV(0.0f, 1.0f);

        // send an OSC message with argurments for strip number, durationg and linger time
        oscSenderObject.SendOSCStarMessage("/starfall", strip.stripNumber, (int)(duration * 1000));
    
      //  soundManager.GetComponent<AudioSource>().pitch = duration / 1 * soundManager.starFall.length;
      //  AudioSource.PlayClipAtPoint(soundManager.starFall, spawnPoint);

        return;
    }



    private Strip GetStrip()
    {
        if (testSend)
        {
            return stripPositions.getTestStrip();
        } else
		    return stripPositions.getRandomStrip();
    }


}


