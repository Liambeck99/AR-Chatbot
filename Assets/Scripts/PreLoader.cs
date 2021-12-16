/*
    Refrenece - https://www.youtube.com/watch?v=sZhhfOH0Q3Y
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoader : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 3.0f;

    private void Start()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha = 1;

        // Preload app
        // LOAD ALL DEPENDENCIES HERE

        if (Time.time < minimumLogoTime)
            loadTime = minimumLogoTime;
        else
            loadTime =Time.time;
    }

    private void Update()
    {
        // Fade in
        if (Time.time < minimumLogoTime)
        {
            fadeGroup.alpha = 1 - Time.time;
        }

        // Fade out
        if (Time.time > minimumLogoTime && loadTime != 0 ){
            fadeGroup.alpha = Time.time - minimumLogoTime;
            if(fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
