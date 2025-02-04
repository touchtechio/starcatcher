﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawn : MonoBehaviour {
    /*
    The Star Spawn class spawns different types of stars for the game, sends the osc message
    handles timing for random star spawning, sets star spawn probabilities for different world states
    destroys stars upon death
    */

    public GameObject prefab;
    public GameObject warmStar;
    public GameObject neutralStar;
    public GameObject coolStar;
    public StarSpawnBase starSpawnBase;

    private static ArrayList starStarts;
    public static System.Random random = new System.Random();

    //private static UnityEngine.Random random = new UnityEngine.Random();
    public StripPosition stripPositions;
    private float timeToNextSpawn = 0;
    private SoundManager soundManager;
    // empty game object as parent for spwned stars
    private string[] StarType = { "easy", "medium", "hard" };

    public float waitWaves = 20;
    public int ShowerCount = 10;
    public bool testSend = false;
    public bool randomStarsSending = true;
    [HideInInspector]
    public string StarTypeToSpawn;

    // set to durations between spawn events
    [Range(0.1f, 5.0f)]
    public float rejunvinateTimeToSpawnMin = 0f;
    [Range(0.1f, 5.0f)]
    public float rejunvinateTimeToSpawnMax = 1f;
    [Range(0.1f, 5.0f)]
    public float flourishTimeToSpawnMin = 0f;
    [Range(0.1f, 5.0f)]
    public float flourishTimeToSpawnMax = 2f;
    [Range(0.1f, 5.0f)]
    public float declineTimeToSpawnMin = 1.0f;
    [Range(0.1f, 5.0f)]
    public float declineTimeToSpawnMax = 3.0f;
    [Range(0.1f, 10.0f)]
    public float dyingTimeToSpawnMin = 1.5f;
    [Range(0.1f, 10.0f)]
    public float dyingTimeToSpawnMax = 5.0f;

    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyFallDuration = 5.0f;
    [Range(0.1f, 10.0f)]
    public float mediumFallDuration = 3.8f;
    [Range(0.1f, 10.0f)]
    public float hardFallDuration = 2.0f;


    // set to durations between spawn events
    [Range(0.1f, 10.0f)]
    public float easyLingerTime = 3.0f;
    [Range(0.1f, 10.0f)]
    public float mediumLingerTime = 2.0f;
    [Range(0.1f, 10.0f)]
    public float hardLingerTime = 1.0f;


    private float[] SpawnRate;
    private float[] flourishPercentages;
    private float[] declinePercentages;
    private float[] dyingPercentages;
    private float[] deadPercentages;
    private float[] rejuvinationPercentages;
    private float[][] LevelPercentages;
    private float[] flourishSpawnRates;
    private float[] declineSpawnRates;
    private float[] dyingSpawnRates;
    private float[] rejuvinateSpawnRates;

    Dictionary<Score.GameState, float[]> worldStatePercentages = new Dictionary<Score.GameState, float[]> ();
    Dictionary<Score.GameState, float[]> worldStateSpawnRates = new Dictionary<Score.GameState, float[]> ();
    private static float deadTimer;

    public ArrayList plasmaArray = new ArrayList();


    // Use this for initialization
    void Start () {

        //InvokeRepeating("Spawn", 0.5f, spawnRate);

        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        if (null == stripPositions)
        {
            Debug.Log("ERROR: no StripPosition found");
        }

        // StartCoroutine(SpawnShowers());
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();

        flourishPercentages = new float[3] { 0.8f, 0.2f, 0.05f };
        declinePercentages = new float[3] { 0.3f, 0.3f, 0.4f };
        dyingPercentages = new float[3] { 0.05f, 0.3f, 0.6f };
        // DeadPercentages = new float[3] {10f, 10f, 10f };
        rejuvinationPercentages = new float[3] {0.5f, 0.2f, 0.3f };

        worldStatePercentages.Add(Score.GameState.Flourishing, flourishPercentages);
        worldStatePercentages.Add(Score.GameState.Decline, declinePercentages);
        worldStatePercentages.Add(Score.GameState.Dying, dyingPercentages);
        // worldStatePercentages.Add(Score.GameState.Dead, DeadPercentages);
        worldStatePercentages.Add(Score.GameState.Rejuvination, rejuvinationPercentages);

        flourishSpawnRates = new float[2] {flourishTimeToSpawnMin, flourishTimeToSpawnMax};
        declineSpawnRates = new float[2] {declineTimeToSpawnMin, declineTimeToSpawnMax};
        dyingSpawnRates = new float[2] {dyingTimeToSpawnMin, dyingTimeToSpawnMax};
        rejuvinateSpawnRates = new float[2] {rejunvinateTimeToSpawnMin, rejunvinateTimeToSpawnMax};

        worldStateSpawnRates.Add(Score.GameState.Flourishing, flourishSpawnRates);
        worldStateSpawnRates.Add(Score.GameState.Decline, declineSpawnRates);
        worldStateSpawnRates.Add(Score.GameState.Dying, dyingSpawnRates);
        worldStateSpawnRates.Add(Score.GameState.Rejuvination, rejuvinateSpawnRates);

    }

    public void DestroyStars()
    {
        this.randomStarsSending = false;
        starSpawnBase.DestroyStars();
        return;
    }

    public void StartRandomStars()
    {
        //Debug.Log("Restarted random stars");
        this.randomStarsSending = true;
    }

    public void StopRandomStars()
    {
        // Debug.Log("Stopped random stars");
        this.randomStarsSending = false;
    }

    public void Update()
    {
        //SpawnRate = new float[3] { easyTimeToSpawn, mediumTimeToSpawn, hardTimeToSpawn }; // in case we are updating values at runtime
        worldStateSpawnRates[Score.GameState.Flourishing][0] = flourishTimeToSpawnMin;
        // burn down time since last update
        timeToNextSpawn -= Time.deltaTime;

        // send random stars. This was the original functionality
        if (randomStarsSending) {
            // if in Dead state, don't spawn stars
            if (timeToNextSpawn <= 0 && Score.plasmaWorldState != Score.GameState.Dead)
            {
                Spawn();
            }
        }


    }

    private float calcNextSpawnRate(float[] spawnRates)
    {
        float randomPoint = UnityEngine.Random.value;
        return spawnRates[0] + randomPoint*(spawnRates[1]-spawnRates[0]);
    }

    public void Spawn()
    {
        // get position of strip
        Strip strip = GetStrip();
        // Debug.Log("which strip" + strip.stripNumber);
        Spawn(strip);
    }

    // called by StarSpawnFormations for formation collisions directly
    public void Spawn(int stripId)
    {
        Strip strip = this.stripPositions.getStarPositions()[stripId] as Strip;
        this.Spawn(strip);
    }

    public void Spawn(Strip strip)
    {
       // if (Score.plasmaWorldState != Score.GameState.Dead) {
        timeToNextSpawn = calcNextSpawnRate(worldStateSpawnRates[Score.plasmaWorldState]);
        StarTypeToSpawn = calcNextSpawnType(worldStatePercentages[Score.plasmaWorldState]);
      //  }
        // if (!randomStarsSending) {
        //     Debug.Log("Star " + StarTypeToSpawn);
        // }

        // needs change to enum type and case switch statements
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

    private void SpawnHard(Strip strip)
    {
        starSpawnBase.Spawn(strip, hardFallDuration, hardLingerTime, UnityEngine.Random.ColorHSV(0.0f, 0.5f), coolStar, "cool");

    }

    private void SpawnMedium(Strip strip)
    {
        starSpawnBase.Spawn(strip, mediumFallDuration, mediumLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f), neutralStar, "neutral");
    }


    private void SpawnEasy(Strip strip)
    {
        starSpawnBase.Spawn(strip, easyFallDuration, easyLingerTime, UnityEngine.Random.ColorHSV(0.5f, 1.0f), warmStar, "warm");
    }


    private void SpawnTrail(Strip strip)
    {
        timeToNextSpawn = calcNextSpawnRate(worldStateSpawnRates[Score.plasmaWorldState]);
        StarTypeToSpawn = calcNextSpawnType(worldStatePercentages[Score.plasmaWorldState]);
        starSpawnBase.Spawn(strip, 250, 50, UnityEngine.Random.ColorHSV(0.5f, 1.0f), warmStar, "warm");
    }



    private Strip GetStrip()
    {
        if (testSend)
        {
            return stripPositions.getTestStrip();
        } else
		    return stripPositions.getRandomStrip();
    }

    private string calcNextSpawnType(float[] levelPercentagesCurrentLevel)
        {
        float total = 0;

        foreach (float percentage in levelPercentagesCurrentLevel)
        {
            total += percentage;
        }

        // roll dice to see which type of star is spawned
        float randomPoint = UnityEngine.Random.value * total;
        float randomOriginal = randomPoint;

        // check which type of star to spawn for current level passed in
        for (int i = 0; i < levelPercentagesCurrentLevel.Length; i++)
        {
            //Debug.Log("random point "+ randomPoint + "levelPercentagesCurrentLevel " + i + ": " + levelPercentagesCurrentLevel[i]);
            if (randomPoint < levelPercentagesCurrentLevel[i])
            {
                // sets the corresponding spawn rate float from array
                return StarType[i];
                //Debug.Log("based on rand no " + randomOriginal + "Spawning " + StarTypeToSpawn + " stars, at a rate of " + SpawnRate[i]);
            } else
            {
                randomPoint -= levelPercentagesCurrentLevel[i];
                
            }
        }
        return StarType[-1]; // return last type of star in list
    }

}


