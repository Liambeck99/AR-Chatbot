using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MappingScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f; // multiplied by time.time if (10 seconds 0.1)
    

    private void Start()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha =1;
    }

    private void Update()
    {
        // Fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        //

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

}
