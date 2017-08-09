using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningForATime : MonoBehaviour {

    public float WinningTime = 5f;
    private float TimeToDie = 0f;

    void OnEnable()
    {
        TimeToDie = WinningTime;
        //Debug.Log("WinningForATime: script was enabled");
    }

    void Update () {
        TimeToDie -= Time.deltaTime;
        if( TimeToDie <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
