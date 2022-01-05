using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : BaseUIScene
{
    private void Start()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));
        Debug.Log(Application.persistentDataPath);
        SetFade();
        SetFadeInSpeed(0.33f);
        UpdateColoursIfColourBlindMode();
    }

    private void Update()
    {
        UpdateFade();
    }

}