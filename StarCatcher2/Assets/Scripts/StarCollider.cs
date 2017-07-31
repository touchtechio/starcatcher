using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollider : MonoBehaviour {
    public float timeToDestory = 1f;
    private SerialController starCaughtSerialController;
    // Use this for initialization
    void Start () {
        starCaughtSerialController = SerialController.FindObjectOfType<SerialController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
       // other.gameObject.CompareTag(NET);
        Score.catchStar();
        Destroy(gameObject, timeToDestory);
        starCaughtSerialController.SendSerialMessage("x");
        HU_Star starEffects = gameObject.GetComponent<HU_Star>();
        starEffects._color = Color.cyan;
        starEffects._jets = true; 
     
    }

}
