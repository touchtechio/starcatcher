using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackingTracks : MonoBehaviour {

    public AudioClip[] levelAudio;
    int i = 0;

	public void UpdateLevel () {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = GetAudioClipForLevel();
        audio.Play();
    }

    AudioClip GetAudioClipForLevel()
    {
        return levelAudio[(i++ % levelAudio.Length)];
    }
}
