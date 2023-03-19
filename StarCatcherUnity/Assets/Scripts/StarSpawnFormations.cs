using UnityEngine;

public class StarSpawnFormations : MonoBehaviour
{
    /*
    The Star Spawn formations class handles behaviors for stars spawned using shapes
    and collisions with star positions

    */
    public StarSpawn starSpawn;

    public StarSpawnBase starSpawnBase;


    // Start is called before the first frame update
    void Start()
    {
        starSpawn = StarSpawn.FindObjectOfType<StarSpawn>();
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
            //Debug.Log("Spawn from formation");
            GameObject starPosition = collision.gameObject;
            StripIndex index = starPosition.GetComponent<StripIndex>();
            //Debug.Log("star index " + index.stripIndex);
            starSpawn.Spawn(index.stripIndex);
        }
    
    }
}
