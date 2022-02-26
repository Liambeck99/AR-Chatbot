// Script that is attached to the info scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfoScene : BaseUIScene
{
    private void Start()
    {
        LoadSettings();

        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();
        if (currentSettings.GetLanguage() != "English"){
            Translate();
        }
    }

    private void Update()
    {
        UpdateFade();
    }

}
