using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class scoreLog : MonoBehaviour {
    public GameObject scoreObjectUI;
    private Text scoreText;
    public GameObject worldStateUI;
    private Text worldStateText;
    private float starScoreLogger;
    public GameObject starsCaughtUI;
    private Text starsCaughtText;

    // Use this for initialization
    void Start() {
        //scoreObjectUI = GameObject.Find("ScoreText");
        scoreText = scoreObjectUI.GetComponent<Text>();
        //worldStateUI = GameObject.Find("WorldState");
        worldStateText = worldStateUI.GetComponent<Text>();
        //starsCaughtUI = GameObject.Find("StarsCaught");
        starsCaughtText = starsCaughtUI.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        starsCaughtText.enabled = true;
        starScoreLogger = Score.cumulativeEnvironmentDamageScore;
        if (Score.plasmaWorldState == Score.GameState.Dead || Score.plasmaWorldState == Score.GameState.Rejuvination)
        {
            starsCaughtText.enabled = false;
        } 
    }

    public void LogScore()
    {
        scoreText.text = (starScoreLogger).ToString();
        worldStateText.text = (Score.plasmaWorldState).ToString();
        //starsCaughtText.text = "stars caught  " + (Score.starCaughtLog).ToString();
        starsCaughtText.text = (Score.starCaughtLog + Score.randomAdd).ToString();
        //Debug.Log("random" +  Score.randomAdd);
    }
}
