using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
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
        Debug.Log("AR Button has been clicked!");
        SceneManager.LoadScene("AR");
        
    }

    public void OnInfoClick()
    {
        Debug.Log("Info Button has been clicked!");
    }

    public void OnMappingClick()
    {
        Debug.Log("Mapping Button has been clicked!");
    }

}
