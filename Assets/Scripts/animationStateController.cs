using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    private bool modelSelected;

    private GameObject xBotModel;

    private int animationPhase;

    private float crossFadeTime = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // find models in the scene
        xBotModel = GameObject.Find("xbot");

        // get the animation component in the scene
        animator = xBotModel.GetComponent<Animator>();

        Debug.Log("animator: " + animator);

        animationPhase = 0;

        setDefaultValues();
    }

    private void setDefaultValues()
    {
        animator.SetBool("isIdle", true);

        animator.SetInteger("randomAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isThinking", false);
    }

    // Toggle animations
    public void ToggleAnimationPhase()
    {
        switch (animationPhase)
        {
            case 0:
                PerformThinkingAnimation();
                break;

            case 1:
                PerformExplinationAnimation();
                break;

            case 2:
                PerformIDLEAnimation();
                break;
        }
    }

    public void PerformIDLEAnimation()
    {
        animator.SetInteger("randomAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isIdle", true);
        animator.CrossFade("Breathing Idle", crossFadeTime);
        animationPhase = 0;
    }

    public void PerformThinkingAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isThinking", true);
        animator.CrossFade("Thinking", crossFadeTime);
        animationPhase = 1;
    }

    public void PerformExplinationAnimation()
    {
        animator.SetBool("isThinking", false);
        animator.SetBool("isExplaining", true);
        animator.CrossFade("Explination", crossFadeTime);
        animationPhase = 2;
    }

    public void PerformRandomAnimationIfIdle()
    {
        int numRandomAnimations = 3;

        // Perform random animation
        if (animationPhase == 0)
        {
            int animationNum = Random.Range(1, numRandomAnimations+1);

            animator.SetInteger("randomAnimation", animationNum);

            switch(animationNum){
                case 1:
                    animator.CrossFade("Laughing", crossFadeTime);
                    break;

                case 2:
                    animator.CrossFade("Hip Hop Dancing", crossFadeTime);
                    break;

                case 3:
                    animator.CrossFade("Booty Hip Hop Dance", crossFadeTime);
                    break;

            }
        }
    }

}
