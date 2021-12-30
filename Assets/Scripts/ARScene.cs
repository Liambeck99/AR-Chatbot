using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARScene : BaseUIScene
{
    private int tutorialPhase;
    private int maxTutorialPhases;

    private GameObject[] tutorialElements;

    private void Start()
    {
        tutorialPhase = 0;
        maxTutorialPhases = GameObject.Find("Tutorial").transform.childCount;

        tutorialElements = new GameObject[maxTutorialPhases];

        for (int i=0; i < maxTutorialPhases; i++)
            tutorialElements[i] = (GameObject.FindGameObjectsWithTag("Tutorial" + (i+1).ToString())[0]);

        if (currentSettings.ReturnFieldValue("completeTutorial"))
            ToggleTutorial(false);
        else
            ToggleTutorial(true);
    }

    private void Update()
    {
        
    } 

    public void UpdateTutorial()
    {
        if (tutorialPhase == maxTutorialPhases-1)
        {
            ToggleTutorial(false);
            currentSettings.UpdateField("completeTutorial", true);
            currentSettings.WriteJson();
        }
        else if (tutorialPhase < maxTutorialPhases-1)
        {
            tutorialElements[tutorialPhase].SetActive(false);
            tutorialPhase++;
            tutorialElements[tutorialPhase].SetActive(true);
        }
    }

    private void ToggleTutorial(bool activate)
    {
        GameObject TutorialButton = GameObject.Find("TutorialButton");
        TutorialButton.SetActive(activate);

        GameObject TutorialContainer = GameObject.Find("Tutorial");
        TutorialContainer.SetActive(activate);

        if (activate)
        {
            tutorialElements[0].SetActive(true);
            for(int i=1; i < maxTutorialPhases; i++)
                tutorialElements[i].SetActive(false);
        }
    }

    public void GoToAvatarIfTutorialFinished()
    {
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnAvatarClick();
    }

    public void GoToChatbotIfTutorialFinished()
    {
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnChatbotClick();
    }
}

