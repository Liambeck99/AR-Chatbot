﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfoScene : BaseUIScene
{
    private void Start()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();
    }

    private void Update()
    {
        UpdateFade();
    }

}
