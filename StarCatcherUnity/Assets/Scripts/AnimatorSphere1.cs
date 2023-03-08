using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSphere1 : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    // Start is called before the first frame update
    void Start()
    {
        myAnimationController.SetBool("sphereMoveDiagonal1", true);
    }

    // Update is called once per frame
    void Update()
    {
       //Debug.Log(myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime); 
       if (myAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            //Debug.Log("diagonal sphere return");
            myAnimationController.SetBool("sphereMoveDiagonal1", false);
       }
    }
}
