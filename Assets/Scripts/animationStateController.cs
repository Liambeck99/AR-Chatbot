using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // get the animation component in the scene
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    // Toggle animations
    public void ToggleAnimation()
    {
        bool current = animator.GetBool("isWalking");
        animator.SetBool("isWalking", !current);
    }

}
