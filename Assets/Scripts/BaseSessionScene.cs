﻿// The parent class for the AR, Avatar and Chatbot scenes. This class contains
// methods for handling user input (microphone and keyboard) as well as how
// to output responses

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

    // The information that is shown when the microphone is recording
    private GameObject microphoneRecordingInfoContainer;

    // The information that is shown when an error occurs (e.g. insufficient microphone permissions)
    private GameObject errorInfoText;

    // Main buttons for the microphone and keyboard
    protected GameObject microphoneButton;
    protected GameObject keyboardButton;

    protected GameObject speechTTSButton;

    // The time that the microphone started recording after the microphone button is clicked
    private DateTime recordingStartTime;

    // How long the microphone should record for before being disconnected
    private int recordingTTL;

    // Determines whether a message is being recorded
    private bool recordingMessage = false;

    // The time that the error information box was shown
    private DateTime errorTextShownTime;

    // How Long the error information box should be shown before being hidden
    private float errorTextTTL;

    // Stores all sound bars in the decibel animation
    private RectTransform[] soundBars;

    // Container object for the sound bars
    private Transform soundBarHolder;

    // The language code used for the Text to Speech translation"
    protected const string languageCodeForTTS = "en-US";

    // Sprites for the two microphone states (Green = active/ Normal= idle)
    public Sprite greenMicrophoneSprite;
    public Sprite normalMicrophoneSprite;

    // Sprites for the two keyboard states (Green = active/ Normal= idle)
    public Sprite greenKeyboardSprite;
    public Sprite normalKeyboardSprite;

    // Sprites for the two TTS button states (Green = active/ Normal= idle)
    public Sprite greenTTSButtonSprite;
    public Sprite redTTSButtonSprite;

    // Contains whether or not to use the text-to-speech functionality
    protected bool useTTS;

    // Contains whether or not text is currently being spoken
    protected bool currentlySpeaking;

    // Configures all data for the scene to work
    protected void ConfigureScene()
    {
        LoadSettings();

        // Checks that the user has the correct permissions for the app to work
        CheckPermissions();

        // Configures TTS and STT for correct translation ext...
        ConfigureTTSandSTT();

        // Sets the keyboard input has inactive, meaning the user cannot view it until they click
        // the keyboard button
        keyboardInputField = GameObject.Find("KeyboardInputField");
        keyboardInputField.SetActive(false);

        // Default sets the microphone information container to inactive (hidden)
        microphoneRecordingInfoContainer = GameObject.Find("MicrophoneRecordingInfoContainer");
        microphoneRecordingInfoContainer.SetActive(false);

        // Default sets the error information container to inactive (hidden)
        errorInfoText = GameObject.Find("ErrorText");
        errorInfoText.SetActive(false);

        // Gets the microphone and keyboard GameObject buttons from the UI
        microphoneButton = GameObject.Find("MicrophoneButton");
        keyboardButton = GameObject.Find("KeyboardButton");

        speechTTSButton = GameObject.Find("TTSButton");

        // Sets the time to live for the microphone recording
        recordingStartTime = DateTime.Now;
        recordingTTL = 10;

        // Sets the time to live for the error box to show
        errorTextShownTime = DateTime.Now;
        errorTextTTL = 5;

        // Automatically uses TTS if ticked in the settings
        useTTS = true;
        if (!currentSettings.ReturnFieldValue("autoUseTTS"))
        {
            useTTS = false;
            speechTTSButton.GetComponent<Image>().sprite = redTTSButtonSprite;
        }

        // Avatar is not speaking so set to false
        currentlySpeaking = false;

        // Configures the conversation by analysing session data
        ConfigureConversation();
    }

    // Checks if the user has correct permissions, this is only needed if the user is using andriod
    // as Apple does this automatically
    protected void CheckPermissions()
    {
#if UNITY_ANDROID
        // Requests microphone permission if this permission is not already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        // Requests camera permission if this permission is not already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);

        // Requests coarse GPS location permission if this permission is not already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            Permission.RequestUserPermission(Permission.CoarseLocation);

        // Requests accurate GPS location permission if this permission is not already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            Permission.RequestUserPermission(Permission.FineLocation);
#endif 
    }

    // Configures the TTS and STT objects
    protected void ConfigureTTSandSTT()
    {
        // Sets TTS and STT settings to the language code
        TextToSpeech.instance.Setting(languageCodeForTTS, 1, 1);
        SpeechToText.instance.Setting(languageCodeForTTS);

        // Calls when the avatar starts/stops talking
        TextToSpeech.instance.onStartCallBack = OnSpeechStart;
        TextToSpeech.instance.onDoneCallback = OnSpeechStop;

        // When a translation has been made from a user speech input,
        // then the 'OnSpeechTranslation' method is called
        SpeechToText.instance.onResultCallback = OnSpeechTranslation;

        soundBarHolder = GameObject.Find("BarHolder").transform;

        // The following only applies to the Android build of the app, as currently
        // Apples does not support these features
#if UNITY_ANDROID
        
        // Gets the number of sound bars and creates a new array to manage these objects
        int numberOfBars = soundBarHolder.childCount;
        soundBars = new RectTransform[numberOfBars];

        // Gets each sound bar from the sound bar container parent
        for (int i=0; i < numberOfBars; i++)
            soundBars[i] = soundBarHolder.GetChild(i).gameObject.GetComponent<RectTransform>();

        // If an error occures, then the 'ErrorHandler' method is called
        SpeechToText.instance.onErrorCallback = ErrorHandler;

        // Calculates with each sound byte the decibel and calls the function 'ChangeSoundBars'
        SpeechToText.instance.onRmsChangedCallback = ChangeSoundBars;
#endif
    }

    // Configures the current conversation so that all messages in the current session are loaded
    protected void ConfigureConversation()
    {
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

                StartTTSIfActivated(welcomeMessage);
            }
        }

        // Adds the default greetings message if no messages are currently stored in the session
        else
        {
            currentConversation.AddNewMessage(welcomeMessage, false);

            StartTTSIfActivated(welcomeMessage);
        }

        // Sets whether to save messages (after the welcom message has potentially been said)
        // based on the user's settings
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));
    }

    // Changes the length of the sound bars if the user is speaking
    public void ChangeSoundBars(float intensity)
    {
        float size;
        int numSoundBars = soundBars.Length;

        // Loops through each soundbar, for this animation the bars in the centre of the number of bars
        // should have a length larger than adjacent bars, with the length decreasing the further from
        // the centre the bars are
        for (int i = 0; i < numSoundBars; i++)
        {
            // Calculates the length of the bar based on its position in the array (centre = larger)
            size = intensity * 10 * (((numSoundBars + 0.5f) / 2) - Math.Abs((numSoundBars / 2) - i));

            // Bar length should not be greater than 600
            if (size > 600)
                size = 600;

            // Updates with the new size
            soundBars[i].sizeDelta = new Vector2(10, size);
        }
    }

    // Called when an error occurs and needs to be displayed to the user
    public void ErrorHandler(string errorCode)
    {
        // Keyboard is closed
        keyboardInputField.SetActive(false);
        keyboardButton.GetComponent<Image>().sprite = normalKeyboardSprite;

        // Microphone is closed
        recordingMessage = false;
        microphoneRecordingInfoContainer.SetActive(false);
        microphoneButton.GetComponent<Image>().sprite = normalMicrophoneSprite;

        // Activates the error info container and sets the time that is was visible to the current time
        errorInfoText.SetActive(true);
        errorTextShownTime = DateTime.Now;

        // Sets Error Info color to black (avoids colour not being black when shown)
        Color newColor = Color.black;
        errorInfoText.GetComponent<Text>().color = newColor;

        // Text is updated to the error code argument
        errorInfoText.GetComponent<Text>().text = "Error: " + errorCode;
    }

    // Used to update all parts of the scene
    protected void UpdateScene()
    {
        UpdateCheckMicrophoneRecording();
        UpdateCheckErrorInfo();
    }

    // Checks if the microphone recording has exceeded the TTL
    protected void UpdateCheckMicrophoneRecording()
    {
        // Checks if the microphone is currently recording
        if (recordingMessage)
        {
            // Calculates the difference in seconds from the recording start time and current time
            int secondsDifference = (int)DateTime.Now.Subtract(recordingStartTime).TotalSeconds;

            // If the number of seconds is larger than the TTL, then stop of the microphone recording
            if (secondsDifference >= recordingTTL)
                StopMicroPhoneRecording();
        }
    }

    // Checks if the error information text has exceeded the TTL
    protected void UpdateCheckErrorInfo()
    {
        // Checks if the error information text is currently visible
        if (errorInfoText.activeInHierarchy) {

            float animationStartTime = 2.5f;

            // Calculates the difference in seconds from the time the erorr was shown and current time
            float secondsSinceShowing = (float)DateTime.Now.Subtract(errorTextShownTime).TotalSeconds;

            Color newColor = Color.black;

            // If the number of seconds is greater than the time to live, then set the colour back
            // to the default black and make the text invisible
            if (secondsSinceShowing > errorTextTTL)
            {
                errorInfoText.GetComponent<Text>().color = newColor;
                errorInfoText.SetActive(false);
            }
            else 
            {
                // Checks if the animation should occur, if so then slowly change the alpha
                // value so that the text fades away
                if (secondsSinceShowing > errorTextTTL - animationStartTime)
                {
                    newColor.a = Math.Abs(errorTextTTL - secondsSinceShowing) / errorTextTTL;
                    errorInfoText.GetComponent<Text>().color = newColor;
                }
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

            // Sets the keyboard button colour back to the default black
            keyboardButton.GetComponent<Image>().sprite = normalKeyboardSprite;
        }
        else
        {
            // Input is set to active
            keyboardInputField.SetActive(true);

            // Sets the keyboard button colour to the active green state
            keyboardButton.GetComponent<Image>().sprite = greenKeyboardSprite;

            // Broken
            Text currentKeyboardInputText = GameObject.Find("KeyboardInputText").GetComponent<Text>();

            currentKeyboardInputText.text = "";
        }
    }

    // Executes if the microphone is clicked
    public void OnMicroPhoneClick()
    {
        // Does nothing if the user has currently opened the keyboard
        if (keyboardInputField.activeInHierarchy)
            return;

        // If the microphone is currently not recording, then start recording
        if (!recordingMessage)
            StartMicroPhoneRecording();
        // Otherwise stop the current recording
        else
            StopMicroPhoneRecording();
    }

    // Executes if the speech button is clicked
    public void OnTTSButtonClick()
    {
        // If TTS is enabled then disable
        if (useTTS)
        {
            useTTS = false;

            // Sets the TTS button colour to the idle normal state
            speechTTSButton.GetComponent<Image>().sprite = redTTSButtonSprite;

            // If the Avatar is currently speaking (using TTS) then stop
            if (currentlySpeaking)
                StopTTS();
        }
        // Otherwise enable
        else
        {
            useTTS = true;

            // Sets the TTS button colour to the active green state
            speechTTSButton.GetComponent<Image>().sprite = greenTTSButtonSprite;
        }
    }

    // The following methods load scenes if depending on if the Avatar or Camera button are clicked
    public void OnARClick()
    {
        SceneManager.LoadScene("AR");
    }

    public void OnAvatarClick()
    {
        SceneManager.LoadScene("Avatar");
    }

    public void OnChatbotClick()
    {
        SceneManager.LoadScene("Chatbot");
    }

    // Starts a microphone recording session
    protected void StartMicroPhoneRecording()
    {
        recordingMessage = true;

        // Starts recording with STT
        SpeechToText.instance.StartRecording();

        // Sets the recording start time to the current time
        recordingStartTime = DateTime.Now;

        // Makes microphone information visible
        microphoneRecordingInfoContainer.SetActive(true);

        // Sets the microphone button colour to the active green state
        microphoneButton.GetComponent<Image>().sprite = greenMicrophoneSprite;

#if UNITY_ANDROID

#else
        // Hidens the sound bar animation (which was made visible by the microphone recording info container),
        // as this is not supported on devices that are not android
        soundBarHolder.gameObject.SetActive(false);
#endif
    }

    // Stops a microphone recording session
    protected void StopMicroPhoneRecording()
    {
        recordingMessage = false;

        // Stops recording with STT
        SpeechToText.instance.StopRecording();

        // Makes microphone information hidden
        microphoneRecordingInfoContainer.SetActive(false);

        // Sets the microphone button colour back to the default black
        microphoneButton.GetComponent<Image>().sprite = normalMicrophoneSprite;
    }

    // Reads out the message if TTS is currently enabled
    protected void StartTTSIfActivated(string message)
    {
        if (useTTS)
            TextToSpeech.instance.StartSpeak(message);
    }

    // Stops the current message being read out
    protected void StopTTS()
    {
        TextToSpeech.instance.StopSpeak();
        currentlySpeaking = false;
    }

    // Called when text starts being read out
    public void OnSpeechStart()
    {
        currentlySpeaking = true;
    }

    // Called when text stops being read out
    public void OnSpeechStop()
    {
        currentlySpeaking = false;
    }

    // Method for when the user submits a keyboard message
    public void OnKeyboardSubmit(string message)
    {
        // Sets the keyboard button colour back to the default black
        keyboardButton.GetComponent<Image>().sprite = normalKeyboardSprite;

        // Keyboard input field is made inactive
        keyboardInputField.SetActive(false);

        // Adds new message to conversation and gets a Watson response
        HandleNewUserMessage(message);
    }

    // Method for handling the STT has finished translating a message string
    public void OnSpeechTranslation(string message)
    {
        // If the microphone recording was not stopped manually, then set the recording
        // to false and hide the microphone information container
        if (recordingMessage)
        {
            recordingMessage = false;
            microphoneRecordingInfoContainer.SetActive(false);
        }

        // Adds new message to conversation and gets a Watson response
        HandleNewUserMessage(message);
    }

    // Handles the user message and gets a response, adding these to the current conversation
    protected void HandleNewUserMessage(string message) 
    {
        // Checks that the message is valid
        if (!CheckMessageIsValid(message))
            return;

        // Adds the new message to the conversation
        currentConversation.AddNewMessage(message, true);

        // Renders the user message on the screen
        RenderUserMessage(message);

        // Gets the Watson response message
        string watsonResponseMessage = GetWatsonResponse(message);

        // Render a GPS Map if the response message requires this functionality
        bool useMapForMessage = false;
        if (useMapForMessage)
            RenderMap("");

        // Adds new message to conversation and renders it
        currentConversation.AddNewMessage(watsonResponseMessage, false);

        // Renders the user message on the screen
        RenderChatbotResponseMessage(watsonResponseMessage);

        // Reads out the watson message response
        StartTTSIfActivated(watsonResponseMessage);
    }

    // Subclasses implement how the user message should be rendered on the screen
    protected abstract void RenderUserMessage(string message);

    // Subclasses implement how the chatbot response message should be rendered on the screen
    protected abstract void RenderChatbotResponseMessage(string message);

    // Checks the format and contents of the message to ensure it is valid for parsing to Watson
    protected bool CheckMessageIsValid(string message)
    {
        // Add message checking

        /*if (message.Length < 10)
            return false;
        else if (message.Length > 200)
            return false;*/
        
        return true;
    }

    // Simplifies the message so that it is more clear for when Watson parses it
    protected string SimplifyMessageString(string message)
    {
        // Add message simplification

        return message;
    }

    // Renders a map with GPS Data
    protected void RenderMap(string GPSData)
    {
        // Mapping goes here

    }

    // Gets a response from Watsom based on the message argument
    protected string GetWatsonResponse(string message)
    {
        // Watson exchange goes here

        string defaultMessage = "This is an example of what a response would look like...";

        return defaultMessage;
    }
}

