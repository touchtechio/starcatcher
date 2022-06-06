using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public MusicStemCrossfade[] flourishingStems, decayingStems, rejuvenationStems;

    private Score.GameState currentState = Score.GameState.Dead;

    private void Update()
    {
        if (Score.plasmaWorldState != currentState)
        {
            switch(Score.plasmaWorldState)
            {
                case Score.GameState.Dead:
                    StartDeadZone();
                    break;
                case Score.GameState.Dying:
                    //remove brighter layers of the decline music
                    decayingStems[1].FadeOutAudio();
                    break;
                case Score.GameState.Decline:
                    StartDecayingStems();
                    break;
                case Score.GameState.Rejuvination:
                    StartRejuvinationStems();
                    break;
                case Score.GameState.Flourishing:
                    StartFlourishingStems();
                    break;
            }
            currentState = Score.plasmaWorldState;
        }
    }

    public void StartFlourishingStems()
    {
        StopStemGroup(rejuvenationStems);
        StopStemGroup(decayingStems);

        StartStemGroup(flourishingStems);
    }

    public void StartDecayingStems()
    {
        StopStemGroup(flourishingStems);
        StopStemGroup(rejuvenationStems);

        StartStemGroup(decayingStems);
    }

    public void StartDyingStems()
    {
        
    }

    public void StartDeadZone()
    {
        //stopping all
        StopStemGroup(flourishingStems);
        StopStemGroup(rejuvenationStems);
        StopStemGroup(decayingStems);

    }

    public void StartRejuvinationStems()
    {
        StopStemGroup(flourishingStems);
        StopStemGroup(decayingStems);

        StartStemGroup(rejuvenationStems);
    }


    //private methods
    void StopStemGroup(MusicStemCrossfade[] musicGroup)
    {
        foreach (MusicStemCrossfade xFade in musicGroup)
        {
            xFade.FadeOutAudio();
        }
    }

    void StartStemGroup(MusicStemCrossfade[] musicGroup)
    {
        foreach (MusicStemCrossfade xFade in musicGroup)
        {
            xFade.FadeInAudio();
        }
    }

}
