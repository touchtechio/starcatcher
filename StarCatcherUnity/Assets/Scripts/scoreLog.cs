using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class scoreLog : MonoBehaviour {
    private GameObject scoreObjectUI;
    private Text scoreText;
    private GameObject worldStateUI;
    private Text worldStateText;
    private float starScoreLogger;
    private GameObject starsCaughtUI;
    private Text starsCaughtText;

    // Use this for initialization
    void Start() {
        scoreObjectUI = GameObject.Find("ScoreText");
        scoreText = scoreObjectUI.GetComponent<Text>();
        worldStateUI = GameObject.Find("WorldState");
        worldStateText = worldStateUI.GetComponent<Text>();
        starsCaughtUI = GameObject.Find("StarsCaught");
        starsCaughtText = starsCaughtUI.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        starScoreLogger = Score.cumulativeEnvironmentDamageScore;
    }

    public void LogScore()
    {
        scoreText.text = (starScoreLogger).ToString();
        worldStateText.text = (Score.plasmaWorldState).ToString();
        starsCaughtText.text = (Score.starCaughtLog).ToString();
    }
}
