using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This is a nothing script that just instantiates the plant amanger prefab

I mostly made this so that I can work on the plants without changing anything in the scene
*/

public class PlantKickoff : MonoBehaviour
{

    public GameObject plant_manager_prefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(plant_manager_prefab, transform.position, Quaternion.identity);
    }

}
