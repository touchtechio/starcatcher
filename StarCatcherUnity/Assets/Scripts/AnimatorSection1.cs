using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSection1 : MonoBehaviour, IFormationAnimation
{
     
     [SerializeField] private Animator myAnimationController;
     [SerializeField] private Transform section1Parent;
     [SerializeField] private Transform section2Parent;
     [SerializeField] private Transform section3Parent;
     [SerializeField] private Transform section4Parent;
     private List<Transform> parentPositions = new List<Transform>();

    // Start is called before the first frame update
    public void Start() {
      // Instance = this;
      parentPositions.Add(section1Parent);
      parentPositions.Add(section2Parent);
      parentPositions.Add(section3Parent);
      parentPositions.Add(section4Parent);
    }
    public void Update()
    {
        
      if (Input.GetKeyDown(KeyCode.Alpha8)) {
         Debug.Log("trigger section animation");
         // myAnimationController.SetTrigger("triggerMove");
         Animate();
      }
   }

   public void AnimateWithRandomPosition() {
      Debug.Log("Random section selection");
      // chose one of the parent posisiont and do animation on that
      int randomNumber = Random.Range(0, parentPositions.Count);
      Transform aSectionParent = parentPositions[randomNumber];
      transform.SetParent(aSectionParent, false);
      Animate();
         
   }
   public void Animate() {
      Debug.Log("trigger section 1 animation");
         myAnimationController.SetTrigger("triggerMove");
   }
      
   public void Reset() {
      Debug.Log("trigger section 1 reset");
      myAnimationController.ResetTrigger("triggerMove");
   }

   public float GetAnimationLength() {
      return myAnimationController.runtimeAnimatorController.animationClips[0].length * 3; // myAnimationController.speed;
   }  
}

