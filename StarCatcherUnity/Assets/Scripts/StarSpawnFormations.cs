using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawnFormations : MonoBehaviour
{
    public StarSpawn starSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
     Debug.Log("I AM SPHERE");

        starSpawn = StarSpawn.FindObjectOfType<StarSpawn>();
        if (null == starSpawn)
        {
            Debug.Log("ERROR: no starSpawn found");
        }    
    }


    void OnCollisionEnter(Collision collision){
        Debug.Log("collided, " + collision.gameObject.name);
        foreach(ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
            Vector3 position = contact.point;
            Debug.Log("collided, "+ position);
        }
    
        if (collision.gameObject.tag == "STAR POSITION")
        {
            GameObject starPosition = collision.gameObject;
            StripIndex index = starPosition.GetComponent<StripIndex>();
            starSpawn.Spawn(index.stripIndex);
        }
    
    }
}
