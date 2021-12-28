using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseUIScene : MonoBehaviour
{
    protected CanvasGroup fadeGroup;
    protected float fadeInSpeed; // multiplied by time.time if (10 seconds 0.1)

    protected void SetFade()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha = 1;
    }

    protected void UpdateFade()
    {
        // Fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;
    }

    protected void SetFadeInSpeed(float newFadeInSpeed)
    {
        fadeInSpeed = newFadeInSpeed;
    }

    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.33f);
    }

    private void Update()
    {
        UpdateFade();
    }

    // Buttons
    public void OnARClick()
    {
        SceneManager.LoadScene("AR");

    }
    public void OnMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnInfoClick()
    {
        SceneManager.LoadScene("Info");
    }

    public void OnUIClick()
    {
        SceneManager.LoadScene("UI");
    }


    public void OnSettingsClick()
    {
        SceneManager.LoadScene("Settings");
    }
}
