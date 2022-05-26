using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public static int starCaughtCount;
    public static int level;
    private static Constellations constellations;
    internal static readonly int LEVEL_ONE = 0;
    internal static readonly int LEVEL_TWO = 1;
    internal static readonly int LEVEL_THREE = 2;

    public static float environmentDamageScore;
    public static float cumulativeEnvironmentDamageScore;
    public static int totalStarsToBeCaught;
    public static int minStarCaught;
    public static string plasmaWorldState;


    public int totalStarsToBeCaughtUser;
    public int minStarCaughtUser;


    private static BackingTracks BackingTracks;

    // Use this for initialization
    void Start () {
        starCaughtCount = 0;
        minStarCaught = 3;
        totalStarsToBeCaught = 10;
        SetLevel(0);
        plasmaWorldState = "flourish";

        if (totalStarsToBeCaughtUser > 0) 
        {
            totalStarsToBeCaught = totalStarsToBeCaughtUser;
        }

        if (minStarCaughtUser > 0)
        {
            minStarCaught = minStarCaughtUser;
        }

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

    public static Vector3 catchStar()
    {
        starCaughtCount++;
        Debug.Log("caught star " + starCaughtCount);
        checkEffectOnEnvironment();
        SetGameState();

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

    public static void checkEffectOnEnvironment()
    {
        // TODO: replace formula
        if (minStarCaught < starCaughtCount & starCaughtCount <= (totalStarsToBeCaught + minStarCaught)) {
            environmentDamageScore = (float) (starCaughtCount - minStarCaught) / totalStarsToBeCaught;
            cumulativeEnvironmentDamageScore = environmentDamageScore;
            Debug.Log("environmental damage " + cumulativeEnvironmentDamageScore);
        }

    }
    void Update(){
        if (Input. GetKeyUp("c")){
            starCaughtCount++;
            checkEffectOnEnvironment();
            Debug.Log("key press caught star " + starCaughtCount);
        }
    }

    public static void SetGameState()
    {
        if (cumulativeEnvironmentDamageScore < 0.5)
        {
            plasmaWorldState = "flourish";
        } else if (cumulativeEnvironmentDamageScore >= 0.5 || cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = "decline";
        } else if (cumulativeEnvironmentDamageScore == 1)
        {
            plasmaWorldState = "dead";
            //TODO: start dead sequence of events and game reset
        }


    }
}
