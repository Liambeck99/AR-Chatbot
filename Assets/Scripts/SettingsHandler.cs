using System;
using System.IO;
using UnityEngine;

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
    }

    // JSON format object
    private SettingsJSON newSettingsFile;

    // File name to store JSON
    private string fileName = "Assets/Data/ApplicationSettings.json";

    public SettingsHandler()
    {
        ReadJson();
    }

    // Writes the current JSON object into the specified file at file location
    public void WriteJson()
    {
        string jsonString = JsonUtility.ToJson(newSettingsFile);
        File.WriteAllText(fileName, jsonString);
    }

    // Reads JSON from file location into current JSON object
    public void ReadJson()
    {
        string jsonString = File.ReadAllText(fileName);
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
        }

        return valueToReturn;
    }

}