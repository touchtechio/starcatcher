using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class DeadStarCollider : MonoBehaviour {
    //public float timeToDestory = 1f;
    float timeCaught;
    Vector3 starCaughtPosition;
    Vector3 onConstellationPosition;
    float journeyLength;
    public bool isStarReturned = false;
    public int starCollideCount = 0;
    public int availableStarReturns = 5;
    HU_Star starEffects;
    public int caughtStripNumber;
    private SoundManager soundManager;
    private GameObject gameManagerObject;
    private Score gameScore;
 
    // OSC
    OSCSenderReturn oscSenderObject;
    OSCSenderLinger oscSenderLinger;


    // Use this for initialization
    void Start () {
        starEffects = gameObject.GetComponent<HU_Star>();
        oscSenderObject = (OSCSenderReturn)FindObjectOfType<OSCSenderReturn>();
        oscSenderLinger = (OSCSenderLinger)FindObjectOfType<OSCSenderLinger>();
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();
        gameManagerObject = GameObject.Find("GameManager");
        gameScore = (Score)FindObjectOfType<Score>();
       
    }


    // Update is called once per frame
    void Update () {

        if (isStarReturned == true){
            // destroy star
            Destroy(gameObject);
        }
        
	}

    //When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    void OnTriggerEnter(Collider collision)
    {
        // other.gameObject.CompareTag(NET);
        // TODO: maybe deactivate this
        if (isStarReturned == true)
        {
            return;
        }

        // star can be returned up to x times in the same position before marked destroyed
        starCollideCount++;
        if (starCollideCount >= availableStarReturns) {
            // removing this capability as we want to keep returning stars
            //isStarReturned = true;
        }

        timeCaught = Time.time;
        Debug.Log("returning: "+ caughtStripNumber);

                // Debug.Log("linger " + caughtStripNumber);
        oscSenderLinger.SendOSCLingerMessage("/starlinger", caughtStripNumber, 2000);
        oscSenderObject.SendOSCReturnMessage("/starreturn", caughtStripNumber);

        // play caught sound at place of caught
        SoundCatch();
        //Destroy(gameObject, timeToDestory);

        gameScore.returnStar(); // position got from the constellations script

    }

    private void SoundCatch()
    {
        if (null != soundManager.starCaught)
        {
            soundManager.PlayStarCaughtSound(transform.position);
        }
    }
}
