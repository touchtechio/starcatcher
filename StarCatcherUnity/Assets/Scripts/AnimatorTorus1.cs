using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTorus1 : MonoBehaviour, IFormationAnimation
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
        Animate();
      }

    }

    // function to trigger animator, that can be called from the score script
    public void Animate() {
        //  Debug.Log("trigger torus animation");
         myAnimationController.SetTrigger("triggerMove");
    } 

    public void Reset() {
      // Debug.Log("trigger sphere animation");
        myAnimationController.ResetTrigger("triggerMove");
   }
}
