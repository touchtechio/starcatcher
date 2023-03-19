using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSphere2 : MonoBehaviour
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
         myAnimationController.SetTrigger("TriggerMove");

      //  //Debug.Log(myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime); 
      //  if (myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
      //       //Debug.Log("diagonal sphere return");
      //       myAnimationController.SetBool("sphereMoveDiagonal1", false);
      //  }

         TriggerTutorialSectionAnimation();
    }
}
   public void TriggerTutorialSectionAnimation() {
      // Debug.Log("Random section selection");
      // chose one of the parent posisiont and do animation on that
      int randomNumber = Random.Range(0, parentPositions.Count);
      Transform aSectionParent = parentPositions[randomNumber];
      transform.SetParent(aSectionParent, false);
      TriggerSphere2Animation();
   
   }
   public void TriggerSphere2Animation() {
         // Debug.Log("trigger tutorial sphere animation");
         myAnimationController.SetTrigger("TriggerMove");
   }

   public void ResetSphere2Animation() {
      // Debug.Log("trigger sphere animation");
         myAnimationController.ResetTrigger("TriggerMove");
   }
}
