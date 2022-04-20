// This class contains commonly used methods that are used by all scene scripts; this class
// is inherited by all scene scripts in the app

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net;
// The main parent class used for most of the scenes in the project
// Set to abstract as this class should not be used individually
public abstract class BaseUIScene : MonoBehaviour
{
    protected Dictionary<string, string> languages;

    // The fade object
    protected CanvasGroup fadeGroup;

    // Speed to reduce fade alpha value (multiplied by time.time if (10 seconds 0.1))
    protected float fadeInSpeed; 

    // Holds current settings
    protected SettingsHandler currentSettings;

    private string url = "https://api.eu-gb.language-translator.watson.cloud.ibm.com/instances/fb81b1cf-25f6-426b-b292-bd4228bccc3e/v3/translate?version=2018-05-01";

    // Generates the correct path for a stored JSON file in the data folder. This must be called
    // after the scene has started (Application.persistentDataPath only works after script instantiation) 
    public string CreateRelativeFilePath(string JsonFile)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, JsonFile +".json");
        return filePath;
    }

    protected void LoadSettings()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        ConfigureTranslation();
    }

    // Initialises fade object
    protected void SetFade()
    {
        // Get CanvasGroup in scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start on white screen
        fadeGroup.alpha = 1;
    }

    // Updates fade alpha value
    protected void UpdateFade()
    {
        // Reduces alpha value to become more translucent(depending on fade in speed)
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;
    }

    // Updates the fade in speed
    protected void SetFadeInSpeed(float newFadeInSpeed)
    {
        fadeInSpeed = newFadeInSpeed;
    }

    // Updates object in the scene if the colour blind setting is used
    protected virtual void UpdateColoursIfColourBlindMode()
    {
        // Checks that the colour blind setting is currently being used
        if (currentSettings.ReturnFieldValue("useColourBlind"))
        {
            // Finds all button objects in the scene
            Button[] buttons = FindObjectsOfType<Button>();

            // Loops through each button object and sets the colour to black. Highlighted and pressed
            // colours are set to slight variations of black (makes UI seem more responsive)
            for (int i = 0; i < buttons.Length; i++)
            {
                var colors = buttons[i].colors;
                colors.normalColor = Color.black;
                colors.highlightedColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                colors.pressedColor = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                colors.selectedColor = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                buttons[i].colors = colors;
            }

            // Finds all text objects in the scene
            Text[] allText = FindObjectsOfType<Text>();

            // Loops through each text object. If the text object is not white, then set the colour to black.
            // This means that white text used for buttons and speech bubbles are not impacted, but red text
            // used for explinations/settings are set to black
            for (int i = 0; i < allText.Length; i++)
            {
                if(!(allText[i].color.r == 1.0f && allText[i].color.g == 1.0f && allText[i].color.b == 1.0f)) 
                    allText[i].color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }

            // Sets the background to white
            GameObject background = GameObject.Find("Background");

            if(background != null)
                background.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    // Loads main menu scene
    public void OnMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }

    // Loads settings scene
    public void OnSettingsClick()
    {
        SceneManager.LoadScene("Settings");
    }

    // Executes if the user chooses to exit the app
    public void ExitApplication()
    {
        Application.Quit();
    }

    protected void TranslateScene(){
        Text[] textobjs = FindObjectsOfType<Text>();
        string model = "";

        if (languages.TryGetValue(currentSettings.GetLanguage(), out model))
            model = model;

        foreach(Text t in textobjs)
            t.text = TranslateString(t.text, "en", model);
    }

    protected void ConfigureTranslation()
    {
        languages = new Dictionary<string, string>();
        if (languages.Count == 0)
            AddLanguages();
    }

    protected string TranslateString(string t, string sourceLanguage, string targetLanguage)
    {
        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
        httpRequest.Method = "POST";

        httpRequest.ContentType = "application/json";
        httpRequest.Headers["Authorization"] = "Basic YXBpa2V5OlZ5WUItRkljQlVJRHc1SGJXR1NQMjVzOFFCYXU4TkptWkF2RDNrVHNWaHNB";

        var data = "{\"text\":[\"" + t.Replace("\n", "") + "\"],\"model_id\":\"" + sourceLanguage + "-" + targetLanguage + "\"}";

        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
        {
            streamWriter.Write(data);
        }

        var result = "";

        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            result = streamReader.ReadToEnd();
        }

        string[] strlist = result.Split('{');
        foreach (string split in strlist)
        {
            if (split.Contains("translation") == true && split.Contains("translations") == false)
            {
                var text = split.Split(':')[1].Split('"')[1];
                return text;
            }
        }

        return "";
    }

    protected void AddLanguages(){
        languages = new Dictionary<string,string>();
        languages.Add("English", "en");
        languages.Add("Spanish","es");
        languages.Add("Simplified Chinese","zh");
        languages.Add("Traditional Chinese","zh_TW");
        languages.Add("Japanese","ja");
    }
}
