using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class DeathNarrationManager : MonoBehaviour
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
    private Score scoreObject; // do we need this?? 
    private bool startedEndTimer = false;
    private float endStartTime;
    private bool hasDeathStarted = false;

    // public int CaughtStarCountToAdvanceScene = 10;


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
        if (hasDeathStarted && (Time.time >= startTime + narrationStems[0].sourceToCrossfade.clip.length)) {
            Debug.Log("clip length " + narrationStems[0].sourceToCrossfade.clip.length);
            Debug.LogWarning("Trigger star and clip");
            DeadStarPositionColliders deadStarPositionCollider = FindObjectOfType<DeadStarPositionColliders>();
            deadStarPositionCollider.UpdateDeadStarPositionColliders();
            TriggerSpawnFaintStarAnimation(); //TODO: look into the trigger narration and formation with index
        }


    
    }

    // this starts the Death Narration from the score / game mode transistion
    public void TriggerDeath() {
        Debug.Log("Death getting triggered");
        hasDeathStarted = true;
        startTime = Time.time;
        TriggerNarration(0);

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

    private void TriggerSpawnFaintStarAnimation() {
        // Debug.Log("trigger tutorial animation");
        hasIntroEnded = true;
        startTime = Time.time;
        starFallTime = starSpawnObject.TutorialFallDuration + starSpawnObject.TutorialLingerTime;

        animatorSphere2.Animate();
        // if star caught, Spawn star

        // At the end of star fall time, Spawn again
    }


}
