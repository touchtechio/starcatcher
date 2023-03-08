using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPositionColliders : MonoBehaviour
{

    public GameObject starPositionPrefab;

    public StripPosition stripPositions;


    // Start is called before the first frame update
    void Start()
    {
        stripPositions = StripPosition.FindObjectOfType<StripPosition>();
        if (null == stripPositions)
        {
            Debug.Log("ERROR: no StripPosition found");
        }
        Debug.Log("TRACE: found stripPositions", stripPositions);

        UpdateStarPositionColliders();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateStarPositionColliders() {

        ArrayList starStrips = stripPositions.getStarPositions();
        int positionCount = starStrips.Count;
        Debug.Log("TRACE: found stripPositions count: " + positionCount);

        if (positionCount == 0)
        {
            Debug.Log("ERROR: no starStrips defined yet");
        }

        GameObject parentStartPositions = GameObject.FindGameObjectWithTag("STARTING POSITIONS");
        if (null == parentStartPositions)
        {
            Debug.Log("ERROR: no STARTING POSITIONS found. need a tagged GameObject");
        }

        for (int i = 0; i < positionCount; i++) {
            Strip strip = (Strip)starStrips[i];
            Vector3 starPosition = strip.starStartPoints;
            GameObject star = Instantiate(starPositionPrefab, starPosition, Quaternion.identity) as GameObject;
            star.transform.parent = parentStartPositions.transform;
            StripIndex index = star.GetComponent<StripIndex>();
            index.stripIndex = i; 
        }    
    }
}
