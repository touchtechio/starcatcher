using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VoiceOverManager : MonoBehaviour
{

    public AudioMixerSnapshot mainGameSnapshot;
    public float snapshotTransitionTime = 0f;

    public AudioSource audioSourceIntro;
    public AudioClip clip;
    public float volume=0.5f;
    void Start()
    {
        audioSourceIntro.PlayOneShot(clip, volume);

        mainGameSnapshot.TransitionTo(snapshotTransitionTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
