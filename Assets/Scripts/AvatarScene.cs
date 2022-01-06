// Script that is attached to the Avatar scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AvatarScene : BaseSessionScene
{
    private void Start()
    {
        LoadSettings();
        ConfigureScene();
    }

    private void Update()
    {
        UpdateScene();
    }
}