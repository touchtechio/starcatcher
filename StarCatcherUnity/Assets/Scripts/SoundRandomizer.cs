using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// plays a random audio clip from a list
/// </summary>
public class SoundRandomizer : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    public bool stopCurrentSound = false;

    public bool playOnAwake = false;

    AudioSource attachedSource;

    private void Awake()
    {
        attachedSource = GetComponent<AudioSource>();

        if (playOnAwake) PlaySound();
    }


    public void PlaySound(AudioSource aSource = null)
    {
        if (aSource == null)
        {
            aSource = GetComponent<AudioSource>();
        }

        if (!stopCurrentSound && aSource.isPlaying) return;

        aSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        aSource.Play();
    }
}
