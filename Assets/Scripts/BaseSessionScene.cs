using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using TextSpeech;

// The main parent class used for all scenes that interact with the AI in the project
// Set to abstract as this class should not be used individually
public abstract class BaseSessionScene : BaseUIScene
{
    // Stores the current conversation for the session
    protected ConversationHandler currentConversation;

    // The input field for allowing the user to enter in text to the chatbot
    protected GameObject KeyboardInputField;

    protected AudioSource microphoneInputClip;

    protected DateTime recordingStartTime;

    protected int maxRecordingTime;

    protected GameObject microphoneRecordingInfoContainer;

    const string languageCodeForTTS = "en-US";

    protected bool recordingMessage = false;

    protected void CheckPermissions()
    {
#if UNITY_ANDRIOD
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);

        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            Permission.RequestUserPermission(Permission.CoarseLocation);

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            Permission.RequestUserPermission(Permission.FineLocation);
#endif
    }

    protected void ConfigureTTSandSTT()
    {
        TextToSpeech.instance.Setting(languageCodeForTTS, 1, 1);
        SpeechToText.instance.Setting(languageCodeForTTS);

        SpeechToText.instance.onResultCallback = OnSpeechTranslation;
    }

    // Configures the current conversation so that all messages in the current session are loaded
    protected void ConfigureConversation()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        // Creates a new Conversation Handler and sets whether to save messages
        // based on the 'save conversation' setting
        currentConversation = new ConversationHandler(CreateRelativeFilePath("PreviousConversations"),
                                                   CreateRelativeFilePath("CurrentSession"));

        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        // Loads in session messages
        currentConversation.LoadSessionConversation();

        // The number of minutes that must have passed since the last message was said for the session
        // to expire and be reset
        float timeBeforeRestartSession = 3.0f;

        // Checks if the number of current messages in the session is more than 0 (indicates there is a session currently taking place)
        if (currentConversation.GetCurrentConversationSize() > 0)
        {
            // Gets the last saved message in the session
            Message lastMessageSaved = currentConversation.GetNewMessageAtIndex(currentConversation.GetCurrentConversationSize() - 1);

            // Gets the datetime for when that message was sent
            DateTime lastTimeSent = DateTime.ParseExact(lastMessageSaved.timeProcessed, currentConversation.dateFormatUsed,
                                                        System.Globalization.CultureInfo.InvariantCulture);

            // The number of minutes difference between the current time and the last message that was sent
            float minutesDifference = (float)DateTime.Now.Subtract(lastTimeSent).TotalMinutes;

            // If more time has passed since the last message was sent than the ttl, then the session is restarted
            if (minutesDifference > timeBeforeRestartSession)
            {
                currentConversation.ResetSessionConversation();
                currentConversation.ResetCurrentConversation();

                currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
            }
        }

        // Adds the default greetings message if no messages are currently stored in the session
        else
            currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
    }

    // Sets all input fields to default values when the scene is loaded
    protected void ConfigureInputs()
    {
        // Sets the keyboard input has inactive, meaning the user cannot view it until they click
        // the keyboard button
        KeyboardInputField = GameObject.Find("KeyboardInputField");
        KeyboardInputField.SetActive(false);
    }

    // Executes if the keyboard button is clicked
    protected void OnKeyboardClick()
    {
        // If the keyboard is active, then deactive the input
        if (KeyboardInputField.activeInHierarchy)
            KeyboardInputField.SetActive(false);
        else
        {
            // Input is set to active
            KeyboardInputField.SetActive(true);

            // Broken
            Text currentKeyboardInputText = GameObject.Find("KeyboardInputText").GetComponent<Text>();

            currentKeyboardInputText.text = "";

            Debug.Log(currentKeyboardInputText.text);
        }
    }

    public void ConfigureMicrophone()
    {
        recordingStartTime = DateTime.Now;
        maxRecordingTime = 10;
        microphoneInputClip = GetComponent<AudioSource>();
        microphoneRecordingInfoContainer = GameObject.Find("MicrophoneRecordingInfoContainer");
        microphoneRecordingInfoContainer.SetActive(false);
    }

    public void UpdateCheckMicrophoneRecording()
    {
        if (Microphone.IsRecording(null))
        {
            int secondsDifference = (int)DateTime.Now.Subtract(recordingStartTime).TotalSeconds;

            if (secondsDifference >= maxRecordingTime)
            {
                microphoneRecordingInfoContainer.SetActive(false);
                Microphone.End(null);
            }
        }
    }

    // Abstract class for when the user submits a keyboard message, each subclass
    // should handle this individually
    public abstract void OnKeyboardSubmit(string message);

    public void OnMicroPhoneClick()
    {
        if (!Microphone.IsRecording(null))
        {
            microphoneInputClip.clip = Microphone.Start(null, true, 10, 48000);
            recordingStartTime = DateTime.Now;
            microphoneRecordingInfoContainer.SetActive(true);
        }
        else
        {
            Microphone.End(null);
            microphoneRecordingInfoContainer.SetActive(false);
        }

        /*if (recordingMessage)
        {
            recordingMessage = false; 
            SpeechToText.instance.StopRecording();
        }
        else
        {
            recordingMessage = true;
            SpeechToText.instance.StartRecording();
        }*/
    }

    public void OnSpeechTranslation(string message)
    {
        // Adds the new message to the conversation
        currentConversation.AddNewMessage(message, true);

        // Gets the Watson response message
        string watsonResponseMessage = GetWatsonResponse(message);

        // Adds new message to conversation and renders it
        currentConversation.AddNewMessage(watsonResponseMessage, false);
    }

    // Gets a response from Watsom based on the message argument
    protected string GetWatsonResponse(string message)
    {
        // Watson exchange goes here

        string defaultMessage = "This is an example of what a response would look like...";

        //TextToSpeech.instance.StartSpeak(defaultMessage);

        return defaultMessage;
    }

    // Checks the format and contents of the message to ensure it is valid for parsing to Watson
    protected bool CheckMessageIsValid(string message)
    {
        // Add message checking

        if (message.Length < 10)
            return false;
        else if (message.Length > 200)
            return false;
        
        return true;
    }

    // Simplifies the message so that it is more clear for when Watson parses it
    protected string SimplifyMessageString(string message)
    {
        // Add message simplification

        return message;
    }
}

