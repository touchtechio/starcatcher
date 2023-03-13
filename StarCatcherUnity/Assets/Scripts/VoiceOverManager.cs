using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverManager : MonoBehaviour
{
    public AudioSource audioSourceIntro;
    public AudioClip clip;
    public float volume=0.5f;
    void Start()
    {
        audioSourceIntro.PlayOneShot(clip, volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
