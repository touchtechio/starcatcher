using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class NarrationManager : MonoBehaviour
{

    public Text text;
    public StarSpawnBase starSpawnObject;
    public MusicStemCrossfade[] narrationStems;
    private float startTime;
    public string[] narrationText;
    private float starFallTime;
    public AnimatorSphere2 animatorSphere2;
    private bool hasIntroEnded = false;
    public AudioMixerSnapshot tutorialSnapshot;
    private Score scoreObject;
    private bool startedEndTimer = false;
    private float endStartTime;

    public int CaughtStarCountToAdvanceScene = 10;


    // Start is called before the first frame update
    void Start()
    {
        tutorialSnapshot.TransitionTo(0.0f);
        scoreObject = FindObjectOfType<Score>();

        //StarSpawnObject.StartRandomStars();
        // Debug.Log("spawn");
        // TriggerSpawnAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0)) {
            TriggerAnimation(0);
            // TriggerNarration(0);
        }
        if(Input.GetKeyDown(KeyCode.Keypad1)) {
            TriggerAnimation(0);
            // TriggerNarration(1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("KEY PRESS: S");
            Debug.Log("Score = " + scoreObject.starCaughtCount);
        }

        // also check if star caught
        if (hasIntroEnded && (Time.time >= (startTime + starFallTime))) {
            TriggerSpawnAnimation();
        }

        // Start ending narrative and then change scene
        if (scoreObject.starCaughtCount >= CaughtStarCountToAdvanceScene) {
            if (startedEndTimer == false) { 
                Debug.Log(scoreObject.starCaughtCount);
                TriggerNarration(1);
                endStartTime = Time.time;
                startedEndTimer = true;
            }
        }

        // change to main game
        if (startedEndTimer && (Time.time >= endStartTime + narrationStems[1].sourceToCrossfade.clip.length)) {
            Debug.Log("clip length " + narrationStems[1].sourceToCrossfade.clip.length);
            Debug.LogWarning("CHANGE SCENE not yet impl");
            SceneTrigger sceneTrigger = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SceneTrigger>();
            sceneTrigger.SwitchToGameScene();
        }
    
    }

    public void TriggerNarration(int triggerItemIndex) {
        Debug.Log("TriggerNarration() w triggerItemIndex: " + triggerItemIndex);

        if ((narrationStems.Length < triggerItemIndex) || (narrationText.Length < triggerItemIndex)){
            Debug.LogError("NOT enough text or narration audio for triggerItemIndex: " + triggerItemIndex);
        }

        text.text = narrationText[triggerItemIndex];
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

    private void ManageStarCatchingTutorial(){
        // audio: "here comes some stars"

    }

    private void TriggerSpawnAnimation() {
        // Debug.Log("trigger tutorial animation");
        hasIntroEnded = true;
        startTime = Time.time;
        starFallTime = starSpawnObject.TutorialFallDuration + starSpawnObject.TutorialLingerTime;
        animatorSphere2.TriggerTutorialSectionAnimation();
        // if star caught, Spawn star

        // At the end of star fall time, Spawn again
    }


}
