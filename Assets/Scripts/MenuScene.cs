using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : BaseUIScene
{
    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.33f);
    }

    private void Update()
    {
        UpdateFade();
    }

}