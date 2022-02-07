// Script that is attached to the AR scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARScene : BaseSessionScene
{
    // Stores the current phase of the tutorial the user is at
    private int tutorialPhase;
    // Stores the number of steps in the tutorial
    private int maxTutorialPhases;

    // Holds references to all GameObjects used in the tutorial
    private GameObject[] tutorialElements;

    private animationStateController animationController = new animationStateController();

    private void Start()
    {
        ConfigureScene();

        // Tutorial starts at phase 0
        tutorialPhase = 0;
        // The number of phases is the number of child objects from the 'Tutorial' container, 
        // this is set dynamically so that more tutorial phases can be added without having to
        // change this script
        maxTutorialPhases = GameObject.Find("Tutorial").transform.childCount;

        tutorialElements = new GameObject[maxTutorialPhases];

        // Adds all children of 'tutorial' to the references array
        for (int i = 0; i < maxTutorialPhases; i++)
            tutorialElements[i] = GameObject.Find("Tutorial").transform.GetChild(i).gameObject;
        
        // If the user has completed the tutorial, then don't activate the tutorial
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            ToggleTutorial(false);
        // Otherwise, enable the tutorial
        else
            ToggleTutorial(true);
    }

    private void Update()
    {
        UpdateScene();
    }

    // Sets the tutorial to the next phase
    public void UpdateTutorial()
    {
        // If the tutorial is at the last phase, then disable the tutorial and update the settings to not
        // show the tutorial again
        if (tutorialPhase == maxTutorialPhases-1)
        {
            ToggleTutorial(false);
            currentSettings.UpdateField("completeTutorial", true);
            currentSettings.WriteJson();
        }
        // Otherwise, disable the current tutorial phase, increment the counter and activate the next phase of the tutorial
        else if (tutorialPhase < maxTutorialPhases-1)
        {
            tutorialElements[tutorialPhase].SetActive(false);
            tutorialPhase++;
            tutorialElements[tutorialPhase].SetActive(true);
        }
    }

    // Activates/deactivates the tutorial
    private void ToggleTutorial(bool activate)
    {
        // Activates/deactivates the tutorial button, which covers most of the screen that the user clicks on
        // to progress in the tutorial process
        GameObject TutorialButton = GameObject.Find("TutorialButton");
        TutorialButton.SetActive(activate);

        // Activates/deactivates the main tutorial container
        GameObject TutorialContainer = GameObject.Find("Tutorial");
        TutorialContainer.SetActive(activate);

        // Condition for if the tutorial needs to be activated
        if (activate)
        {
            // Activates first phase (as this should be shown first)
            tutorialElements[0].SetActive(true);

            // Deactivates all other phases in the tutorial, as these will be shown incrementally
            for(int i=1; i < maxTutorialPhases; i++)
                tutorialElements[i].SetActive(false);
        }
    }

    // Executes if the user clicks to go to the Avatar scene 
    public void GoToAvatarIfTutorialFinished()
    {
        // User can only switch scenes once the tutorial is complete
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnAvatarClick();
    }

    // Executes if the user clicks to go to the Chatbot scene
    public void GoToChatbotIfTutorialFinished()
    {
        // User can only switch scenes once the tutorial is complete
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnChatbotClick();
    }

    // Executes if the user clicks the keyboard button
    public void GetKeyboardInputIfTutorialFinished()
    {
        // User can only bring up the keyboard input if the tutorial is finished
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnKeyboardClick();
    }

    // Executes if the user clicks the microphone button
    public void GetMicrophoneInputIfTutorialFinished()
    {
        // User can only bring up the microphone input if the tutorial is finished
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnMicroPhoneClick();
    }

    protected override void RenderUserMessage(string message)
    {

    }

    protected override void RenderChatbotResponseMessage(string message)
    {

    }

    // Calls the animation state controller to update animations
    public void onChangeModelButton()
    {
        
    }
}

