using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseUIScene : MonoBehaviour
{
    protected CanvasGroup fadeGroup;
    protected float fadeInSpeed; // multiplied by time.time if (10 seconds 0.1)

    // Holds current settings
    protected SettingsHandler currentSettings = new SettingsHandler();

    protected void SetFade()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha = 1;
    }

    protected void UpdateFade()
    {
        // Fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;
    }

    protected void SetFadeInSpeed(float newFadeInSpeed)
    {
        fadeInSpeed = newFadeInSpeed;
    }

    protected void UpdateColoursIfColourBlindMode()
    {
        if (currentSettings.ReturnFieldValue("useColourBlind"))
        {
            Button[] buttons = FindObjectsOfType<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                var colors = buttons[i].colors;
                colors.normalColor = Color.black;
                colors.highlightedColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                colors.pressedColor = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                buttons[i].colors = colors;
            }

            Text[] allText = FindObjectsOfType<Text>();

            for (int i = 0; i < allText.Length; i++)
            {
                if(!(allText[i].color.r == 1.0f && allText[i].color.g == 1.0f && allText[i].color.b == 1.0f)) 
                    allText[i].color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }

        }
    }

    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.33f);
    }

    private void Update()
    {
        UpdateFade();
    }

    public void OnAskQuestionClick()
    {
        if(currentSettings.ReturnFieldValue("autoUseAR"))
            SceneManager.LoadScene("AR");
        else if(currentSettings.ReturnFieldValue("autoUseAvatar"))
            SceneManager.LoadScene("Avatar");
        else
            SceneManager.LoadScene("Chatbot");
    }

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

    public void ExitApplication()
    {
        Application.Quit();
    }
}
