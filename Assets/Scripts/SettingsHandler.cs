using System;
using System.IO;
using UnityEngine;

public class SettingsHandler
{

    public class SettingsJSON
    {
        public bool autoUseAvatar;
        public bool autoUseAR;
        public bool autoUseTTS;
        public bool saveConversations;
    }

    private SettingsJSON newSettingsFile;
    private string fileName = "Assets/Data/ApplicationSettings.json";

    public SettingsHandler()
    {
        ReadJson();
    }

    public void WriteJson()
    {
        string jsonString = JsonUtility.ToJson(newSettingsFile);
        File.WriteAllText(fileName, jsonString);
    }

    public void ReadJson()
    {
        string jsonString = File.ReadAllText(fileName);
        newSettingsFile = JsonUtility.FromJson<SettingsJSON>(jsonString);
    }

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
        }
    }

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
        }

        return valueToReturn;
    }

}