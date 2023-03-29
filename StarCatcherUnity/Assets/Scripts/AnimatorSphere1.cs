using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSphere1 : MonoBehaviour, IFormationAnimation
{
    [SerializeField] private Animator myAnimationController;

   void Update()
   {

      if (Input.GetKeyDown(KeyCode.Alpha6)) {
         Debug.Log("trigger sphere animation");
         Animate();
      }
   }
   public void Animate() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.SetTrigger("triggerMove");
   }

   public void Reset() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.ResetTrigger("triggerMove");
   }
}
