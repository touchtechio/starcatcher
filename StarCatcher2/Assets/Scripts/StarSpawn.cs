using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawn : MonoBehaviour {

    public GameObject prefab;
    


    private static ArrayList starStarts;
    private static Random random = new Random();

    // empty game object as parent for spwned stars
    private GameObject parent;

    // Use this for initialization
    void Start () {

        parent = GameObject.FindGameObjectWithTag("STARS");

        InvokeRepeating("Spawn", 0.5f, 5.0f);

        starStarts = new ArrayList();
        for (int i=0; i<5; i++)
        {
            starStarts.Add(new Vector3(i, i * 0.2f, 10));
        }

        
    }
	
    public void Spawn()
    {

        int startPosition = Random.Range(0, starStarts.Count);
        Vector3 start = (Vector3)starStarts.ToArray().GetValue(startPosition);


        GameObject star = Instantiate(prefab, start, Quaternion.identity) as GameObject;
        star.transform.parent = parent.transform;
        star.name = "star-" + startPosition;


    }

}
