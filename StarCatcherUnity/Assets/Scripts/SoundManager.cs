using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour {

    private AudioSource source;
    public AudioClip[] starCaught;
    public AudioClip constellationFull;

    [Header("Mixer Groups")]
    public AudioMixerGroup starFallingGroup; 
    public AudioMixerGroup starCatchingGroup;
    public AudioMixerGroup plantSoundGroup;

    [Space(10)]
    [Header("Transition Sounds")]
    public AudioClip restoreTransition;
    public AudioClip flourishTransition, decayTransition, dyingTransition, deathTransition;

    [Space(10)]
    [Header("Plant Audio")]
    public AudioClip[] growClips;
    public AudioClip[] limbGrowClips;
    [Range(0f, 1f)] [SerializeField] float limbGrowClipChance = 0.2f;
    public AudioClip[] swayClips;
    [Range(0f,1f)] [SerializeField] float swayClipChance = 0.3f;
    public AudioClip[] dieClips;
    public AudioClip[] limbDieClips;
    [Range(0f, 1f)] [SerializeField] float limbDieClipChance = 0.2f;

    [Space(10)]
    [Header("Voice Management")]
    public int maxSimultaneousSwaySounds = 2;
    public bool voiceSteal = true;

    public GameObject audioPrefab;

    List<AudioSource> currentlyPlaying;

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        currentlyPlaying = new List<AudioSource>();
    }

    public void PlayStateTransition()
    {
        if (source.isPlaying) return;

        switch (Score.plasmaWorldState)
        {
            case Score.GameState.Dead:
                source.clip = deathTransition;
                break;
            case Score.GameState.Dying:
                source.clip = dyingTransition;
                break;
            case Score.GameState.Decline:
                source.clip = decayTransition;
                break;
            case Score.GameState.Rejuvination:
                source.clip = restoreTransition;
                break;
            case Score.GameState.Flourishing:
                source.clip = flourishTransition;
                break;
                
        }
        source.Play();
    }

    public void PlayGrowSound(Vector3 pos)
    {
        PlaySoundFromArray(growClips, pos, plantSoundGroup);
    }

    public void PlayLimbGrowSound(Vector3 pos)
    {
        //random chance
        if (Random.value > limbGrowClipChance) return;
        PlaySoundFromArray(limbGrowClips, pos, plantSoundGroup, true);

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

    public void PlayLimbDieSound(Vector3 pos)
    {
        if (Random.value > limbDieClipChance) return;
        PlaySoundFromArray(limbDieClips, pos, plantSoundGroup, true);
    }

    public void PlayStarCaughtSound(Vector3 pos)
    {
        PlaySoundFromArray(starCaught, pos, starCatchingGroup, false);
    }

    public void PlaySoundFromArray(AudioClip[] clips, Vector3 posToPlay, AudioMixerGroup groupToUse, bool checkMaxSounds = false)
    {
        //Check that clips are assigned
        if (clips.Length == 0) return;

        //instantiate game object for sound, set to destroy after it's finished playing
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
