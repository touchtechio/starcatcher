using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniOSC;




public class StripPositionMessageSender : MonoBehaviour {

    int currentStripSent;
    public StripPosition StripPosition;

    public OSCSenderCaught OSCSenderCaught;
    public OSCSenderLinger OSCSenderLinger;

    // Use this for initialization
    void Start() {
        StripPosition = FindObjectOfType<StripPosition>();
        currentStripSent = StripPosition.Count();
        if (null == StripPosition)
        {
            Debug.LogWarning("cannot find StripPositon, does it exist in the scene?");
        }
        if (null == OSCSenderCaught)
        {
            Debug.LogWarning("cannot find OSCSenderCaught, was it wired up?");
        }
        if (null == OSCSenderLinger)
        {
            Debug.LogWarning("cannot find OSCSenderLinger, was it wired up?");
        }

    }

    // Update is called once per frame
    void Update() {

        // do nothing, if strip count hasn't increased
        if (currentStripSent == StripPosition.Count())
        {
            return;
        }

        Debug.Log("catch " + currentStripSent);
        OSCSenderCaught.SendOSCCaughtMessage("/starcaught", currentStripSent);

        currentStripSent = StripPosition.Count();

        Debug.Log("linger " + currentStripSent);
        OSCSenderLinger.SendOSCLingerMessage("/starlinger", currentStripSent, 30000);

    }


}
