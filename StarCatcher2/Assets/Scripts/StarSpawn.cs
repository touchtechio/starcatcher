using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawn : MonoBehaviour {

    [Range(0.5f, 5.0f)]
    public float spawnRate = 1.0f;  // seconds between spawnings
    public GameObject prefab;
    


    private static ArrayList starStarts;
    private static Random random = new Random();
    private SerialController starSpawnSerialController;

    // empty game object as parent for spwned stars
    private GameObject parent;
    int starCount = 0;


    // set to durations between spawn events
    public float easyTimeToSpawn = 0;
    public float hardTimeToSpawn = 0;


    private float easyTimeToNextSpawn = 0;
    private float hardTimeToNextSpawn = 0;

    // Use this for initialization
    void Start () {

        parent = GameObject.FindGameObjectWithTag("STARS");

        //InvokeRepeating("Spawn", 0.5f, spawnRate);
        InvokeRepeating("Spawn", 0.5f, 5.0f);

        starStarts = new ArrayList();
        starStarts.Add(new Vector3(-1.022f, 3.048f, -0.069f));
        starStarts.Add(new Vector3(-0.94f, 3.048f, 0.42f));
        starStarts.Add(new Vector3(-0.49f, 3.048f, -0.29f));
        starStarts.Add(new Vector3(-0.46f, 3.048f, 0.8f));
        starStarts.Add(new Vector3(-0.05f, 3.048f, -0.88f));
        starStarts.Add(new Vector3(0.13f, 3.048f, 0.01f));
        starStarts.Add(new Vector3(-0.09f, 3.048f, 0.89f));
        starStarts.Add(new Vector3(1.06f, 3.048f, -0.92f));
        starStarts.Add(new Vector3(0.97f, 3.048f, 0.02f));
        starStarts.Add(new Vector3(0.94f, 3.048f, 0.84f));
        starStarts.Add(new Vector3(1.75f, 3.048f, -1.19f));
        starStarts.Add(new Vector3(1.78f, 3.048f, -0.47f));
        starStarts.Add(new Vector3(1.67f, 3.048f, 0.17f));
        starStarts.Add(new Vector3(1.7f, 3.048f, 0.5f));
        starStarts.Add(new Vector3(1.62f, 3.048f, 1.1f));


        starSpawnSerialController = SerialController.FindObjectOfType<SerialController>();

    }

    public void Update()
    {

        // burn down time since last update
        easyTimeToNextSpawn -= Time.deltaTime;
        hardTimeToNextSpawn -= Time.deltaTime;


        if (easyTimeToNextSpawn <= 0)
        {
            SpawnEasy();
            easyTimeToNextSpawn = easyTimeToSpawn;
        }
        if (hardTimeToNextSpawn <= 0)
        {
            SpawnHard();
            hardTimeToNextSpawn = hardTimeToSpawn; ;
        }

        print("10 seconds ellapsed");


    }





    private void SpawnEasy()
{
        Vector3 spawnPoint = GetSpawnPoint();
        GameObject star = Instantiate(prefab, spawnPoint, Quaternion.identity) as GameObject;
        star.transform.parent = parent.transform;
        starCount++;
        string starNumber = starCount.ToString();
        star.name = "star-" + starNumber;

        starNumber = "a";
        starSpawnSerialController.SendSerialMessage(starNumber);
        Debug.Log("star fell");

    }


    private void SpawnHard()
{
        SpawnEasy();

    }

    private Vector3 GetSpawnPoint()
    {
        int startPosition = Random.Range(0, starStarts.Count);
        Vector3 start = (Vector3)starStarts.ToArray().GetValue(startPosition);
        Debug.Log("new star point: " + start.ToString());
        return start;
    }


    public void Spawn()
    {

        SpawnEasy();
        
       

    }

}


