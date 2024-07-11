using System.Collections;
using UnityEngine;

public class DeadStarPositionColliders : MonoBehaviour
{

    public GameObject deadStarPositionPrefab;

    public GameObject deadStars; 

    public StripPosition stripPositions;
    public StarSpawn starSpawn;



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
        MakeDeadStarPositionColliders();

    }

    // Update is called once per frame
    void Update()
    {
        updateDeadStarsOnKeyPress();
    }

    public void updateDeadStarsOnKeyPress() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key was pressed.");
            KillDeadStarPositionColliders();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.LogWarning("D key was pressed.");
            MakeDeadStarPositionColliders();
        }
    }

    public void KillDeadStarPositionColliders() {
        for (var i = deadStars.transform.childCount - 1; i >= 0; i--)
            Destroy(deadStars.transform.GetChild(i).gameObject);
    }


    public void UpdateDeadStarPositionColliders(int stripIndex, int durationMS) {
        // get positions of led strips
        ArrayList starStrips = stripPositions.getStarPositions();
        int positionCount = starStrips.Count;
        //Debug.Log("TRACE: found stripPositions count: " + positionCount);

        if (positionCount == 0)
        {
            Debug.Log("ERROR: no starStrips defined yet");
        }

        if (stripIndex >= positionCount)
        {
            Debug.Log("ERROR:stripIndex arrayindex yet");
        }

        Strip strip = (Strip)starStrips[stripIndex];
        Vector3 starPosition = strip.starStartPoints;

        // Create Dead Star from prefab
        GameObject star = Instantiate(deadStarPositionPrefab, starPosition, Quaternion.identity) as GameObject;
        star.transform.parent = deadStars.transform;

        // configuring the dead star collider with star index. 
        // wher you tell the sstar which led strip to control ..

        DeadStarCollider deadStarCollider = star.GetComponent<DeadStarCollider>();
        deadStarCollider.plasmaStripNumber = stripIndex; 

        ReturnStarDestroyer starDestroyer = star.GetComponent<ReturnStarDestroyer>();
        if (starDestroyer != null)
        {
            starDestroyer.timeToDestroyStar = durationMS / 1000;    //timeToDestroyStar in seconds.
        }
    }

    public void MakeDeadStarPositionColliders() {

        // get positions of led strips
        ArrayList starStrips = stripPositions.getStarPositions();
        int positionCount = starStrips.Count;
        Debug.Log("TRACE: found stripPositions count: " + positionCount);


        if (positionCount == 0)
        {
            Debug.Log("ERROR: no starStrips defined yet");
        }


        // TODO: read from current blue star formations

        for (int i = 0; i < positionCount; i++) {
        // foreach ( int i in starSpawn.plasmaArray) {

            Strip strip = (Strip)starStrips[i];
            Vector3 starPosition = strip.starStartPoints;

            // Create Dead Star from prefab
            GameObject star = Instantiate(deadStarPositionPrefab, starPosition, Quaternion.identity) as GameObject;
            star.transform.parent = deadStars.transform;

            // configuring the dead star collider with star index. 
            // where you tell the star which led strip to control ..

            DeadStarCollider deadStarCollider = star.GetComponent<DeadStarCollider>();
            deadStarCollider.plasmaStripNumber = i; 
        }    

    }

    
    public void DestroyDeadStars()
    {
        foreach (Transform child in deadStars.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
