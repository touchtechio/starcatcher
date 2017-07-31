using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripPosition : MonoBehaviour {


    public GameObject prefab;
    


    private static ArrayList starStarts;
    private static Random random = new Random();


    // empty game object as parent for strips
    private GameObject parent;



    // Use this for initialization
    void Start () {

        parent = GameObject.FindGameObjectWithTag("STRIPS");

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

        
    }

    public void Update()
    {

       

    }




    


}


