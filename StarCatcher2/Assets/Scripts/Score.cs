using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public static int score;
    private static Constellations constellations;

	// Use this for initialization
	void Start () {
        score = 0;
        constellations = FindObjectOfType<Constellations>();

        if (null == constellations)
        {
            Debug.Log("ERROR: no constellation manager found");
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    public static Vector3 catchStar()
    {
        score++;
        Debug.Log("caught star " + score);
        return constellations.GetNextEmptyPosition().transform.position;
    }
    
}
