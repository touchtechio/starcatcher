using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTorus1 : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    // Start is called before the first frame update
    void Start()
    {
       //myAnimationController.SetBool("torusDrop", true);
    }

    // Update is called once per frame
    void Update()
    {
     if (Input.GetKeyDown(KeyCode.Alpha7)) {
         Debug.Log("trigger torus animation from keypress");
         myAnimationController.SetTrigger("triggerMove");
     }
     //   //Debug.Log(myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime); 
     //   if (myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
     //       // Debug.Log("torus return");
     //        myAnimationController.SetBool("torusDrop", false);
     //   }
    }

    // function to trigger animator, that can be called from the score script
    public void triggerTorus1Animation() {
         Debug.Log("trigger torus animation");
         myAnimationController.SetTrigger("triggerMove");
    } 
}
