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
    protected GameObject keyboardInputField;

    protected GameObject microphoneRecordingInfoContainer;

    protected GameObject errorInfoText;

    protected GameObject microphoneButton;

    protected GameObject keyboardButton;

    protected DateTime recordingStartTime;

    protected int recordingTTL;

    protected bool recordingMessage = false;

    protected RectTransform[] soundBars;

    protected Transform soundBarHolder;

    protected DateTime errorTextShownTime;

    protected float errorTextTTL;

    protected const string languageCodeForTTS = "en-US";

    public Sprite greenMicrophoneSprite;
    public Sprite normalMicrophoneSprite;

    public Sprite greenKeyboardSprite;
    public Sprite normalKeyboardSprite;

    protected void ConfigureScene()
    {
        CheckPermissions();
        ConfigureTTSandSTT();

        // Sets the keyboard input has inactive, meaning the user cannot view it until they click
        // the keyboard button
        keyboardInputField = GameObject.Find("KeyboardInputField");
        keyboardInputField.SetActive(false);

        microphoneRecordingInfoContainer = GameObject.Find("MicrophoneRecordingInfoContainer");
        microphoneRecordingInfoContainer.SetActive(false);

        errorInfoText = GameObject.Find("ErrorText");
        errorInfoText.SetActive(false);

        microphoneButton = GameObject.Find("MicrophoneButton");
        keyboardButton = GameObject.Find("KeyboardButton");

        recordingStartTime = DateTime.Now;
        recordingTTL = 10;

        errorTextShownTime = DateTime.Now;
        errorTextTTL = 5;

        ConfigureConversation();

        SpeechErrorHandler("STOP BRO");
    }

    protected void CheckPermissions()
    {
#if UNITY_ANDROID
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

        soundBarHolder = GameObject.Find("BarHolder").transform;

#if UNITY_ANDROID

        int numberOfBars = soundBarHolder.childCount;
        soundBars = new RectTransform[numberOfBars];

        for (int i=0; i < numberOfBars; i++)
            soundBars[i] = soundBarHolder.GetChild(i).gameObject.GetComponent<RectTransform>();

        SpeechToText.instance.onErrorCallback = SpeechErrorHandler;
        SpeechToText.instance.onRmsChangedCallback = ChangeSoundBars;
#endif

    }

    // Configures the current conversation so that all messages in the current session are loaded
    protected void ConfigureConversation()
    {
        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        // Creates a new Conversation Handler and sets whether to save messages
        // based on the 'save conversation' setting
        currentConversation = new ConversationHandler(CreateRelativeFilePath("PreviousConversations"),
                                                   CreateRelativeFilePath("CurrentSession"));

        // Loads in session messages
        currentConversation.LoadSessionConversation();

        // The number of minutes that must have passed since the last message was said for the session
        // to expire and be reset
        float timeBeforeRestartSession = 3.0f;

        string welcomeMessage = "Hello there, how can I help you today?";

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

                currentConversation.AddNewMessage(welcomeMessage, false);

                TextToSpeech.instance.StartSpeak(welcomeMessage);
            }
        }

        // Adds the default greetings message if no messages are currently stored in the session
        else
        {
            currentConversation.AddNewMessage(welcomeMessage, false);
            TextToSpeech.instance.StartSpeak(welcomeMessage);
        }

        // Sets whether to save messages (after the welcom message has potentially been said)
        // based on the user's settings
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));
    }

    public void ChangeSoundBars(float intensity)
    {
        float size;
        int numSoundBars = soundBars.Length;
        for (int i = 0; i < numSoundBars; i++)
        {
            size = intensity * 10 * (((numSoundBars + 0.5f) / 2) - Math.Abs((numSoundBars / 2) - i));
            if (size > 600)
                size = 600;
            soundBars[i].sizeDelta = new Vector2(10, size);
        }
    }

    public void SpeechErrorHandler(string errorCode)
    {
        StopMicroPhoneRecording();

        errorInfoText.SetActive(true);
        errorTextShownTime = DateTime.Now;

        errorInfoText.GetComponent<Text>().text = errorCode;
    }

    protected void UpdateScene()
    {
        UpdateCheckMicrophoneRecording();
        UpdateCheckErrorInfo();
    }

    protected void UpdateCheckMicrophoneRecording()
    {
        if (recordingMessage)
        {
            int secondsDifference = (int)DateTime.Now.Subtract(recordingStartTime).TotalSeconds;

            if (secondsDifference >= recordingTTL)
                StopMicroPhoneRecording();
        }
    }

    protected void UpdateCheckErrorInfo()
    {
        if (errorInfoText.activeInHierarchy) { 
            float secondsSinceShowing = (float)DateTime.Now.Subtract(errorTextShownTime).TotalSeconds;

            Color newColor = Color.black;

            if (secondsSinceShowing > errorTextTTL)
            {
                errorInfoText.GetComponent<Text>().color = newColor;
                errorInfoText.SetActive(false);
            }
            else 
            {
                if (secondsSinceShowing < errorTextTTL - 2.5)
                    return; 

                newColor.a = Math.Abs(errorTextTTL - secondsSinceShowing)/errorTextTTL;
                errorInfoText.GetComponent<Text>().color = newColor;
            }
        }
    }

    // Executes if the keyboard button is clicked
    public void OnKeyboardClick()
    {
        // Does nothing if the user is currently recording a message
        if (recordingMessage)
            return;

        // If the keyboard is active, then deactive the input
        if (keyboardInputField.activeInHierarchy)
        {
            keyboardInputField.SetActive(false);

            keyboardButton.GetComponent<Image>().sprite = normalKeyboardSprite;
        }
        else
        {
            // Input is set to active
            keyboardInputField.SetActive(true);

            keyboardButton.GetComponent<Image>().sprite = greenKeyboardSprite;

            // Broken
            Text currentKeyboardInputText = GameObject.Find("KeyboardInputText").GetComponent<Text>();

            currentKeyboardInputText.text = "";


        }
    }

    public void OnMicroPhoneClick()
    {
        // Does nothing if the user has currently opened the keyboard
        if (keyboardInputField.activeInHierarchy)
            return;

        if (!recordingMessage)
            StartMicroPhoneRecording();
        else
            StopMicroPhoneRecording();
    }

    protected void StartMicroPhoneRecording()
    {
        recordingMessage = true;
        SpeechToText.instance.StartRecording();
        recordingStartTime = DateTime.Now;
        microphoneRecordingInfoContainer.SetActive(true);

#if UNITY_ANDROID

#else
        soundBarHolder.gameObject.SetActive(false);
#endif
        microphoneButton.GetComponent<Image>().sprite = greenMicrophoneSprite;
    }

    protected void StopMicroPhoneRecording()
    {
        recordingMessage = false;
        SpeechToText.instance.StopRecording();
        microphoneRecordingInfoContainer.SetActive(false);

        microphoneButton.GetComponent<Image>().sprite = normalMicrophoneSprite;
    }

    // Virtual class for when the user submits a keyboard message, a subclass
    // can handle this individually
    public virtual void OnKeyboardSubmit(string message)
    {
        // Checks that the message is valid
        if (!CheckMessageIsValid(message))
            return;

        // Keyboard input field is made inactive
        keyboardInputField.SetActive(false);

        // Adds the new message to the conversation
        currentConversation.AddNewMessage(message, true);

        message = SimplifyMessageString(message);

        // Gets the Watson response message
        string watsonResponseMessage = GetWatsonResponse(message);

        // Adds new message to conversation and renders it
        currentConversation.AddNewMessage(watsonResponseMessage, false);
    }

    public virtual void OnSpeechTranslation(string message)
    {
        if (recordingMessage)
        {
            recordingMessage = false;
            microphoneRecordingInfoContainer.SetActive(false);
        }

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

        TextToSpeech.instance.StartSpeak(defaultMessage);

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

