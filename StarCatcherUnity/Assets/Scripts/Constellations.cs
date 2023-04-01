using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniOSC;


public class Constellations : MonoBehaviour {

    public List<Constellation> constellations = new List<Constellation>();
    OSCSenderComplete oscSenderObject;
    private SoundManager soundManager;
    public StarSpawn starSpawn;


    bool allFull = false;

    // Use this for initialization
    void Start () {

        oscSenderObject = (OSCSenderComplete)FindObjectOfType<OSCSenderComplete>();
        soundManager = (SoundManager)FindObjectOfType<SoundManager>();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("CONSTELLATION");
        if (objs.Length == 0) {
            Debug.Log("ERROR: no constellations defined");
        }

        for (int i = 0; i < objs.Length; i++)
        {
            // deactivate all but the first constellation
            if (i != 0)
            {
                objs[i].SetActive(false);
            }

            // find all the consteallation positions
            List<GameObject> Children = new List<GameObject>();
            foreach (Transform child in objs[i].transform)
            {
                if (child.tag == "CONSTELLATION_POSITION")
                {
                    child.gameObject.SetActive(false);
                    Children.Add(child.gameObject);
                    //Debug.Log("Found : " + child.tag + ":: position: " + child.transform.localPosition + " on " + objs[i].name);

                }

            }


            // create constellation and all to our list
            constellations.Add(new Constellation(objs[i], Children, objs[i].name));
            starSpawn = (StarSpawn)FindObjectOfType<StarSpawn>();

        }

    }

    public GameObject GetNextEmptyPosition()
    {

        GameObject position = null; 
        foreach (Constellation constellation in constellations)
        {
            // reset this next constellation bc the previous just complete
            if(allFull)
            {
                constellation.EmptyAllPositions();
                allFull = false;
            }

            // only check for positions if the constellation has positions
            if (!constellation.IsFull())
            {
                position = constellation.GetNextEmptyPosition();

                // only return good positions
                if (position != null)
                {
                    return position;
                }

                // mark full and do all kinds of ui craziness
                constellation.Complete();
                starSpawn.DestroyStars();


                //oscSenderObject.SendOSCCompleteMessage("/constellationfull", 0);
                SoundConstellationFull();
                allFull = true;


            }
        }

        // looping through all constellations yeild no empty constellation positions.  you must have filled them all
            Debug.Log("good news, no empty constellations.  YOU WIN!");


        // cleanup some memory
        starSpawn.DestroyStars();

        // allow first to be re-enabled
        allFull = true;

        // this will return something useful
        return ResetConstellations();

    }

    private void SoundConstellationFull()
    {
        Debug.Log("doing something????????");
        if (null != soundManager.constellationFull)
        {
            AudioSource.PlayClipAtPoint(soundManager.constellationFull, gameObject.transform.position);
            Debug.Log(" constellation script - full ");
        }
    }


    private GameObject ResetConstellations()
    {
        ((Constellation)constellations.ToArray()[0]).EmptyAllPositions();

        return ((Constellation)constellations.ToArray()[0]).GetNextEmptyPosition();
    }

}

public class Constellation
{
    private GameObject constellation;
    private List<GameObject> positions;
    private string constellationName;
    private GameObject complete;

    public Constellation(GameObject gameObject, List<GameObject> children, string name)
    {
        this.constellation = gameObject;
        this.positions = children;
        this.constellationName = name;        
    }

    public GameObject GetNextEmptyPosition()
    {
           
        foreach (GameObject position in positions)
        {
            if(!position.activeSelf)
            {
                position.SetActive(true);
                return position;
            }
        }

        return null;
    }


    public bool IsFull()
    {
        return !constellation.activeSelf;
    }

    internal void EmptyAllPositions()
    {
        foreach (GameObject position in positions)
        {
            position.SetActive(false);
        }
        constellation.SetActive(true);
    }

    internal void Complete()
    {
        // find and set text and activate
        GameObject canvas = GameObject.Find("Canvas-LogoInstructions");
        foreach (Transform child in canvas.transform)
        {
            if (child.tag == "CONSTELLATION_COMPLETE")
            {
                Debug.Log("Found : " + child.tag + ":: complete text obj for " + constellationName);

                GameObject textObj = child.gameObject;
                Text text = textObj.GetComponent<Text>();
                text.text = "☄️ " + constellationName + " COMPLETE!";
                textObj.SetActive(true);

            }

        }

        constellation.SetActive(false);

    }
}