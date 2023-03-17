using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrationManager : MonoBehaviour
{

    public Text text;

    public StarSpawn StarSpawn;
    public MusicStemCrossfade[] narrationStems;


    public string[] narrationText;


    // Start is called before the first frame update
    void Start()
    {
        
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

}
