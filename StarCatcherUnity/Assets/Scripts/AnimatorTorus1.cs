using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTorus1 : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    // Start is called before the first frame update
    void Start()
    {
        myAnimationController.SetBool("torusDrop", true);
    }

    // Update is called once per frame
    void Update()
    {
       //Debug.Log(myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime); 
       if (myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2) {
           // Debug.Log("torus return");
            myAnimationController.SetBool("torusDrop", false);
       }
    }
}
