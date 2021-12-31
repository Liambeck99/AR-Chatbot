using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreviousConversations: BaseUIScene
{
    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();
    }

    private void Update()
    {
        UpdateFade();
    }

}