using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfoScene : BaseUIScene
{
    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.66f);
    }

    private void Update()
    {
        UpdateFade();
    }

}
