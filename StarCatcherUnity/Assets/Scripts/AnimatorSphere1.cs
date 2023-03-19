using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSphere1 : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    // Start is called before the first frame update
    void Start()
    {
       // myAnimationController.SetBool("sphereMoveDiagonal1", true);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha6)) {
         Debug.Log("trigger sphere animation");
         myAnimationController.SetTrigger("triggerMove");

      //  //Debug.Log(myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime); 
      //  if (myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
      //       //Debug.Log("diagonal sphere return");
      //       myAnimationController.SetBool("sphereMoveDiagonal1", false);
      //  }


    }
}
   public void TriggerSphere1Animation() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.SetTrigger("triggerMove");
   }

   public void ResetSphere1Animation() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.ResetTrigger("triggerMove");
   }
}
