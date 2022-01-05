using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsScene : BaseUIScene
{

    private void Start()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();

        ConfigureToggles();
    }

    private void ConfigureToggles()
    {

        // Finds all toggle objects in the scene
        Toggle[] allToggles = FindObjectsOfType<Toggle>();

        // Iterates through each toggle
        for (int i = 0; i < allToggles.Length; i++)
        {
            Toggle currentToggle = allToggles[i];

            // Updates wether the toggle is active (on/off) depending on its value (specified by the tag) stored in the settings file
            currentToggle.isOn = currentSettings.ReturnFieldValue(currentToggle.tag);

            // Sets a listener depending on the current toggle tag
            // (e.g. "autoUseTTS" tag will activate the UpdateAutomaticallyUseTTS method when the value of the toggle is changed)
            switch (currentToggle.tag)
            {
                case "autoUseAR":
                    currentToggle.onValueChanged.AddListener(delegate {
                        UpdateAutomaticallyUseARSetting(currentToggle);
                    });
                    break;

                case "autoUseAvatar":
                    currentToggle.onValueChanged.AddListener(delegate {
                        UpdateAutomaticallyUseAvatarSetting(currentToggle);
                    });
                    break;

                case "autoUseTTS":
                    currentToggle.onValueChanged.AddListener(delegate {
                        UpdateAutomaticallyUseTTS(currentToggle);
                    });
                    break;

                case "saveConversations":
                    currentToggle.onValueChanged.AddListener(delegate {
                        UpdateSaveConversations(currentToggle);
                    });
                    break;

                case "useColourBlind":
                    currentToggle.onValueChanged.AddListener(delegate {
                        UpdateColourBlindMode(currentToggle);
                    });
                    break;
            }
        }

    }

    // Changes all buttons/text on the page back to their default colours
    public void ReverseColourBlindMode()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        Color redLeedsColor = new Color(0.7686275f, 0.07058824f, 0.1882353f, 1.0f);

        // All buttons get converted back to the default red colour
        for (int i = 0; i < buttons.Length; i++)
        {
            var colors = buttons[i].colors;
            colors.normalColor = redLeedsColor;
            colors.highlightedColor = redLeedsColor;
            colors.pressedColor = redLeedsColor;
            colors.selectedColor = redLeedsColor;
            buttons[i].colors = colors;
        }

        Text[] allText = FindObjectsOfType<Text>();

        // All text, apart from the title, get converted back to red if they are black
        for (int i = 0; i < allText.Length; i++)
        {
            if (allText[i].name == "Title")
                continue;

            if (allText[i].color.r == 0.0f && allText[i].color.g == 0.0f && allText[i].color.b == 0.0f)
                allText[i].color = redLeedsColor;
        }

        // Sets the background to cream colour
        GameObject background = GameObject.Find("Background");

        if (background != null)
            background.GetComponent<Image>().color = new Color(0.9647059f, 0.945098f, 0.8941177f, 1.0f);
    }

    private void Update()
    {
        UpdateFade();
    }

    // The following methods each update an individual setting if toggled on the settings page 

    public void UpdateAutomaticallyUseARSetting(Toggle toggle)
    {
        currentSettings.UpdateField("autoUseAR", toggle.isOn);
        currentSettings.WriteJson();
    }

    public void UpdateAutomaticallyUseAvatarSetting(Toggle toggle)
    {
        currentSettings.UpdateField("autoUseAvatar", toggle.isOn);
        currentSettings.WriteJson();
    }

    public void UpdateAutomaticallyUseTTS(Toggle toggle)
    {
        currentSettings.UpdateField("autoUseTTS", toggle.isOn);
        currentSettings.WriteJson();
    }

    public void UpdateSaveConversations(Toggle toggle)
    {
        currentSettings.UpdateField("saveConversations", toggle.isOn);
        currentSettings.WriteJson();
    }

    public void UpdateColourBlindMode(Toggle toggle)
    {
        currentSettings.UpdateField("useColourBlind", toggle.isOn);
        currentSettings.WriteJson();

        if (toggle.isOn)
            UpdateColoursIfColourBlindMode();
        else
            ReverseColourBlindMode();
    }

    // Executed if the user selects to reset the application back to default settings
    public void ResetApplication()
    {
        // Settings are reset
        currentSettings.resetSettings();

        // Previous conversation messages are deleted
        ConversationHandler currentConversation = new ConversationHandler(CreateRelativeFilePath("PreviousConversations"),
                                                                          CreateRelativeFilePath("CurrentSession"));
        currentConversation.ResetPrevConversations();
        currentConversation.ResetSessionConversation();

        // Clears data cache
        Caching.ClearCache();

        // Application is closed
        Application.Quit();
    }
}