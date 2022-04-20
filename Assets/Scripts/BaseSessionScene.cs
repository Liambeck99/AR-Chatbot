// The parent class for the AR, Avatar and Chatbot scenes. This class contains
// methods for handling user input (microphone and keyboard) as well as how
// to output responses

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using TextSpeech;

using UnityEngine.Networking;
using System.Net;

using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;


// The main parent class used for all scenes that interact with the AI in the project
// Set to abstract as this class should not be used individually
public abstract class BaseSessionScene : BaseUIScene
{
    [SerializeField]
    private string iamApikey;

    [SerializeField]
    private string serviceUrl;

    [SerializeField]
    private string versionDate;

    [SerializeField]
    private string assistantId;

    // Stores the current data for the session
    protected SessionHandler currentSessionHandler;

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

    // The number of minutes that must have passed since the last message was said for the session
    // to expire and be reset
    protected float timeBeforeRestartSession = 3.0f;

    // Sprites for the two microphone states (Green = active/ Normal= idle)
    public Sprite greenMicrophoneSprite;
    public Sprite normalMicrophoneSprite;

    // Sprites for the two keyboard states (Green = active/ Normal= idle)
    public Sprite greenKeyboardSprite;
    public Sprite normalKeyboardSprite;

    // Sprites for the two TTS button states (Green = active/ Normal= idle)
    public Sprite greenTTSButtonSprite;
    public Sprite redTTSButtonSprite;

    public Sprite blackActiveTTSButtonSprite;
    public Sprite blackDeactiveTTSButtonSprite;

    // Contains whether or not to use the text-to-speech functionality
    protected bool useTTS;

    // Contains whether or not text is currently being spoken
    protected bool currentlySpeaking;

    // Allows the keyboard and microphone buttons to be used
    protected bool allowInputs;

    private AssistantService service;

    private bool createSessionTested = false;
    protected string watsonResponseMessage = null;
    protected string sessionId;

    //Flag which is true when the user is responding to the NLU and not to Watson
    protected bool userIsRespondingToNLU = false;

    //Watson response that indicates an NLU response is required from the user
    protected string watsonNLUPrompt = "So firstly, can you tell me a little bit about your hobbies and interests ";

    protected IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new AssistantService(versionDate, authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        service.CreateSession(OnCreateSession, assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }

    }

