using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsScene : BaseUIScene
{
    private SettingsHandler currentSettings;

    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.66f);
        currentSettings = new SettingsHandler();

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
            }
        }

    }

    private void Update()
    {
        UpdateFade();
    }

    public void UpdateAutomaticallyUseAvatarSetting(Toggle toggle)
    {
        currentSettings.UpdateField("autoUseAvatar", toggle.isOn);
        currentSettings.WriteJson();
    }

    public void UpdateAutomaticallyUseARSetting(Toggle toggle)
    {
        currentSettings.UpdateField("autoUseAR", toggle.isOn);
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
}