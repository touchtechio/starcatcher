using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public static int score;
    private static Constellations constellations;
    internal static readonly int LEVEL_ONE = 0;
    internal static readonly int LEVEL_TWO = 1;
    internal static readonly int LEVEL_THREE = 2;

    private static BackingTracks BackingTracks;

    // Use this for initialization
    void Start () {
        score = 0;
        constellations = FindObjectOfType<Constellations>();
        if (null == constellations)
        {
            Debug.Log("ERROR: no constellation manager found");
        }

        BackingTracks = FindObjectOfType<BackingTracks>();
        if (null == BackingTracks)
        {
            Debug.LogWarning("WARN: no BackingTracks  found");
        }

    }

    public static Vector3 catchStar()
    {
        score++;
        Debug.Log("caught star " + score);

        if (0 == (score%10))
        {
            BackingTracks.UpdateLevel();
        }
        return constellations.GetNextEmptyPosition().transform.position;
    }

    internal static int GetCurrentLevel()
    {
        return LEVEL_ONE;
    }
}
