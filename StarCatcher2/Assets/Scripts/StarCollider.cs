using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollider : MonoBehaviour {
    //public float timeToDestory = 1f;
    float blazeTime = 1f;
    float timeToMove;
    private SerialController starCaughtSerialController;
    float timeCaught;
    float speedTravel = 0.5f;
    Vector3 starCaughtPosition;
    Vector3 onConstellationPosition;
    float journeyLength;
    public bool isStarCaught = false;

    // Use this for initialization
    void Start () {
        starCaughtSerialController = SerialController.FindObjectOfType<SerialController>();
        if (null == starCaughtSerialController)
        {
            Debug.Log("ERROR: no serial controller found");
        }

    }

    // Update is called once per frame
    void Update () {

        if (isStarCaught == true)
        {
            timeToMove = timeCaught + blazeTime;
            //Debug.Log("time to move" + timeToMove);
            float distCovered = (Time.time - timeToMove) * speedTravel;
            float fracJourney = distCovered / journeyLength;
          
            transform.position = Vector3.Lerp(starCaughtPosition, onConstellationPosition, fracJourney);
           // transform.position+= new Vector3(0.1f,0.1f,0.1f);
        }


	}

    void OnTriggerEnter(Collider other)
    {
       // other.gameObject.CompareTag(NET);
        Score.catchStar();
        isStarCaught = true;
        timeCaught = Time.time;
        Debug.Log("star caught at " +timeCaught);
        //Destroy(gameObject, timeToDestory);
        starCaughtSerialController.SendSerialMessage("x");
        HU_Star starEffects = gameObject.GetComponent<HU_Star>();
        starEffects._color = Color.cyan;
        starEffects._jets = true;
        starCaughtPosition = transform.position;
        onConstellationPosition = starCaughtPosition + new Vector3(-1, 3, -1);
        journeyLength = Vector3.Distance(starCaughtPosition, onConstellationPosition);
     
    }

}
