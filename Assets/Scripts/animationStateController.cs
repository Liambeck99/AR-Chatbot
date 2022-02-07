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

        // set model selected and switch off other model
        modelSelected = false;
        showModel(modelSelected);

    }

    // Toggles between the displayed models
    private void showModel(bool selected)
    {
        // show Y Bot
        if (selected)
        {
            yBotModel.transform.position = new Vector3(0, 0, 0);
            xBotModel.transform.position = new Vector3(0, 0, -2);
        }

        // show X Bot
        if (!selected)
        {
            yBotModel.transform.position = new Vector3(0, 0, -2);
            xBotModel.transform.position = new Vector3(0, 0, 0);
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
        bool current = animator.GetBool("isWalking");
        animator.SetBool("isWalking", !current);
    }

}
