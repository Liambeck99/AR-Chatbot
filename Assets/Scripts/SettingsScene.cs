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
        Toggle[] allToggles = FindObjectsOfType<Toggle>();

        for (int i = 0; i < allToggles.Length; i++)
        {
            Toggle currentToggle = allToggles[i];
            currentToggle.isOn = currentSettings.ReturnFieldValue(currentToggle.tag);

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