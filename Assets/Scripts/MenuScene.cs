// Script that is attached to the menu scene

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : BaseUIScene
{
    string SockeyFile = @"{
    'keyword': 'A Cappella',
    'name': 'A Cappella',
    'link': 'https://engage.luu.org.uk/groups/TTQ/a-cappella'
},
{
    'keyword': 'Abuse',
    'name': 'Abuse Awareness Society',
    'link': 'https://engage.luu.org.uk/groups/TF9/abuse-awareness-society'
},
{
    'keyword': 'rugby',
    'name': 'Rugby Union Society',
    'link': 'https://engage.luu.org.uk/groups/H3H/adventist-students-on-campus'
},
{
    'keyword': 'African Caribbean',
    'name': 'African Caribbean',
    'link': 'https://engage.luu.org.uk/groups/47D/african-caribbean'
},
{
    'keyword': 'Ahlul Bayt',
    'name': 'Ahlul Bayt',
    'link': 'https://engage.luu.org.uk/groups/HFC/ahlul-bayt'
},
{
    'keyword': 'Akido',
    'name': 'Akido',
    'link': 'https://engage.luu.org.uk/groups/8YT/aikido'
},
{
    'keyword': 'AIMES',
    'name': 'AIMES',
    'link': 'https://engage.luu.org.uk/groups/7TH/aimes'
},
{
    'keyword': 'American Football',
    'name': 'American Football',
    'link': 'https://engage.luu.org.uk/groups/DBD/american-football'
},
{
    'keyword': 'Karate',
    'name': 'Leeds Karate Society',
    'link': 'https://engage.luu.org.uk/groups/PTC/karate'
},
{
    'keyword': 'football',
    'name': 'Leeds Football Society',
    'link': 'https://engage.luu.org.uk/groups/PTC/karate'
},
{
    'keyword': 'Computing',
    'name': 'CompSoc',
    'link': 'https://engage.luu.org.uk/groups/PTC/karate'
},";

    private void Start()
    {
        LoadSocietyFileIfNotExists();

        LoadSettings();

        SetFade();
        SetFadeInSpeed(0.33f);
        UpdateColoursIfColourBlindMode();

        if (currentSettings.GetLanguage() != "English")
            TranslateScene();
    }

    private void LoadSocietyFileIfNotExists()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/data/sockey.json"))
            System.IO.File.WriteAllText(Application.persistentDataPath + "/data/sockey.json", SockeyFile);
    }

    private void Update()
    {
        UpdateFade();
    }

    // Executes when the user clicks on the 'ask a question' button
    public void OnAskQuestionClick()
    {
        // Switches to AR scene if the user has not yet completed the tutorial
        if (!currentSettings.ReturnFieldValue("completeTutorial"))
            SceneManager.LoadScene("AR"); 
        // Switches to AR scene if the user has ticked to automatically use AR in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAR"))
            SceneManager.LoadScene("AR");
        // Else switches to avatar scene if the user has ticked to automatically use avatar in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAvatar"))
            SceneManager.LoadScene("Avatar");
        // Otherwise, switch to the chatbot scene 
        else
            SceneManager.LoadScene("Chatbot");
    }

    // Loads info scene
    public void OnInfoClick()
    {
        SceneManager.LoadScene("Info");
    }

    // Loads mapping scene
    public void OnMappingClick()
    {
        SceneManager.LoadScene("Mapping");
    }

}