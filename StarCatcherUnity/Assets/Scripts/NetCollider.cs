using UnityEngine;
using Valve.VR;

public class NetCollider : MonoBehaviour
{
    /*
    Net collider handles star counts and net    
    */
    // private StarSpawn starSpawn;

    // private StarSpawnBase starSpawnBase;
    public int starsCaughtbyNet = 0;

    public int deviceIndex;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("net check " + gameObject.transform.parent.parent.name);
        // starSpawn = StarSpawn.FindObjectOfType<StarSpawn>();
        // starSpawnBase = StarSpawn.FindObjectOfType<StarSpawnBase>();
        // if (null == starSpawn)
        // {
        //     Debug.Log("ERROR: no starSpawn found");
        // }    
        
        SteamVR_TrackedObject netController = gameObject.transform.parent.parent.gameObject.GetComponent<SteamVR_TrackedObject>();
        deviceIndex = (int)netController.index;
    }


    void OnCollisionEnter(Collision collision){
        // Debug.Log(gameObject + "caught a star "+ collision.gameObject);
        foreach(ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
            Vector3 position = contact.point;
        }
        // Debug.Log(collision.gameObject.transform.parent.name);
        if (collision.gameObject.transform.parent.name == "STARS") {
            Debug.Log("caught a star "+ collision.gameObject.transform.name);
            starsCaughtbyNet++;
            Debug.Log(gameObject.transform.parent.parent.name + " caught " + starsCaughtbyNet + " stars");
        }
    
    }
}
