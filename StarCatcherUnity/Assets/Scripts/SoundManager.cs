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
    public AudioClip[] growClips;
    public AudioClip[] swayClips;
    [Range(0f,1f)] [SerializeField] float swayClipChance = 0.3f;
    public AudioClip[] dieClips;

    public int maxSimultaneousSwaySounds = 2;
    public bool voiceSteal = true;

    public GameObject audioPrefab;

    List<AudioSource> currentlyPlaying;

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        currentlyPlaying = new List<AudioSource>();
    }

    public void PlayGrowSound(Vector3 pos)
    {
        PlaySoundFromArray(growClips, pos, plantSoundGroup);
    }

    public void PlaySwaySound(Vector3 pos)
    {
        if (Random.value > swayClipChance) return;

        PlaySoundFromArray(swayClips, pos, plantSoundGroup, true);
    }

    public void PlayPlantDieSound(Vector3 pos)
    {
        PlaySoundFromArray(dieClips, pos, plantSoundGroup);
    }

    public void PlaySoundFromArray(AudioClip[] clips, Vector3 posToPlay, AudioMixerGroup groupToUse, bool checkMaxSounds = false)
    {
        GameObject newSound = Instantiate(audioPrefab, posToPlay, Quaternion.identity);
        

        AudioSource aSource = newSound.GetComponent<AudioSource>();
        aSource.clip = clips[Random.Range(0, clips.Length)];
        newSound.name = "SFX_" + aSource.clip;

        if (groupToUse != null) aSource.outputAudioMixerGroup = groupToUse;

        if (checkMaxSounds && MaxSoundsReached(aSource.clip, voiceSteal))
        {
            Debug.Log("max sounds reached for " + aSource.clip);
            return;
        }

        aSource.Play();

        currentlyPlaying.Add(aSource);

        StartCoroutine(DestroySoundObject(newSound, aSource.clip.length));

    }

    IEnumerator DestroySoundObject(GameObject objToDestroy, float time)
    {
        yield return new WaitForSeconds(time);

        //may have been destroyed during voice stealing
        if(objToDestroy != null)
        {
            currentlyPlaying.Remove(objToDestroy.GetComponent<AudioSource>());

            Destroy(objToDestroy);
        }
    }

    public bool MaxSoundsReached(AudioClip clipToCheck, bool useVoiceSteal)
    {
        int clipCount = 0;
        AudioSource oldestSource = null;
        for (int i = 0; i < currentlyPlaying.Count; i++)
        {
            if(currentlyPlaying[i].clip.name == clipToCheck.name)
            {
                if(oldestSource == null) 
                {
                    oldestSource = currentlyPlaying[i];
                }
                else if (oldestSource.time < currentlyPlaying[i].time)
                {
                    oldestSource = currentlyPlaying[i];
                }
                clipCount++;
            }
        }

        if (clipCount >= maxSimultaneousSwaySounds)
        {
            //sub-option for voice stealing
            if (voiceSteal)
            {
                oldestSource.Stop();
                currentlyPlaying.Remove(oldestSource);
                Destroy(oldestSource.gameObject);

                return false;
            }

            else return true;
        }

        //if we haven't reached max simultanous sounds, return false
        return false;

    }


}
