// The purpose of the session handler is to store all data that the user needs to interact with the 
// either the Chatbot, Avatar and AR scenes. Data is stored while the user is in each of these scenes
// and when the user loads another scene, the saved session data is loaded

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// Handles a conversation that is/has occured between the user and chatbot
public class SessionHandler
{
    // Data flags for the current session
    [Serializable]
    public class SessionData
    {
        public bool isTTSButtonActive;
    }

    // Stores data for the current session
    protected SessionData currentSessionData;

    // Stores the current conversation for the session
    public ConversationHandler currentConversation;

    // The number of minutes that must have passed since the last message was said for the session
    // to expire and be reset
    protected float timeBeforeRestartSession = 3.0f;

    // File path to the session data
    string sessionDataFilePath;

    // Loads the current session data and conversation
    public SessionHandler(string newSessionDataFilePath, 
                          string previousConversationsFilePath, string currentSessionConversationFilePath,
                          bool autoUseTTS)
    {

        // Creates a new Conversation Handler and sets whether to save messages
        // based on the 'save conversation' setting
        currentConversation = new ConversationHandler(previousConversationsFilePath, currentSessionConversationFilePath);

        // Loads in session messages
        currentConversation.LoadSessionConversation();

        sessionDataFilePath = newSessionDataFilePath;

        // If the session data file does not exist or the current session has expired
        if (!File.Exists(sessionDataFilePath) || hasSessionExpired())
        {
            // Create a new empty session and write an empty object JSON file
            ResetSession();

            // Set whether or not to use depending on the settings
            currentSessionData.isTTSButtonActive = autoUseTTS;
        }

        // Otherwise load in the current session data
        else
        {
            string jsonString = File.ReadAllText(sessionDataFilePath);
            currentSessionData = JsonUtility.FromJson<SessionData>(jsonString);
        }
    }

    // Checks if the current session has expired (and should be reset)
    public bool hasSessionExpired()
    {
        if (currentConversation.GetCurrentConversationSize() == 0)
            return true; 

        // Gets the last message that was sent in the conversation
        Message lastMessageSaved = currentConversation.GetNewMessageAtIndex(currentConversation.GetCurrentConversationSize() - 1);

        // Gets the datetime for when that message was sent
        DateTime lastTimeSent = DateTime.ParseExact(lastMessageSaved.timeProcessed, currentConversation.dateFormatUsed,
                                                    System.Globalization.CultureInfo.InvariantCulture);

        // The number of minutes difference between the current time and the last message that was sent
        float minutesDifference = (float)DateTime.Now.Subtract(lastTimeSent).TotalMinutes;

        // If more time has passed since the last message was sent than the ttl, then the session is restarted
        if (minutesDifference > timeBeforeRestartSession)
            return true;

        // Otherwise the session is still valid
        return false;
    }

    // Updates the session object depending on the field name argument and value
    public void UpdateField(string fieldName, bool newValue)
    {

        switch (fieldName)
        {
            case "isTTSButtonActive":
                currentSessionData.isTTSButtonActive = newValue;
                break;
        }

        WriteJson();
    }

    // Returns field value in JSON object dependong on the field name argument
    public bool ReturnFieldValue(string fieldName)
    {
        bool valueToReturn = false;

        switch (fieldName)
        {
            case "isTTSButtonActive":
                valueToReturn = currentSessionData.isTTSButtonActive;
                break;

        }

        return valueToReturn;
    }

    // Writes the current JSON object into the specified file at file location
    public void WriteJson()
    {
        string jsonString = JsonUtility.ToJson(currentSessionData);
        File.WriteAllText(sessionDataFilePath, jsonString);
    }

    // Create a new empty session and write an empty object JSON file 
    public void ResetSession()
    {
        currentSessionData = new SessionData();

        // Default use TTS is set to active
        currentSessionData.isTTSButtonActive = true;
        File.WriteAllText(sessionDataFilePath, JsonUtility.ToJson(currentSessionData));
    }
}