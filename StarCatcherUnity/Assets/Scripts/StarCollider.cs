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
    public int CaughtStripNumber;
    OSCSenderCaught oscSenderObject;

    // Use this for initialization
    void Start () {
        tiltAngle = Random.Range(-80f, 80f);
        
        starEffects = gameObject.GetComponent<HU_Star>();
        oscSenderObject = (OSCSenderCaught)FindObjectOfType<OSCSenderCaught>();
    }


    // Update is called once per frame
    void Update () {

        if (isStarCaught == true)
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
        }
	}

    void OnTriggerEnter(Collider other)
    {
        // other.gameObject.CompareTag(NET);
        if (isStarCaught == true)
        {
            return;
        }
        isStarCaught = true;
        timeCaught = Time.time;
        oscSenderObject.SendOSCCaughtMessage("/starcaught", CaughtStripNumber);
        //Debug.Log("star caught at " +timeCaught);
        //Destroy(gameObject, timeToDestory);
        
        starEffects._color = Color.cyan;
        starEffects._jets = true;
        starCaughtPosition = transform.position;

        // remove mesh collider in Jet when caught
        foreach (Transform child in gameObject.transform.GetChild(4))
        {
            child.GetComponentInChildren<MeshCollider>().enabled = false;
        }

        onConstellationPosition = Score.catchStar(); // position got from the constellations script
        journeyLength = Vector3.Distance(starCaughtPosition, onConstellationPosition);

     
    }

}
