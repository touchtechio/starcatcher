using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private AudioSource source;
    public AudioClip starFall;
    public AudioClip starCaught;
    public AudioClip constellationFull;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        AudioSource.PlayClipAtPoint(starFall, new Vector3(0, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
