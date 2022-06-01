using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public MusicStemCrossfade[] flourishingStems, decayingStems, rejuvenationStems;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartFlourishingStems();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartDecayingStems();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            StartDeadZone();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            StartRejuvinationStems();
        }
    }

    public void StartFlourishingStems()
    {
        foreach (MusicStemCrossfade rejuvinationStem in rejuvenationStems)
        {
            rejuvinationStem.FadeOutAudio();
        }

        foreach (MusicStemCrossfade flourishingStem in flourishingStems)
        {
            flourishingStem.FadeInAudio();
        }
    }

    public void StartDecayingStems()
    {
        foreach (MusicStemCrossfade flourishingStem in flourishingStems)
        {
            flourishingStem.FadeOutAudio();
        }

        foreach (MusicStemCrossfade decayingStem in decayingStems)
        {
            decayingStem.FadeInAudio();
        }
    }

    public void StartDeadZone()
    {
        foreach (MusicStemCrossfade decayingStem in decayingStems)
        {
            decayingStem.FadeOutAudio();
        }
    }

    public void StartRejuvinationStems()
    {
        foreach (MusicStemCrossfade rejuvStem in rejuvenationStems)
        {
            rejuvStem.FadeInAudio();
        }
    }

}
