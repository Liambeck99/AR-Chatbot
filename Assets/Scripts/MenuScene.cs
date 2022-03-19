// Script that is attached to the menu scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : BaseUIScene
{
    private void Start()
    {
        LoadSettings();

        SetFade();
        SetFadeInSpeed(0.33f);
        UpdateColoursIfColourBlindMode();
    }

    private void Update()
    {
        UpdateFade();
    }

    // Executes when the user clicks on the 'ask a question' button
    public void OnAskQuestionClick()
    {
        // Switches to AR scene if the user has not yet completed the tutorial
        if (!currentSettings.ReturnFieldValue("completeTutorial"))
            SceneManager.LoadScene("AR"); 
        // Switches to AR scene if the user has ticked to automatically use AR in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAR"))
            SceneManager.LoadScene("AR");
        // Else switches to avatar scene if the user has ticked to automatically use avatar in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAvatar"))
            SceneManager.LoadScene("Avatar");
        // Otherwise, switch to the chatbot scene 
        else
            SceneManager.LoadScene("Chatbot");
    }

    // Loads info scene
    public void OnInfoClick()
    {
        SceneManager.LoadScene("Info");
    }

    // Used for loading a test scene (good for quickly testing scenes and features)
    public void OnTestClick()
    {
        
    }

    // Loads mapping scene
    public void OnMappingClick()
    {
        SceneManager.LoadScene("Mapping");
    }

}