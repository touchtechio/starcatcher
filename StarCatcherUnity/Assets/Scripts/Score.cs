using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int starCaughtCount;
    public static int level;
    private static Constellations constellations;
    internal static readonly int LEVEL_ONE = 0;
    internal static readonly int LEVEL_TWO = 1;
    internal static readonly int LEVEL_THREE = 2;

    public static float environmentDamageScore;
    public static float cumulativeEnvironmentDamageScore;
    public int totalStarsToBeCaught;
    
    public enum GameState {Dead, Rejuvination, Flourishing, Decline, Dying};
    public static GameState plasmaWorldState;
    private float rejuvinationTimer;
    public float rejuvinationTimerValue = 5f;
    public static float deadTimer;
    public float deadTimerValue = 10f;
    private float reduceScoreTimer;
    public float reduceScoreTimerValue = 10f;

    private static BackingTracks BackingTracks;
    private scoreLog scoreLogger;

    public static Score Instance;
     
    void Awake(){
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        starCaughtCount = 0;
        totalStarsToBeCaught = 30;
        SetLevel(0);
        plasmaWorldState = GameState.Rejuvination;
        rejuvinationTimer = rejuvinationTimerValue;
        deadTimer = deadTimerValue;
        reduceScoreTimer = reduceScoreTimerValue;

        constellations = FindObjectOfType<Constellations>();
        if (null == constellations)
        {
            Debug.Log("ERROR: no constellation manager found");
        }

        BackingTracks = FindObjectOfType<BackingTracks>();
        if (null == BackingTracks)
        {
            Debug.LogWarning("WARN: no BackingTracks found");
        }

        scoreLogger = (scoreLog)FindObjectOfType<scoreLog>();

    }

    public Vector3 catchStar()
    {
        starCaughtCount++;

        //TODO: add toggle for game type
        // if (0 == (constellationScore % 10))
        // {
        //     SetLevel(level+1);
        // }
        return constellations.GetNextEmptyPosition().transform.position;
    }

    public static void SetLevel(int level)
    {
        Score.level = level;
        Score.level %= 3;
        //BackingTracks.UpdateLevel();
    }

    internal static int GetCurrentLevel()
    {
        return level;
    }

    public void checkEffectOnEnvironment()
    {
        // TODO: replace formula
        if (starCaughtCount <= totalStarsToBeCaught) {
            environmentDamageScore = (float) starCaughtCount / totalStarsToBeCaught;
            cumulativeEnvironmentDamageScore = environmentDamageScore;
            //Debug.Log("damage: " + cumulativeEnvironmentDamageScore);
        }
    }

    void Update(){
        // set timers for rejuvination state and dead state
        // during rejuvination, no stars can be caught
        // during dead state, no stars are spawned
        if (plasmaWorldState == GameState.Rejuvination) {
            setRejuvinationTimer();
        }

        if (plasmaWorldState == GameState.Dead) {
            setDeadTimer();
        }

        checkEffectOnEnvironment();
        if (plasmaWorldState != GameState.Rejuvination && plasmaWorldState != GameState.Dead){
            SetGameState();
        }
        scoreLogger.LogScore();

        // simulate stars caught with hotkey
        if (Input. GetKeyUp("c")){
            starCaughtCount++;
        }
    }

    public void SetGameState()
    {
        // change states as damage score increases
        if (cumulativeEnvironmentDamageScore < 0.5) plasmaWorldState = GameState.Flourishing;
        else if (cumulativeEnvironmentDamageScore >= 0.5 && cumulativeEnvironmentDamageScore < 0.8)
        {
            plasmaWorldState = GameState.Decline;
        } else if (cumulativeEnvironmentDamageScore >= 0.8 && cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = GameState.Dying;
        } else if (cumulativeEnvironmentDamageScore == 1)
        {
            plasmaWorldState = GameState.Dead;
        }
        //Debug.Log("score: " + cumulativeEnvironmentDamageScore + "state: " + plasmaWorldState);

        // Reduce score over time so that the world regenerates if noone is playing
        reduceScoreTimer -= Time.deltaTime;
        if (reduceScoreTimer <= 0) {
            if (starCaughtCount > 0) starCaughtCount --;
            reduceScoreTimer = reduceScoreTimerValue;
        }

    }

    public void setRejuvinationTimer(){
        rejuvinationTimer -= Time.deltaTime;
        if (rejuvinationTimer <= 0)  {
            plasmaWorldState = GameState.Flourishing;
            rejuvinationTimer = rejuvinationTimerValue;
        }
    }

    public void setDeadTimer(){
        deadTimer -= Time.deltaTime;
        if (deadTimer <= 0)  {
            plasmaWorldState = GameState.Rejuvination;
            starCaughtCount = 0;
            deadTimer = deadTimerValue;
        }
    }
}
