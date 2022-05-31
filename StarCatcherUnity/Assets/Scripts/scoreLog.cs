using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class scoreLog : MonoBehaviour {
    public GameObject scoreObjectUI;
    public Text scoreText;
    private float starScoreLogger;

    public GameObject worldStateUI;
    public Text worldStateText;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        starScoreLogger = Score.cumulativeEnvironmentDamageScore;
        //Debug.Log(starStripLogger);

        scoreObjectUI = GameObject.Find("ScoreText");
        scoreText = scoreObjectUI.GetComponent<Text>();
        worldStateUI = GameObject.Find("WorldState");
        worldStateText = worldStateUI.GetComponent<Text>();

    }

    public void LogScore()
    {
        scoreText.text = (starScoreLogger).ToString();
        worldStateText.text = (Score.plasmaWorldState).ToString();
    }
}
