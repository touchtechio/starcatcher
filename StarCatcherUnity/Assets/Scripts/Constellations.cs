using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constellations : MonoBehaviour {

    public List<Constellation> constellations = new List<Constellation>();
    private bool full = false;

    // Use this for initialization
    void Start () {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("CONSTELLATION");
        if (objs.Length == 0) {
            Debug.Log("ERROR: no constellations defined");
        }

        for (int i = 0; i < objs.Length; i++)
        {

            List<GameObject> Children = new List<GameObject>();
            foreach (Transform child in objs[i].transform)
            {
                if (child.tag == "CONSTELLATION_POSITION")
                {
                    child.gameObject.SetActive(false);
                    Children.Add(child.gameObject);
                    Debug.Log("Found : " + child.tag + ":: position: " + child.transform.localPosition);

                }
            }

            constellations.Add(new Constellation(objs[i], Children, objs[i].name));

        }

        
    }

    public GameObject GetNextEmptyPosition()
    {
        if (full)  return null;

        GameObject position = null; 
        foreach (Constellation constellation in constellations)
        {
            if ( ! constellation.IsFull())
            {
                position = constellation.GetNextEmptyPosition();
                if (position != null)
                {
                    return position;
                }
            } 
        }

        if (position == null)
        {
            full = true;
        }

        return null;

    }


}

public class Constellation
{
    private GameObject constellation;
    private List<GameObject> positions;
    private string constellationName;
    bool full = false;

    public Constellation(GameObject gameObject, List<GameObject> children, string name)
    {
        this.constellation = gameObject;
        this.positions = children;
        this.constellationName = name;
    }

    public GameObject GetNextEmptyPosition()
    {
        if (full) return null;

        
        foreach (GameObject position in positions)
        {
            if(!position.activeSelf)
            {
                position.SetActive(true);
                return position;
            }
        }

        full = true;
        Debug.Log(constellationName + "is full");
        return constellation;
    }

    public bool IsFull()
    {

        return full;
    }

}