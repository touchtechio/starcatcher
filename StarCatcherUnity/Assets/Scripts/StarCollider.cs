using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

public class StarCollider : MonoBehaviour {
    //public float timeToDestory = 1f;
    float blazeTime = 1f;
    float timeToMove;
    float timeCaught;
    float travelSpeed = .5f;
    Vector3 starCaughtPosition;
    Vector3 onConstellationPosition;
    float journeyLength;
    public bool isStarCaught = false;
    public float tiltAngle;
    HU_Star starEffects;
    public int caughtStripNumber;
    private SoundManager soundManager;
    private GameObject gameManagerObject;
    private Score gameScore;
 
    OSCSenderCaught oscSenderObject;

    // Use this for initialization
    void Start () {
        tiltAngle = Random.Range(-80f, 80f);
        
        starEffects = gameObject.GetComponent<HU_Star>();
        oscSenderObject = (OSCSenderCaught)FindObjectOfType<OSCSenderCaught>();
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();
        gameManagerObject = GameObject.Find("GameManager");
        gameScore = (Score)FindObjectOfType<Score>();
       
    }


    // Update is called once per frame
    void Update () {

        if (isStarCaught == true && gameScore.constellationMode == true)
        {
            timeToMove = timeCaught + blazeTime;
            //Debug.Log("time to move" + timeToMove);
            float distCovered = (Time.time - timeToMove) * travelSpeed;
            float fracJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(starCaughtPosition, onConstellationPosition, fracJourney);
            // transform.position+= new Vector3(0.1f,0.1f,0.1f);

            // adding accelaration
            if (transform.position != onConstellationPosition)
            {
                travelSpeed += .03f;

               // float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
               // float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
                Quaternion targetRotation = Quaternion.Euler(tiltAngle, 0, tiltAngle);
                if (starEffects._jets == true)
                {
                 //   Debug.Log("rotate with angle " + tiltAngle);
                    transform.GetChild(4).rotation = Quaternion.Slerp(transform.rotation, targetRotation, travelSpeed);
                }
            }
        } else if (isStarCaught == true){
            // destroy star
            Destroy(gameObject);
        }
        
	}

    //When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    void OnTriggerEnter(Collider other)
    {
        // other.gameObject.CompareTag(NET);
        if (isStarCaught == true)
        {
            return;
        }
        isStarCaught = true;

        timeCaught = Time.time;
        Debug.Log("caught: "+ caughtStripNumber);
        oscSenderObject.SendOSCCaughtMessage("/starcaught", caughtStripNumber);

        // play caught sound at place of caught
        SoundCatch();
        //Destroy(gameObject, timeToDestory);

        starEffects._color = Color.cyan;
        starEffects._jets = true;
        starCaughtPosition = transform.position;

        // remove mesh collider in Jet when caught
        foreach (Transform child in gameObject.transform.GetChild(4))
        {
            child.GetComponentInChildren<MeshCollider>().enabled = false;
        }
        if (gameScore.constellationMode == true) 
        {
            onConstellationPosition = gameScore.catchStar(); // position got from the constellations script
        } else {
            gameScore.catchStarNoConstellation();
        }
    }

    private void SoundCatch()
    {
        if (null != soundManager.starCaught)
        {
            soundManager.PlayStarCaughtSound(transform.position);
        }
    }
}
