using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSphere2 : MonoBehaviour, IFormationAnimation
{
    [SerializeField] private Animator myAnimationController;
    // Start is called before the first frame update
     [SerializeField] private Transform Tutorial1Parent;
     [SerializeField] private Transform Tutorial2Parent;
     [SerializeField] private Transform Tutorial3Parent;
     [SerializeField] private Transform Tutorial4Parent;
     private List<Transform> parentPositions = new List<Transform>();

    // Start is called before the first frame update
    public void Start() {
      // Instance = this;
      parentPositions.Add(Tutorial1Parent);
      parentPositions.Add(Tutorial2Parent);
      parentPositions.Add(Tutorial3Parent);
      parentPositions.Add(Tutorial4Parent);
       // myAnimationController.SetBool("sphereMoveDiagonal1", true);
    }

    // Update is called once per frame
    void Update()
    {

      if (Input.GetKeyDown(KeyCode.Alpha9)) {
         Debug.Log("trigger sphere animation");
         Animate();
      }
    }
   public void Animate() {
      // Debug.Log("Random section selection");
      // chose one of the parent posisiont and do animation on that
      int randomNumber = Random.Range(0, parentPositions.Count);
      Transform aSectionParent = parentPositions[randomNumber];
      transform.SetParent(aSectionParent, false);
      // in the script on the animating object the type of spawn is determined
         myAnimationController.SetTrigger("TriggerMove");
   
   }
 

   public void Reset() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.ResetTrigger("TriggerMove");
   }

   // public void TriggerFaintStarAnimation(){

   // }
   public float GetAnimationLength() {
      return myAnimationController.runtimeAnimatorController.animationClips[0].length * 2; // myAnimationController.speed;
   }
}