    // Configures all data for the scene to work
    protected void ConfigureScene()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());

        watsonResponseMessage = null;

        createSessionTested = false;

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

        // Avatar is not speaking so set to false
        currentlySpeaking = false;

        // Allows microphone and keyboard inputs
        allowInputs = true;

        // Sets colour blind mode if this is active in the settings
        UpdateColoursIfColourBlindMode();

        // Loads in previous session data for correct scene rendering
        ConfigureSessionData();
    }

    protected void CheckIfWatsonHasReturned()
    {
        if (watsonResponseMessage != null)
            HandleWatsonResponse();
    }

    // Converts scene to black and white if colour blind mode was enabled
    protected override void UpdateColoursIfColourBlindMode()
    {
        if (currentSettings.ReturnFieldValue("useColourBlind"))
        {
            // Finds all text objects in the scene
            Text[] allText = FindObjectsOfType<Text>();

            // Sets the green (active) microphone and keyboard sprites to default black sprites (no green animation change)
            greenMicrophoneSprite = normalMicrophoneSprite;
            greenKeyboardSprite = normalKeyboardSprite;

            // TTS button is set to the black active and deactive sprites
            greenTTSButtonSprite = blackActiveTTSButtonSprite;
            redTTSButtonSprite = blackDeactiveTTSButtonSprite;

            // Sets the background to white
            GameObject background = GameObject.Find("Background");

            if (background != null)
                background.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            // An abstract method that is defined in each of the subclass scene scripts, these 
            // Set individual sprites to their colourblind equivalent
            SetColourBlindSprites();
        }
    }

    protected abstract void SetColourBlindSprites();

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


    // Loads in current session data and session conversation if valid and exists
    protected void ConfigureSessionData()
    {

        // Creates a new session handler and passes the field paths to load in the JSON objects
        currentSessionHandler = new SessionHandler(CreateRelativeFilePath("CurrentSessionData"),
                                                   CreateRelativeFilePath("PreviousConversations"),
                                                   CreateRelativeFilePath("CurrentSessionConversation"),
                                                   currentSettings.ReturnFieldValue("AutoUseTTS"));

        // Default message to display on new session
        string welcomeMessage = "Hello there, how can I help you today?";

        // Checks if the number of current messages in the session is more than 0 (indicates there is a session currently taking place)
        if (currentSessionHandler.currentConversation.GetCurrentConversationSize() > 0)
        {
            // If more time has passed since the last message was sent than the ttl, then the session is restarted
            if (hasSessionExpired())
            {
                currentSessionHandler.currentConversation.ResetSessionConversation();
                currentSessionHandler.currentConversation.ResetCurrentConversation();

                currentSessionHandler.currentConversation.AddNewMessage(welcomeMessage, false);

                StartTTSIfActivated(welcomeMessage);
            }
        }

        // Adds the default greetings message if no messages are currently stored in the session
        else
        {
            currentSessionHandler.currentConversation.AddNewMessage(welcomeMessage, false);

            StartTTSIfActivated(welcomeMessage);
        }

        // Sets whether to save messages (after the welcome message has potentially been said)
        // based on the user's settings
        currentSessionHandler.currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        // Automatically uses TTS if ticked in the settings
        useTTS = true;
        if (!currentSessionHandler.ReturnFieldValue("isTTSButtonActive"))
        {
            useTTS = false;
            speechTTSButton.GetComponent<Image>().sprite = redTTSButtonSprite;
        }
        else
            speechTTSButton.GetComponent<Image>().sprite = greenTTSButtonSprite;

    }

    // Checks if the current session has expired and needs to be reset
    protected bool hasSessionExpired()
    {
        // Gets the last message that was sent in the conversation
        Message lastMessageSaved = currentSessionHandler.currentConversation.GetNewMessageAtIndex(
                                   currentSessionHandler.currentConversation.GetCurrentConversationSize() - 1);

        // Gets the datetime for when that message was sent
        DateTime lastTimeSent = DateTime.ParseExact(lastMessageSaved.timeProcessed, ConversationHandler.dateFormatUsed,
                                                    System.Globalization.CultureInfo.InvariantCulture);

        // The number of minutes difference between the current time and the last message that was sent
        float minutesDifference = (float)DateTime.Now.Subtract(lastTimeSent).TotalMinutes;

        // If more time has passed since the last message was sent than the ttl, then the session is restarted
        if (minutesDifference > timeBeforeRestartSession)
            return true;

        // Otherwise the session is still valid
        return false;
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
        UpdateButtonAnimations();
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

    // Updates the keyboard and microphone button animations if either is currently active
    protected void UpdateButtonAnimations()
    {
        // Gets the current milleseconds over 0-2 seconds, this acts as the angle to the sin
        // animation function
        float milleseconds = (DateTime.Now.Second % 2) * 1000 + DateTime.Now.Millisecond;

        // Gets the new scale of the button size
        float scale = (float)Math.Sin(Math.PI * ((float)milleseconds / 4000) * 2.0f) / 6;

        // If the microphone button has been clicked, then set new size
        if (recordingMessage)
            microphoneButton.transform.localScale = new Vector3(scale + 1.0f, scale + 1.0f, 1);

        // If the keyboard button has been clicked, then set new size
        else if (keyboardInputField.activeInHierarchy)
            keyboardButton.transform.localScale = new Vector3(scale + 1.0f, scale + 1.0f, 1);
    }

    // Executes if the keyboard button is clicked
    public void OnKeyboardClick()
    {
        // Does nothing if the user is currently recording a message or inputs are disabled
        if (recordingMessage || !allowInputs)
            return;

        // If the keyboard is active, then deactive the input
        if (keyboardInputField.activeInHierarchy)
        {
            keyboardInputField.SetActive(false);

            // Sets the keyboard button colour back to the default black
            keyboardButton.GetComponent<Image>().sprite = normalKeyboardSprite;

            // Resets button size to 1 (end of animation)
            keyboardButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            // Input is set to active
            keyboardInputField.SetActive(true);

            // Sets the keyboard button colour to the active green state
            keyboardButton.GetComponent<Image>().sprite = greenKeyboardSprite;
        }
    }

    // Executes if the microphone is clicked
    public void OnMicroPhoneClick()
    {
        // Does nothing if the user has currently opened the keyboard or inputs are disabled
        if (keyboardInputField.activeInHierarchy || !allowInputs)
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

            // Session data is updated
            currentSessionHandler.UpdateField("isTTSButtonActive", false);

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

            // Session data is updated
            currentSessionHandler.UpdateField("isTTSButtonActive", true);

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

        // Resets button size to 1 (end of animation)
        microphoneButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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

        // Set button size back to default
        keyboardButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Adds new message to conversation and gets a Watson response
        HandleNewUserMessage(message);

        // Resets the input value back to an empty string (no input)
        keyboardInputField.GetComponent<InputField>().text = "";
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

        // Sets the microphone button colour back to the default black
        microphoneButton.GetComponent<Image>().sprite = normalMicrophoneSprite;

        // Resets button size to 1 (end of animation)
        microphoneButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Adds new message to conversation and gets a Watson response
        HandleNewUserMessage(message);
    }

    // Handles the user message and gets a response, adding these to the current conversation
    protected void HandleNewUserMessage(string message)
    {
        // Checks that the message is valid
        string messageError = CheckMessageIsValid(message);
        if (messageError.Length != 0) {
            ErrorHandler(messageError);
            return;
        }
       
        // Adds the new message to the conversation
        currentSessionHandler.currentConversation.AddNewMessage(message, true);

        // Renders the user message on the screen
        RenderUserMessage(message);

        Debug.Log("User Message: " + message);

        // If the current language is not english, then translate the user message to 
        // English so that the Watson translator can properly translate the message
        if (currentSettings.GetLanguage() != "English")
        {
            string model = "";

            if (languages.TryGetValue(currentSettings.GetLanguage(), out model))
                model = model;

            message = TranslateString(message, model, "en"); 
        }

        //  if the user is not responding to NLU then run watson as usual
        if (userIsRespondingToNLU == false)
            GetWatsonResponse(message);
     
        //  pass user input through to the NLU
        else
        {
            StartCoroutine(RecommenderService(message));
        }
    }

    protected IEnumerator RecommenderService(string message)
    {
        //  get NLU to process user input and extract keywords
        NaturalLanguageUnderstanding newNlu = new NaturalLanguageUnderstanding();
        Runnable.Run(newNlu.NLURun(message));

        yield return new WaitForSeconds(2);

        //run recommender system
        RecommenderSystem newRecSys = new RecommenderSystem();
        List<RecommenderSystem.NLUReturnValues3> recommendedSocietityList = new List<RecommenderSystem.NLUReturnValues3>();

        recommendedSocietityList = newRecSys.loadNLUJSON();

        //construct response message to show to user of recommended societies
        string societyResponseMessage = "Here are some of the societies I reccomend:\n";

        foreach (RecommenderSystem.NLUReturnValues3 o in recommendedSocietityList)
        {
            //societyResponseMessage += "\n\n" + o.keyword + string.Format("   Score({0:0.00})", o.score);
            societyResponseMessage += "\n\n" + o.keyword;

        }

        //  reset flag to allow user to communicate with watson again
        userIsRespondingToNLU = false;

        watsonResponseMessage = societyResponseMessage;
    }

    protected void GetWatsonResponse(string userMessage)
    {

        var resultstring = "";

        var url = "https://api.eu-gb.assistant.watson.cloud.ibm.com/instances/40f7db54-e31f-485a-86bc-d8d32deede56/v2/assistants/9e981394-1da2-4f1e-a342-9841e27dedff/sessions/" + sessionId + "/message?version=2020-09-24";

        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
        httpRequest.Method = "POST";

        httpRequest.ContentType = "application/json";
        httpRequest.Headers["Authorization"] = "Basic YXBpa2V5OmNnMFRFWEVUMm82cm5PYUhrblg4Nm5Rdkk3dWRpTlN2TU1wTGFmUEc2QW55";

        var data = "{\"input\": {\"text\": \"" + userMessage + "\"}}";

        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
        {
            streamWriter.Write(data);
        }

        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            resultstring = streamReader.ReadToEnd();
        }

        if (!resultstring.Contains("suggestions"))
        {
            string[] res = resultstring.Split('{');
            foreach (string s in res)
            {
                if (s.Contains("text") == true)
                {
                    string[] rez = s.Split(':');
                    foreach (string ss in rez)
                    {
                        if (ss.Contains("user_id"))
                        {
                            resultstring = ss;
                            break;
                        }
                    }
                }
            }
        }

        else
        {
            string[] res = resultstring.Split('{');
            resultstring = "Did you mean:";
            foreach (string s in res)
            {
                if (s.Contains("label") == true)
                {
                    int firstColon = s.IndexOf(":");
                    int firstComma = s.IndexOf(",");
                    resultstring += "\n\n" + s.Substring(firstColon+1, firstComma-firstColon-1);
                }
            }
        }

        watsonResponseMessage = resultstring.Split('}')[0].Replace("\"", string.Empty);
    }

    private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {

        Debug.Log("Response Object: " + response.Response);
        Debug.Log("Is instance: " + (response != null).ToString());

        if (error == null)
        {
            Log.Debug("ExampleCallback", "Response received: {0}", response.Response);
        }
        else
        {
            Log.Debug("ExampleCallback", "Error received: {0}, {1}, {3}", error.StatusCode, error.ErrorMessage, error.Response);
        }

        if (response.Result.Output.Generic[0].ResponseType == "text")
            watsonResponseMessage = response.Result.Output.Generic[0].Text;
        else
        {
            watsonResponseMessage = response.Result.Output.Generic[0].Title;
            int numSuggestions = response.Result.Output.Generic[0].Suggestions.Count;
            for (int i = 0; i < numSuggestions; i++)
            {
                watsonResponseMessage += "\n\n";
                watsonResponseMessage += response.Result.Output.Generic[0].Suggestions[i].Label;
            }
        }

    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("Session ID: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

    protected void ResetWatsonResponse()
    {
        watsonResponseMessage = null;
        //createSessionTested = false;
    }

    protected void HandleWatsonResponse()
    {
        Debug.Log("Watson response: " + watsonResponseMessage);

        //if the response is the prompt for NLU then we want to temporarily disable watson
        if (watsonResponseMessage == watsonNLUPrompt)
            userIsRespondingToNLU = true;

        // If the current language is not english, then translate the watson response message
        // back to the current language
        if (currentSettings.GetLanguage() != "English")
        {
            string model = "";

            if (languages.TryGetValue(currentSettings.GetLanguage(), out model))
                model = model;

            watsonResponseMessage = TranslateString(watsonResponseMessage, "en", model);
        }

        // Adds new message to conversation and renders it
        currentSessionHandler.currentConversation.AddNewMessage(watsonResponseMessage, false);

        // Reads out the watson message response
        StartTTSIfActivated(watsonResponseMessage);

        // Renders the user message on the screen
        RenderChatbotResponseMessage(watsonResponseMessage);

        ResetWatsonResponse();
    }

    // Subclasses implement how the user message should be rendered on the screen
    protected abstract void RenderUserMessage(string message);

    // Subclasses implement how the chatbot response message should be rendered on the screen
    protected abstract void RenderChatbotResponseMessage(string message);

    // Checks the format and contents of the message to ensure it is valid for parsing to Watson
    // Returns an empty string if the message is valid, otherwise returns an error string to display
    protected string CheckMessageIsValid(string message)
    {
        // Checks that the message input length is between 1-200 characters
        if (message.Length < 1 || message.Length > 200)
            return "Message must be between 1-200 characters";

        // Since the following check ensures that no special characters are allowed in the
        // message, a whitelist of valid special characters are instantiated which are allowed
        // in the message
        List<char> validSpecialCharacters = new List<char> {'.', '!', '?', ',', ' ', '\'', '\"', '\n'};

        // Checks if any special characters, that are not in the valid list, are in the message, if 
        // so then return an error
        if (message.Any(ch => !Char.IsLetterOrDigit(ch) && !validSpecialCharacters.Contains(ch)))
            return "Message must not contain special characters";

        // Returns an empty string to symbolise the message is valid
        return "";
    }

    // Gets a response from Watsom based on the message argument
    /*protected string GetWatsonResponse(string message)
    {
        // Watson exchange goes here

        string defaultMessage = "This is an example of what a response would look like...";

        RecommenderSystem recommenderSystemHandler = new RecommenderSystem();

        return defaultMessage;
    }*/
}

