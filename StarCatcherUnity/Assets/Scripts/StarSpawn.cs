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
    public float mediumTimeToSpawn = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardTimeToSpawn = 1.0f;

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
    private float[] LevelOnePercentages;
    private float[] LevelTwoPercentages;
    private float[] LevelThreePercentages;
    private float[][] LevelPercentages;




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

        LevelOnePercentages = new float[3] { 0.8f, 0.2f, 0f };
        LevelTwoPercentages = new float[3] { 0.5f, 0.4f, 0.1f };
        LevelThreePercentages = new float[3] { 0.2f, 0.4f, 0.4f };
        LevelPercentages = new float[3][] { LevelOnePercentages, LevelTwoPercentages, LevelThreePercentages };

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
            int currentLevel = Score.GetCurrentLevel();
            timeToNextSpawn = calcNextSpawnRate(LevelPercentages[currentLevel]);
            // what is next?
        }

    }

    private float calcNextSpawnRate(float[] levelPercentages)
    {
        float total = 0;

        foreach (float percentage in levelPercentages)
        {
            total += percentage;
        }

        float randomPoint = UnityEngine.Random.value * total;
        float randomOriginal = randomPoint;
        for (int i = 0; i < levelPercentages.Length; i++)
        {

            if (randomPoint < levelPercentages[i])
            {
                // sets the corresponding spawn rate float from array

                StarTypeToSpawn = StarType[i];
         
                Debug.Log("based on rand no " + randomOriginal + "Spawning " + StarTypeToSpawn + " stars, at a rate of " + SpawnRate[i]);
                return SpawnRate[i];
            } else
            {
                randomPoint -= levelPercentages[i];
            }
        }
        return levelPercentages.Length - 1; // ???
     
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
        if (StarTypeToSpawn == "easy")
        {
            SpawnEasy();
        }
        else if (StarTypeToSpawn == "medium")
        {
            SpawnMedium();
        } else
        {
            SpawnHard();
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


