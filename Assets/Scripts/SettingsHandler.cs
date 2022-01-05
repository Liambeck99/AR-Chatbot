using System;
using System.IO;
using UnityEngine;

// Manages and handles the saved Settings
public class SettingsHandler
{
    // Object that contains the format of the Settings JSON file
    public class SettingsJSON
    {
        public bool autoUseAvatar;
        public bool autoUseAR;
        public bool autoUseTTS;
        public bool saveConversations;
        public bool useColorBlind;
        public bool completeTutorial;
    }

    // JSON format object
    private SettingsJSON newSettingsFile;

    // File path to store JSON
    private string filePath;

    public SettingsHandler(string settingsFilePath)
    {
        newSettingsFile = new SettingsJSON();

        filePath = settingsFilePath;

        CreateFileIfNotExists();
        ReadJson();
    }

    public void CreateFileIfNotExists()
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if (!File.Exists(filePath))
            resetSettings();
    }

    // Writes the current JSON object into the specified file at file location
    public void WriteJson()
    {
        string jsonString = JsonUtility.ToJson(newSettingsFile);
        File.WriteAllText(filePath, jsonString);
    }

    // Reads JSON from file location into current JSON object
    public void ReadJson()
    {
        string jsonString = File.ReadAllText(filePath);
        newSettingsFile = JsonUtility.FromJson<SettingsJSON>(jsonString);
    }
       
    // Updates a field in the JSON object depending on the field name argument
    public void UpdateField(string fieldName, bool newValue)
    {
        switch (fieldName)
        {
            case "autoUseAR":
                newSettingsFile.autoUseAR = newValue;
                break;

            case "autoUseAvatar":
                newSettingsFile.autoUseAvatar = newValue;
                break;

            case "autoUseTTS":
                newSettingsFile.autoUseTTS = newValue;
                break;

            case "saveConversations":
                newSettingsFile.saveConversations = newValue;
                break;

            case "useColourBlind":
                newSettingsFile.useColorBlind = newValue;
                break;

            case "completeTutorial":
                newSettingsFile.completeTutorial = newValue;
                break;
        }
    }

    // Returns field value in JSON object dependong on the field name argument
    public bool ReturnFieldValue(string fieldName)
    {
        bool valueToReturn = false;

        switch (fieldName)
        {
            case "autoUseAR":
                valueToReturn = newSettingsFile.autoUseAR;
                break;

            case "autoUseAvatar":
                valueToReturn = newSettingsFile.autoUseAvatar;
                break;

            case "autoUseTTS":
                valueToReturn = newSettingsFile.autoUseTTS;
                break;

            case "saveConversations":
                valueToReturn = newSettingsFile.saveConversations;
                break;

            case "useColourBlind":
                valueToReturn = newSettingsFile.useColorBlind;
                break;

            case "completeTutorial":
                valueToReturn = newSettingsFile.completeTutorial;
                break;
        }

        return valueToReturn;
    }

    // Resets the settings by updating each field to the default setting value
    // and saving as the new JSON settings file
    public void resetSettings()
    {
        newSettingsFile.autoUseAR = true;
        newSettingsFile.autoUseAvatar = true;
        newSettingsFile.autoUseTTS = true;
        newSettingsFile.saveConversations = true;
        newSettingsFile.useColorBlind = false;
        newSettingsFile.completeTutorial = false;


        WriteJson();
    }

}