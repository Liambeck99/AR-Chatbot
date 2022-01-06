using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AvatarScene : BaseSessionScene
{
    private void Start()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));
        ConfigureScene();
    }

    private void Update()
    {
        UpdateScene();
    }
}