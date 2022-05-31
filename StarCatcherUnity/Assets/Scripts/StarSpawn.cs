using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarSpawn : MonoBehaviour {

    public GameObject prefab;
    public GameObject warmStar;
    public GameObject neutralStar;
    public GameObject coolStar;

    private static ArrayList starStarts;
    public static System.Random random = new System.Random();

    //private static UnityEngine.Random random = new UnityEngine.Random();
    public StripPosition stripPositions;
    private float timeToNextSpawn = 0;
    private float hardTimeToNextSpawn = 0;
    private int stripNumber;
    private SoundManager soundManager;
    bool betweenWaves = false;
    // empty game object as parent for spwned stars
    static private GameObject parent;
    int starCount = 0;
    private string[] StarType = { "easy", "medium", "hard" };


    public OSCSenderSpawn oscSenderObject;
    public float waitWaves = 20;
    public int ShowerCount = 10;
    public bool testSend = false;
    [HideInInspector]
    public string StarTypeToSpawn;

    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyTimeToSpawn = 4.0f;
    [Range(0.1f, 10.0f)]
    public float mediumTimeToSpawn = 6.0f;
    [Range(0.1f, 10.0f)]
    public float hardTimeToSpawn = 10.0f;

    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyFallDuration = 5.0f;
    [Range(0.1f, 10.0f)]
    public float mediumFallDuration = 3.0f;
    [Range(0.1f, 10.0f)]
    public float hardFallDuration = 1.0f;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyLingerTime = 3.0f;
    [Range(0.1f, 10.0f)]
    public float mediumLingerTime = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardLingerTime = 1.0f;

    [Range(0.1f, 10.0f)]
    public float starDropYScale = 3.0f;


    private float[] SpawnRate;
    private float[] FlourishPercentages;
    private float[] DeclinePercentages;
    private float[] DyingPercentages;
    private float[] DeadPercentages;
    private float[][] LevelPercentages;

    Dictionary<Score.GameState, float[]> worldStatePercentages = new Dictionary<Score.GameState, float[]> ();


    // Use this for initialization
    void Start () {
        easyTimeToSpawn = 1.5f;
        mediumTimeToSpawn = 3.0f;
        hardTimeToSpawn = 8.0f;

        // set to durations between spawn events
        easyFallDuration = 5.0f;
        mediumFallDuration = 3.0f;
        hardFallDuration = 1.0f;


        // set to durations between spawn events
        easyLingerTime = 3.0f;
        mediumLingerTime = 2.0f;
        hardLingerTime = 1.5f;

        parent = GameObject.FindGameObjectWithTag("STARS");
        //InvokeRepeating("Spawn", 0.5f, spawnRate);

        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        if (null == stripPositions)
        {
            Debug.Log("ERROR: no StripPosition found");
        }

        // StartCoroutine(SpawnShowers());
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();

        FlourishPercentages = new float[3] { 0.8f, 0.2f, 0f };
        DeclinePercentages = new float[3] { 0.4f, 0.4f, 0.2f };
        DyingPercentages = new float[3] { 0f, 0.2f, 0.8f };
        DeadPercentages = new float[3] { 0f, 0f, 0f };
        // LevelPercentages = new float[4][] { FlourishPercentages, DeclinePercentages, DyingPercentages, DeadPercentages};


        worldStatePercentages.Add(Score.GameState.Flourishing, FlourishPercentages);
        worldStatePercentages.Add(Score.GameState.Decline, DeclinePercentages);
        worldStatePercentages.Add(Score.GameState.Dying, DyingPercentages);
        worldStatePercentages.Add(Score.GameState.Dead, DeadPercentages);

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
        SpawnRate = new float[3] { easyTimeToSpawn, mediumTimeToSpawn, hardTimeToSpawn }; // in case we are updating values at runtime
        // burn down time since last update
        timeToNextSpawn -= Time.deltaTime;
        //hardTimeToNextSpawn -= Time.deltaTime;

        if (timeToNextSpawn <= 0)
        {
            Spawn();

            // what is next?
        }

    }

    private float calcNextSpawnRate(float[] levelPercentagesCurrentLevel)
    {
        float total = 0;

        foreach (float percentage in levelPercentagesCurrentLevel)
        {
            total += percentage;
        }

        float randomPoint = UnityEngine.Random.value * total;
        float randomOriginal = randomPoint;
        // check which type of star to spawn for current level passed in
        for (int i = 0; i < levelPercentagesCurrentLevel.Length; i++)
        {
            if (randomPoint < levelPercentagesCurrentLevel[i])
            {
                // sets the corresponding spawn rate float from array

                StarTypeToSpawn = StarType[i];
         
              //  Debug.Log("based on rand no " + randomOriginal + "Spawning " + StarTypeToSpawn + " stars, at a rate of " + SpawnRate[i]);
                return SpawnRate[i];
            } else
            {
                randomPoint -= levelPercentagesCurrentLevel[i];
            }
        }
        return levelPercentagesCurrentLevel.Length - 1; // ???
     
    }

    private void SpawnHard(Strip strip)
    {
        Spawn(strip, hardFallDuration, hardLingerTime, UnityEngine.Random.ColorHSV(0.0f, 0.5f));

    }

    private void SpawnMedium(Strip strip)
    {
        Spawn(strip, mediumFallDuration, mediumLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f));
    }


    private void SpawnEasy(Strip strip)
    {
        Spawn(strip, easyFallDuration, easyLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f));
    }

    public void Spawn()
    {
        // get position of strip
        Strip strip = GetStrip();
        Spawn(strip);
    }


    public void Spawn(Strip strip)
    {
        timeToNextSpawn = calcNextSpawnRate(worldStatePercentages[Score.plasmaWorldState]);

        if (StarTypeToSpawn == "easy")
        {
            SpawnEasy(strip);
        }
        else if (StarTypeToSpawn == "medium")
        {
            SpawnMedium(strip);
        } else
        {
            SpawnHard(strip);
        }

    }


    private void Spawn(Strip strip, float duration, float lingerTime, Color color)
    {

        Vector3 spawnPoint = strip.starStartPoints;

        // make star
        GameObject star = Instantiate(prefab, spawnPoint, Quaternion.identity) as GameObject;

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


