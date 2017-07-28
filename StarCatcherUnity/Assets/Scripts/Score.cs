using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public static int score;

	// Use this for initialization
	void Start () {
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void catchStar()
    {
        score++;
        Debug.Log("caught star " + score);
    }
}
