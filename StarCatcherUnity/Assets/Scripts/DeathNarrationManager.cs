using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UniOSC;


public class DeathNarrationManager : MonoBehaviour
{

    public Text text;
    public StarSpawnBase starSpawnObject;
    public MusicStemCrossfade[] narrationStems;
    private float startTime;
    public string[] narrationText;
    private float starFallTime;
    public AnimatorSphere2 animatorSphere2;
    private bool hasTriggerSpawnFaintStarAnimation = false;
    private bool isReadyForOtherStars = false;
    private bool hasReturnedOne = false;
    public AudioMixerSnapshot tutorialSnapshot;
    private Score scoreObject; 
    private bool startedEndTimer = false;
    private float endStartTime;
    private bool hasDeathStarted = false;

    public OSCSenderAllFall sendAllFall;
    public OSCSenderAllPulsingLinger sendPulsingLinger;



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
        if (hasDeathStarted && (Time.time >= startTime + narrationStems[0].sourceToCrossfade.clip.length - 8f)) {
            // Debug.Log("clip length " + narrationStems[0].sourceToCrossfade.clip.length);
            TriggerSpawnFaintStarAnimation(); 
        }

        if (isReadyForOtherStars && hasReturnedOne && (Time.time >= startTime + 20f)) {
            TriggerReadyToReturnOtherStars();
        }


    
    }

    // this starts the Death Narration from the score / game mode transistion
    public void TriggerDeath() {
        Debug.Log("Death getting triggered");
        hasDeathStarted = true;
        hasTriggerSpawnFaintStarAnimation = false;
        startTime = Time.time;
        DeadStarPositionColliders deadStarPositionCollider = FindObjectOfType<DeadStarPositionColliders>();
        deadStarPositionCollider.UpdateDeadStarPositionColliders();
        TriggerNarration(0);

    }

    private void TriggerReadyToReturnOtherStars() {
            TriggerNarration(1);
            isReadyForOtherStars = false;
            Debug.Log("SENDING all to pulse in two messages ");
            sendPulsingLinger.SendOSCAllPulsingLingerMessage("/behavior/4");
            sendAllFall.SendOSCAllFallMessage("/allfall");
    }


    public void TriggerFirstVoyage() {
          Debug.Log("Excited getting triggered");
          TriggerNarration(2);
    }

    public void TriggerPlantDying() {
        Debug.Log("plant dying getting triggered");
        TriggerNarration(3);

    }

    public void TriggerOuterRing() {
        Debug.Log("outer ring of planets getting triggered");
        TriggerNarration(4);

    }

    public void TriggerSnowDwarf() {
        Debug.Log("white dwarf triggered");
        TriggerNarration(5);

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

    public void TriggerReturnOtherStars () {
        hasReturnedOne = true;
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

    private void TriggerSpawnFaintStarAnimation() {
        if (!hasTriggerSpawnFaintStarAnimation) {
            hasTriggerSpawnFaintStarAnimation = true;
            startTime = Time.time;
            isReadyForOtherStars = true; // setup for next audio cue
            hasReturnedOne = false;
            starFallTime = starSpawnObject.TutorialFallDuration + starSpawnObject.TutorialLingerTime;
            animatorSphere2.Animate();
        }

    }


}
