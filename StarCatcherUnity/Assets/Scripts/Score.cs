using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int starCaughtCount;
    public static int starCaughtLog;
    public static int level;
    private static Constellations constellations;
    private static StarAnimations starAnimations;
    internal static readonly int LEVEL_ONE = 0;
    internal static readonly int LEVEL_TWO = 1;
    internal static readonly int LEVEL_THREE = 2;

    public static float environmentDamageScore;
    public static float cumulativeEnvironmentDamageScore;
    public int totalStarsToBeCaught;
    
    public enum GameState {Dead, Rejuvination, Flourishing, Decline, Dying};
    public static GameState plasmaWorldState;
    private static GameState previousWorldState;
    private float rejuvinationTimer;
    public float rejuvinationTimerValue = 5f;
    public static float deadTimer;
    public float deadTimerValue = 10f;
    private float reduceScoreTimer;
    public float reduceScoreTimerValue = 10f;

    private static BackingTracks BackingTracks;
    private scoreLog scoreLogger;
    public bool constellationMode = true;

    public static Score Instance;
    private bool stateChange = false;
     
    void Awake(){
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        starCaughtCount = 0;
        totalStarsToBeCaught = 30;
        SetLevel(0);
        plasmaWorldState = GameState.Rejuvination;
        previousWorldState = plasmaWorldState;
        rejuvinationTimer = rejuvinationTimerValue;
        deadTimer = deadTimerValue;
        reduceScoreTimer = reduceScoreTimerValue;

        constellations = FindObjectOfType<Constellations>();
        starAnimations = FindObjectOfType<StarAnimations>();
        if (null == constellations)
        {
            Debug.Log("ERROR: no constellation manager found");
            constellationMode = false;
        }
        if (null == starAnimations)
        {
            Debug.Log("ERROR: no star animation manager found");
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

    public void catchStarNoConstellation()
    {
        starCaughtCount++;
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
            //cumulativeEnvironmentDamageScore = environmentDamageScore;
            cumulativeEnvironmentDamageScore = (float) Mathf.Exp(-1 * Mathf.Pow(3*environmentDamageScore-3.0f,2f));
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
        starCaughtLog = starCaughtCount;
    }

    public void SetGameState()
    {
        // change states as damage score increases
        if (cumulativeEnvironmentDamageScore < 0.2) {
            plasmaWorldState = GameState.Flourishing;
            previousWorldState = plasmaWorldState;
        }
        else if (cumulativeEnvironmentDamageScore >= 0.2 && cumulativeEnvironmentDamageScore < 0.7)
        {
            plasmaWorldState = GameState.Decline;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullAnimation();
            }
            previousWorldState = plasmaWorldState;
        } else if (cumulativeEnvironmentDamageScore >= 0.7 && cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = GameState.Dying;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullAnimation();
            }
            previousWorldState = plasmaWorldState;

        } else if (cumulativeEnvironmentDamageScore == 1)
        {
            plasmaWorldState = GameState.Dead;
            StarSpawn.DestroyStars();
            previousWorldState = plasmaWorldState;
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
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullAnimation();
            }
            previousWorldState = plasmaWorldState;
        }
    }

    public void setDeadTimer(){
        deadTimer -= Time.deltaTime;
        if (deadTimer <= 0)  {
            plasmaWorldState = GameState.Rejuvination;
            starCaughtCount = 0;
            deadTimer = deadTimerValue;
            previousWorldState = plasmaWorldState;
        }
    }
}
