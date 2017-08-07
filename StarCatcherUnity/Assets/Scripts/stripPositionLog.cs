using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class stripPositionLog : MonoBehaviour {
    public GameObject stripCountObject;
    public Text stripCountText;
    public bool clearStripArray = false;
    private Scene currentScene;
    int starStripLogger;
    public StripPosition stripPosition;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        stripPosition = FindObjectOfType<StripPosition>();
        starStripLogger = stripPosition.starStripCount;
        //Debug.Log(starStripLogger);


        stripCountObject = GameObject.Find("StripLog");
        stripCountText = stripCountObject.GetComponent<Text>();

        // clear the array of strip positions to repopulate;
        if (clearStripArray == true)
        {

            stripCountText.text = "Clear";
            clearStripArray = false;
        }

    }

    public void LogStrips()
    {
        stripCountText.text = (starStripLogger).ToString();
    }
}
