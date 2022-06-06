using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour {

    private AudioSource source;
    public AudioClip starCaught;
    public AudioClip constellationFull;

    [Header("Plant Audio")]
    public AudioMixerGroup plantSoundGroup;
    public AudioClip[] growClips, swayClips, dieClips;

    

    public GameObject audioPrefab;


    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
    }

    public void PlayGrowSound(Vector3 pos)
    {
        PlaySoundFromArray(growClips, pos, plantSoundGroup);
    }

    public void PlaySwaySound(Vector3 pos)
    {
        PlaySoundFromArray(swayClips, pos, plantSoundGroup);
    }

    public void PlayPlantDieSound(Vector3 pos)
    {
        PlaySoundFromArray(dieClips, pos, plantSoundGroup);
    }
	
	public void PlaySoundFromArray(AudioClip[] clips, Vector3 posToPlay, AudioMixerGroup groupToUse)
    {
        GameObject newSound = Instantiate(audioPrefab, posToPlay, Quaternion.identity);
        newSound.name = "PlantGrowSound";

        AudioSource aSource = newSound.GetComponent<AudioSource>();
        aSource.clip = clips[Random.Range(0, clips.Length)];

        if (groupToUse != null) aSource.outputAudioMixerGroup = groupToUse;

        aSource.Play();

        Destroy(newSound, aSource.clip.length);

    }
}
