using System.Collections;
using UnityEngine;

public class DeadStarPositionColliders : MonoBehaviour
{

    public GameObject deadStarPositionPrefab;

    public GameObject deadStars; 

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


        deadStars = GameObject.FindGameObjectWithTag("DEAD STARS");
        if (null == deadStars)
        {
            Debug.Log("ERROR: no DEAD STARS found. need a tagged GameObject");
        }

        // start game with dead stars ? 
        //UpdateDeadStarPositionColliders();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDeadStarPositionColliders() {

        ArrayList starStrips = stripPositions.getStarPositions();
        int positionCount = starStrips.Count;
        Debug.Log("TRACE: found stripPositions count: " + positionCount);

        if (positionCount == 0)
        {
            Debug.Log("ERROR: no starStrips defined yet");
        }

        for (int i = 0; i < positionCount; i++) {
            Strip strip = (Strip)starStrips[i];
            Vector3 starPosition = strip.starStartPoints;
            GameObject star = Instantiate(deadStarPositionPrefab, starPosition, Quaternion.identity) as GameObject;
            star.transform.parent = deadStars.transform;
            DeadStarCollider deadStarCollider = star.GetComponent<DeadStarCollider>();
            deadStarCollider.caughtStripNumber = i; 
        }    
    }

    
    public void DestroyDeadStars()
    {
        foreach (Transform child in deadStars.transform)
        {
            Destroy(child.gameObject);
        }
        return;
    }

}
