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
        // update ybot renders
        Renderer[] ybotRender = yBotModel.GetComponentsInChildren<Renderer>();
        for(int i=0; i<ybotRender.Length; i++)
        {
            ybotRender[i].enabled = selected;
        }

        // update xbot renders
        Renderer[] xbotRender = xBotModel.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < xbotRender.Length; i++)
        {
            xbotRender[i].enabled = !selected;
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
