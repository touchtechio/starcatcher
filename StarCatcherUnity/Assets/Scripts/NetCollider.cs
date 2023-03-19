using UnityEngine;

public class NetCollider : MonoBehaviour
{
    /*
    Net collider handles star counts and net    
    */
    // private StarSpawn starSpawn;

    // private StarSpawnBase starSpawnBase;
    public int starsCaughtbyNet = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("net check");
        // starSpawn = StarSpawn.FindObjectOfType<StarSpawn>();
        // starSpawnBase = StarSpawn.FindObjectOfType<StarSpawnBase>();
        // if (null == starSpawn)
        // {
        //     Debug.Log("ERROR: no starSpawn found");
        // }    
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
