using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetScoreScript : MonoBehaviour
{

    public NetCollider net;

    // Start is called before the first frame update
    void Start()
    {
        if(null == net) {
            Debug.LogWarning("NetScoreScript for UI not initialized with a NetCollider");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(includeInactive:true);
        int score = net.starsCaughtbyNet; 
        for (int i = 0; i < allChildren.Length; i++) {
            allChildren[i].gameObject.SetActive(i <= score);
        }
    }
}
