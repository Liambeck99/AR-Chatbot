using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    private bool modelSelected;

    public GameObject yBotModel;
    public GameObject xBotModel;

    // Start is called before the first frame update
    void Start()
    {
        // get the animation component in the scene
        animator = GetComponent<Animator>();

        // find models in the scene
        yBotModel = GameObject.Find("ybot");
        xBotModel = GameObject.Find("xbot");

        yBotModel.transform.position = yBotModel.transform.position + new Vector3(0, 0, 1);
        xBotModel.transform.position = xBotModel.transform.position + new Vector3(0, 0, -2);

        // set model selected and switch off other model
        modelSelected = true;
        showModel(modelSelected);

    }

    // Toggles between the displayed models
    private void showModel(bool selected)
    {
        // show Y Bot
        if (selected)
        {
            yBotModel.transform.position = yBotModel.transform.position + new Vector3(0, 0, -2);
            xBotModel.transform.position = xBotModel.transform.position + new Vector3(0, 0,  2);
        }

        // show X Bot
        if (!selected)
        {
            yBotModel.transform.position = yBotModel.transform.position + new Vector3(0, 0,  2);
            xBotModel.transform.position = xBotModel.transform.position + new Vector3(0, 0, -2);
        }

        // invert value of sleectd
        modelSelected = !selected;
    }

    // Button to call model switch function
    public void ToggleModel()
    {
        showModel(modelSelected);
    }

    // Toggle animations
    public void ToggleAnimation()
    {
        bool Walking = animator.GetBool("isWalking");
        bool Dancing = animator.GetBool("isDancing");

        if (!Walking && !Dancing)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isDancing", false);
            return;
        }

        if (Walking)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isDancing", true);
            return;
        }

        if (Dancing)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isDancing", false);
            return;
        }
    }

}
