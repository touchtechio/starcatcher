using UnityEngine;
using UniOSC;

public class StarSpawnFormations : MonoBehaviour
{
    /*
    The Star Spawn formations class handles behaviors for stars spawned using shapes
    and collisions with star positions
    */
    public StarSpawn starSpawn;

    public StarSpawnBase starSpawnBase;

    OSCSenderFaintStarLinger oscSenderFaintStarLinger;


    // Start is called before the first frame update
    void Start()
    {
        starSpawn = StarSpawn.FindObjectOfType<StarSpawn>();
        oscSenderFaintStarLinger = (OSCSenderFaintStarLinger)FindObjectOfType<OSCSenderFaintStarLinger>();

        if (null == starSpawn)
        {
            Debug.Log("ERROR: no starSpawn found");
        }    
    }


    void OnCollisionEnter(Collision collision){
        foreach(ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
            Vector3 position = contact.point;
        }
    
        if (collision.gameObject.tag == "STAR POSITION TUTORIAL")
        {
            // Debug.Log("Spawn tutorial formation");
            GameObject starPosition = collision.gameObject;
            StripIndex index = starPosition.GetComponent<StripIndex>();
            // Debug.Log("star index " + index.stripIndex);
            starSpawnBase.SpawnStrip(index.stripIndex);
            return;
        }
        
        if (collision.gameObject.tag == "STAR POSITION")
        {
            GameObject starPosition = collision.gameObject;
            StripIndex index = starPosition.GetComponent<StripIndex>();
            if (gameObject.tag == "STAR POSITION REVIVE")
            {
                // Debug.Log("Spawn tutorial revive this star");
                Debug.Log("faintly star " + index.stripIndex);
                // No need to spawn as dead stars are already created
                // This creates the linger signal for the faint star to turn on
                oscSenderFaintStarLinger.SendOSCFaintStarLingerMessage("/faintstarlinger", index.stripIndex, 30000);
                return;
        }
            //Debug.Log("Spawn from formation");
            //Debug.Log("star index " + index.stripIndex);
            starSpawn.Spawn(index.stripIndex);
        }


    
    }
}
