using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// The main parent class used for the scenes in the project
// Set to abstract as this class should not be used individually
public abstract class BaseUIScene : MonoBehaviour
{
    // The fade object
    protected CanvasGroup fadeGroup;

    // Speed to reduce fade alpha value (multiplied by time.time if (10 seconds 0.1))
    protected float fadeInSpeed; 

    // Holds current settings
    protected SettingsHandler currentSettings = new SettingsHandler();

    // Initialises fade object
    protected void SetFade()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha = 1;
    }

    // Updates fade alpha value
    protected void UpdateFade()
    {
        // Reduces alpha value to become more translucent(depending on fade in speed)
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;
    }

    // Updates the fade in speed
    protected void SetFadeInSpeed(float newFadeInSpeed)
    {
        fadeInSpeed = newFadeInSpeed;
    }

    // Updates object in the scene if the colour blind setting is used
    protected void UpdateColoursIfColourBlindMode()
    {
        // Checks that the colour blind setting is currently being used
        if (currentSettings.ReturnFieldValue("useColourBlind"))
        {
            // Finds all button objects in the scene
            Button[] buttons = FindObjectsOfType<Button>();

            // Loops through each button object and sets the colour to black. Highlighted and pressed
            // colours are set to slight variations of black (makes UI seem more responsive)
            for (int i = 0; i < buttons.Length; i++)
            {
                var colors = buttons[i].colors;
                colors.normalColor = Color.black;
                colors.highlightedColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                colors.pressedColor = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                buttons[i].colors = colors;
            }

            // Finds all text objects in the scene
            Text[] allText = FindObjectsOfType<Text>();

            // Loops through each text object. If the text object is not white, then set the colour to black.
            // This means that white text used for buttons and speech bubbles are not impacted, but red text
            // used for explinations/settings are set to black
            for (int i = 0; i < allText.Length; i++)
            {
                if(!(allText[i].color.r == 1.0f && allText[i].color.g == 1.0f && allText[i].color.b == 1.0f)) 
                    allText[i].color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }

        }
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    // Executes when the user clicks on the 'ask a question' button
    public void OnAskQuestionClick()
    {
        // Switches to AR scene if the user has ticked to automatically use AR in the settings page
        if (currentSettings.ReturnFieldValue("autoUseAR"))
            OnARClick();
        // Else switches to avatar scene if the user has ticked to automatically use avatar in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAvatar"))
            OnAvatarClick();
        // Otherwise, switch to the chatbot scene 
        else
            OnChatbotClick();
    }

    // The following methods load scenes if the connected GameObject dictates that the scene should be changed
    public void OnARClick()
    {
        SceneManager.LoadScene("AR");
    }

    public void OnAvatarClick()
    {
        SceneManager.LoadScene("Avatar");
    }

    public void OnChatbotClick()
    {
        SceneManager.LoadScene("Chatbot");
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnInfoClick()
    {
        SceneManager.LoadScene("Info");
    }

    public void OnSettingsClick()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnPrevConversationsClick()
    {
        SceneManager.LoadScene("PreviousConversations");
    }


    // Executes if the user chooses to exit the app
    public void ExitApplication()
    {
        Application.Quit();
    }
}
