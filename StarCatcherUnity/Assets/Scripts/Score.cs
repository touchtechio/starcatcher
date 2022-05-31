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

    private static BackingTracks BackingTracks;

    // Use this for initialization
    void Start () {
        starCaughtCount = 0;
        totalStarsToBeCaught = 30;
        SetLevel(0);
        plasmaWorldState = GameState.Flourishing;

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

    }

    public Vector3 catchStar()
    {
        starCaughtCount++;
        Debug.Log("caught star " + starCaughtCount);

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
             Debug.Log("damage: " + cumulativeEnvironmentDamageScore);

        }

    }
    void Update(){
        if (Input. GetKeyUp("c")){
            starCaughtCount++;
        }
        checkEffectOnEnvironment();
        SetGameState();
    }

    public void SetGameState()
    {
        if (cumulativeEnvironmentDamageScore < 0.5) plasmaWorldState = GameState.Flourishing;
        else if (cumulativeEnvironmentDamageScore >= 0.5 || cumulativeEnvironmentDamageScore < 0.9)
        {
            plasmaWorldState = GameState.Decline;
        } else if (cumulativeEnvironmentDamageScore >= 0.9 || cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = GameState.Dying;
        } else if (cumulativeEnvironmentDamageScore == 1)
        {
            plasmaWorldState = GameState.Dead;
            //TODO: start dead sequence of events and game reset
        }
        //Debug.Log("state: " + plasmaWorldState);

    }
}
