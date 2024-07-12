using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UniOSC;
using System;


public class DeathNarrationManager : MonoBehaviour
{

    public Text text;
    public StarSpawnBase starSpawnObject;
    public MusicStemCrossfade[] narrationStems;
    private float startTime;
    private float starFallTime;
    public string[] narrationText;
    public AnimatorSphere2 animatorSphere2;
    private bool hasTriggerSpawnFaintStarAnimation = false;
    private bool isReadyForOtherStars = false;
    private bool hasReturnedOne = false;
    public AudioMixerSnapshot tutorialSnapshot;
    private Score scoreObject; 
    private bool startedEndTimer = false;
    private float endStartTime;
    private bool hasDeathStarted = false;
    private bool hasTriggeredFirstTime = false;

    public OSCSenderAllFall sendAllFall;
    public OSCSenderAllPulsingLinger sendPulsingLinger;

    private int[] welcomeMessageAudioArray = {0, 1};
    private int[] outerRingStarsAudioArray = {2, 3};
    private int[] snowDwarfAudioArray = {4, 5};
    private int[] degenerateCommentAudioArray = {6, 7, 8, 9, 10};
    private int[] planetDeathAudioArray = {11, 12};
    private int starsReturnedWorked = 13;


    // public int CaughtStarCountToAdvanceScene = 10;


    // Start is called before the first frame update
    void Start()
    {
        tutorialSnapshot.TransitionTo(0.0f);
        scoreObject = FindObjectOfType<Score>();
    }

    // Update is called once per frame
    void Update()
    {
        // sets timer to star Plasma / fainst star animation
        if (hasDeathStarted && (Time.time >= startTime + narrationStems[planetDeathAudioArray[0]].sourceToCrossfade.clip.length - 13f)) {
            // Debug.Log("clip length " + narrationStems[0].sourceToCrossfade.clip.length);
            TriggerSpawnFaintStarAnimation(); 
            hasTriggeredFirstTime = true;
        }

        // Moving animation object to parent requires syncing with animation time 
        // Once animation is done -> check whether to move to different parent
        // Then drop - so animation doesn't loop
        if (hasDeathStarted && hasTriggeredFirstTime) {
            if (Time.time >= startTime +  starFallTime) {
                TriggerSpawnFaintStarAnimation();
            }
        }
    }

    // this starts the Death Narration from the score / game mode transistion
    public void TriggerDeath() {
        Debug.Log("Death getting triggered");
        hasDeathStarted = true;
        hasTriggerSpawnFaintStarAnimation = false;
        startTime = Time.time;
        // TriggerNarration(0);
        // The first time, trigger the new audio, after that keep repeating the same message
        if (Score.starCatcherRevivedCount < 1) {
        TriggerNarration(planetDeathAudioArray[0]);
        }
        else {
            TriggerNarration(planetDeathAudioArray[1]);
        }

    }

    private void TriggerReadyToReturnOtherStars() {
        // narration is the same each time - encouraging
        TriggerNarration(starsReturnedWorked);
        isReadyForOtherStars = false;
        Debug.Log("SENDING all to pulse in two messages ");
        sendPulsingLinger.SendOSCAllPulsingLingerMessage("/behavior/4");
        sendAllFall.SendOSCAllFallMessage("/allfall");
    }


    public void TriggerFirstVoyage() {
        Debug.Log("Excited getting triggered");
        // TriggerNarration(2);
         // The first time, trigger the new audio, after that keep repeating the same message
        if (Score.starCatcherRevivedCount < 1) {
        TriggerNarration(welcomeMessageAudioArray[0]);
        }
        else {
            TriggerNarration(welcomeMessageAudioArray[1]);
        }
    }

    public void TriggerPlantDying() {
        Debug.Log("plant dying getting triggered");
        // Done: this only triggers 10 seconds after enters scene from decline state
        // TriggerNarration(3);
                if (Score.starCatcherRevivedCount < 1) {
        TriggerNarration(degenerateCommentAudioArray[0]);
        }
        else {
            TriggerNarration(degenerateCommentAudioArray[UnityEngine.Random.Range(1,4)]);
        }
    }

    public void TriggerOuterRing() {
        Debug.Log("outer ring of planets getting triggered");
        // TriggerNarration(4);
        // alternate between 2 audios
        TriggerNarration(outerRingStarsAudioArray[(Score.starCatcherRevivedCount % 2)]);
    }

    public void TriggerSnowDwarf() {
        Debug.Log("white dwarf triggered");
        // TriggerNarration(5);
        // alternate between 2 audios
        TriggerNarration(snowDwarfAudioArray[(Score.starCatcherRevivedCount % 2)]);
    }

    // function to trigger the audio clips assigned in the UI
    public void TriggerNarration(int triggerItemIndex) {
        Debug.Log("TriggerNarration() w triggerItemIndex: " + triggerItemIndex);
        if ((narrationStems.Length < triggerItemIndex) || (narrationText.Length < triggerItemIndex)){
            Debug.LogError("NOT enough text or narration audio for triggerItemIndex: " + triggerItemIndex);
        }

        // text.text = narrationText[triggerItemIndex];
        narrationStems[triggerItemIndex].FadeInAudio();
    }

    public void TriggerFormation(int triggerItemIndex) {
        Debug.Log("TriggerNarration() w triggerItemIndex: " + triggerItemIndex);

        if ((narrationStems.Length < triggerItemIndex) || (narrationText.Length < triggerItemIndex)){
            Debug.LogError("NOT enough text or narration audio for triggerItemIndex: " + triggerItemIndex);
        }
        // formationAnimationControllers.SetTrigger("triggerMove");

        // text.text = narrationText[triggerItemIndex];
        // narrationStems[triggerItemIndex].FadeInAudio();
    }

    public void TriggerReturnOtherStars () {
        hasReturnedOne = true;
        Debug.Log("returned 2 stars, start narration");
        TriggerReadyToReturnOtherStars();
    }


    public void TriggerAnimation(int index) {
        Animation[] animations = gameObject.GetComponents<Animation>() as Animation[];
        if(animations.Length < index) {
            Debug.LogError("We have a problem there aren't an animation index: " + index);
        }
        string animationName = animations[index].clip.name;
        Animator animator = gameObject.GetComponent<Animator>();

        Debug.Log("attempting to play: " + animationName); 
        // animator.SetTrigger(animationName);
        animator.Play(animationName);
    }

    // This drops the ball that sends off a signal to the lighting to do a linger for an amount of time
    // TODO: Need to keep the ball down so that the animation stays down. Then change the dead stars to appear only here. 
    private void TriggerSpawnFaintStarAnimation() {
        if (!hasTriggerSpawnFaintStarAnimation) {
            // keep the animation looping
            //hasTriggerSpawnFaintStarAnimation = true;
            isReadyForOtherStars = true; // setup for next audio cue
            // Debug.Log("Is ready for other stars");
            hasReturnedOne = false;
            startTime = Time.time;
            starFallTime = 6f; // match animation time
            animatorSphere2.Animate();
        }
    }

}
