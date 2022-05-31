using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicStemCrossfade : MonoBehaviour
{
    public AudioSource sourceToCrossfade;
    
    public enum CrossfadeType { Linear = 0, ConstantPwr = 1 };
    public CrossfadeType crossfadeType = CrossfadeType.ConstantPwr;

    public float crossfadeTime = 5f;

    public float maxVolume = 1.0f;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            FadeInAudio();
        }
        else if (Input.GetKeyDown(KeyCode.D) && sourceToCrossfade.isPlaying)
        {
            FadeOutAudio();
        }
    }

    /// <summary>
    /// fade an audiosource based on an incoming parameter
    /// to fade out, use 1-parameter
    /// </summary>
    /// <param name="normInput">Input that's normalized from 0-1</param>
    void FadeAudio(float normInput, float _maxVolume)
    {
        if(crossfadeType == CrossfadeType.ConstantPwr)
        {
            normInput *= 2f;
            normInput -= 1f;

            sourceToCrossfade.volume = Mathf.Sqrt(0.5f * (1f + normInput)) * _maxVolume;
        }
        else
        {
            sourceToCrossfade.volume = normInput * _maxVolume;
        }
    }

    void FadeInAudio()
    {
        StopAllCoroutines();
        sourceToCrossfade.volume = 0f;
        sourceToCrossfade.Play();
        StartCoroutine(AudioFadeIn(maxVolume));
    }

    void FadeOutAudio()
    {
        StopAllCoroutines();
        StartCoroutine(AudioFadeOut());
    }

    IEnumerator AudioFadeIn(float destVolume)
    {
        float timer = 0f;
        while (timer < 1f)
        {

            FadeAudio(timer, maxVolume);

            timer += Time.deltaTime / crossfadeTime;

            yield return null;

        }
    }

    IEnumerator AudioFadeOut()
    {
        Debug.Log("fadeout");
        float timer = 0f;
        //crossfade from the current volume, in case this is less than maxVolume
        float startVolume = sourceToCrossfade.volume;
        while (timer < 1f)
        {
            
            FadeAudio(1f-timer, startVolume);

            timer += Time.deltaTime / crossfadeTime;

            yield return null;

        }

        sourceToCrossfade.Stop();
    }

}
