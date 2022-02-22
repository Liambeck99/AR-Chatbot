// Script that is attached to the Avatar scene

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;

using Random = UnityEngine.Random;

public class AvatarScene : BaseSessionScene
{

    // Sprites used for colour blind setting
    public Sprite blackSwitchToARSprite;
    public Sprite blackSwitchToChatbotSprite;

    // Handles avatar animations and audio
    private AvatarMeshHandler meshHandler;

    // Stores the time that a last 'random' animation occured, used for 
    // ensuring that random animations happen sparingly
    private DateTime timeOfLastRandomAnimation;

    // The main avatar model
    private GameObject avatarModel;

    // Flag for if the avatar should perform the intro walking animation
    private bool inIntroAnimation;

    // Stores how long the avatar walking animation should take
    private float introAnimationDurationTime;

    // The 3D position that the avatar should end up in after every animation
    private Vector3 FinalPosition;

    // The 3D position that the avatar starts in when performing the intro walking animation
    private Vector3 BeginningPosition;

    // The 3D position that the avatar should transition to when explaining information
    private Vector3 SidePosition;

    // The chatbot object that displays the information and extends depending on the message size
    GameObject chatbotSpeechBubble;

    // The parent that holds both the speechbubble and text message
    GameObject responseContainer;

    // The parent of the 3D speech bubble, this scales the speech bubble in the downwards y direction
    // depending on the size of the message
    GameObject speechBubbleScaler;

    // The message that is to be displayed on the avatar speech bubble
    TextMeshPro responseText;

    // Flag for if the avatar should perform the animation for showing the speech bubble and 
    // transitoning to the side
    bool executeSpeechBubbleAnimation;

    // Determines whether the animation should be reversed or not (if true, then the speech bubble 
    // is removed and the avatar returns to the default position)
    bool reverseSpeechBubbleAnimation;

    // Sets the time when the animation was set, this is used for determining what position the avatar 
    // and speech bubble size should be in the current frame
    float speechBubbleAnimationStartTime;

    // Determines how long the speech bubble animation should occur for
    float speechBubbleAnimationDurationTime;

    // Determines the least amount of seconds to wait between every random animation
    int secondsToWaitBetweenRandomAnimations;

    // Determines how many characters should be 'read out' per second. Essentially determining how 
    // long the avatar should explain a message for
    int charsPerSecond;

    // Defines the chances that a random animation should occur if the avatar is in the idle state
    int chanceOfRandomAnimation;
    
    public bool useWeatherBasedOnLocation;

    public Material sunriseSkyBox;
    public Material daySkyBox;
    public Material sunsetSkyBox;
    public Material nightSkyBox;
    public Material snowSkyBox;
    public Material rainSkyBox;

    public string openWeatherAPIKey;

    private WeatherHandler weatherHandler;

    bool weatherConfigured;

    private GameObject MainTerrain;

    private GameObject SnowEnvironment;

    private GameObject RainEnvironment;

    private GameObject DrizzlePrefab;
    private GameObject RainPrefab;
    private GameObject ThunderstormPrefab;

    private void Start()
    {
        // Configures the scene correctly
        ConfigureScene();

        // Finds avatar model in the scene
        avatarModel = GameObject.Find("AvatarModel");

        // Gets avatar animation handler
        meshHandler = avatarModel.GetComponent<AvatarMeshHandler>();

        // Sets the time that a random animation occured 4 seconds in the past, this gives
        // a little time before it is possible for a random animation to occur
        timeOfLastRandomAnimation = DateTime.Now.AddSeconds(-4);

        // If Text-To-Speech is enabled, then this means that audio should be played, so
        // the avatar audio is set to volume = 1, otherwise it is muted (volume = 0)
        if (useTTS)
            meshHandler.UnmuteAudio();
        else
            meshHandler.MuteAudio();

        // Defines how long the intro animation should last
        introAnimationDurationTime = 2.5f;

        // Defines the final position that the avatar should end up after the intro animation has 
        // completed. In this case, it is in front of the camera
        FinalPosition = new Vector3(-0.05f, -2.0f, -0.5f);

        // Defines where the avatar should be at the start of the intro animation
        BeginningPosition = new Vector3(-0.05f, -0.7f, 20.0f);

        // The position that the avatar should transition to when explaining a returned message
        SidePosition = new Vector3(1.2f, -2.0f, -1.0f);

        // Sets the least amount of seconds to wait for when the avatar performs a random animation (if idle)
        secondsToWaitBetweenRandomAnimations = 10;

        // There is a session in progress, therefore do not play intro animation as it has already been
        // played once before
        if (currentSessionHandler.currentConversation.GetCurrentConversationSize() > 1)
        {
            inIntroAnimation = false;

            // Position is automatically set to the end of the animation (in front of the camera)
            avatarModel.transform.position = FinalPosition;

            // Avatar is set to the IDLE animation
            meshHandler.FinishWalkAnimation(0.01f, false);
        }

        // There is no session therefore play the intro animation
        else
            inIntroAnimation = true;

        // Retrieves speech bubble and text objects
        chatbotSpeechBubble = GameObject.Find("ChatbotSpeechBubble");
        responseText = GameObject.Find("ResponseText").GetComponent<TextMeshPro>();

        // Parent object of the speechbubble and text, this is used for handling whether all of
        // the objects should be active or not
        responseContainer = GameObject.Find("ResponseContainer");

        // Parent object of the speech bubble, used for correct Y scaling when changing the size of
        // the speech bubble. This must be here if the speechbubble is to correctly scale in only downwards
        // in the y direction
        speechBubbleScaler = GameObject.Find("SpeechBubbleScaler");

        // Speech bubble is automatically deactivated
        responseContainer.SetActive(false);

        // This defines whether the speech bubble animation (speech bubble grows from size 0 to the correct size, and
        // the avatar is transitioned to the left) is currently playing. This animation can also be reversed, this is 
        // defined by 'reverseSpeechBubbleAnimation' (if true then the animation is performed in reverse)
        executeSpeechBubbleAnimation = false;
        reverseSpeechBubbleAnimation = false;

        // Defines how long the speech bubble animation should take
        speechBubbleAnimationDurationTime = 0.75f;

        // Default sets the animation time to the current time. This is used for measuring how long the 
        // animation has occured for
        speechBubbleAnimationStartTime = Time.time;

        // Defines how long the message should be displayed on screen for. E.g: charsPerSecond = 7 means that if a message is
        // 21 characters long, then it will be shown for 3 seconds
        charsPerSecond = 7;

        // Determines the chances of a random animation occuring out of 10000 (per frame)- if the avatar is idle
        chanceOfRandomAnimation = 10;

        SetDefaultEnvironment();

        if (useWeatherBasedOnLocation)
        {
            weatherConfigured = false;

            weatherHandler = GameObject.Find("WeatherHandler").GetComponent<WeatherHandler>();

            weatherHandler.GetWeatherInfo();
        }
        else
        {
            weatherConfigured = true;

            GameObject.Find("LoadingBackground").SetActive(false);
        }
    }

    private void Update()
    {
        if (!weatherConfigured)
        {
            if (weatherHandler.HasFinishedSearch())
            {
                ConfigureWeatherAndLightingSystem();
                GameObject.Find("LoadingBackground").SetActive(false);
                weatherConfigured = true;
            }
        }

        else
        {
            UpdateScene();

            // If the avatar is idle, then it has the chance to perform a random animation
            PerformRandomAnimation();

            // Rotates the skybox by a factor of the current time, this gives the effect that the clouds are moving
            RenderSettings.skybox.SetFloat("_Rotation", Time.time);

            // Performs the appropriate animations if required
            PerformAnimations();
        }
    }

    private void SetDefaultEnvironment()
    {
        MainTerrain = GameObject.Find("Terrain");

        SnowEnvironment = GameObject.Find("Snow_Environment");

        RainEnvironment = GameObject.Find("Rain_Environment");

        DrizzlePrefab = GameObject.Find("DrizzlePrefab");
        RainPrefab = GameObject.Find("RainPrefab");
        ThunderstormPrefab = GameObject.Find("ThunderstormPrefab");

        SnowEnvironment.SetActive(false);
        RainEnvironment.SetActive(false);

        DrizzlePrefab.SetActive(false);
        RainPrefab.SetActive(false);
        ThunderstormPrefab.SetActive(false);

        RenderSettings.skybox = sunsetSkyBox;
    }

    private void ConfigureWeatherAndLightingSystem()
    {
        float lightIntensity = 1.0f;

        float wind = weatherHandler.GetWindSpeed();

        float windSpeedMain = Math.Min((float)wind / 4.0f, 11.0f);
        float windTurbulance = Math.Max((float)wind / 15.0f, 0.5f);

        WindZone windZone = GameObject.Find("PF CTI Windzone").GetComponent<WindZone>();

        windZone.windMain = windSpeedMain;
        windZone.windTurbulence = windTurbulance;

        string weatherType = weatherHandler.GetWeatherType();

        Debug.Log("Current Weather: " + weatherType);
        Debug.Log("Current Wind: " + wind);

        if (weatherType == "Snow")
        {
            MainTerrain.SetActive(false);

            SnowEnvironment.SetActive(true);

            RenderSettings.fogDensity = 0.04f;
            RenderSettings.fogColor = Color.white;

            RenderSettings.skybox = snowSkyBox;

            lightIntensity *= 1.40f;
        }

        else if (weatherType == "Drizzle")
        {
            RainEnvironment.SetActive(true);

            DrizzlePrefab.SetActive(true);

            lightIntensity *= 0.65f;
        }

        else if (weatherType == "Rain")
        {
            RainEnvironment.SetActive(true);

            RainPrefab.SetActive(true);

            RenderSettings.skybox = rainSkyBox;

            lightIntensity *= 0.45f;
        }

        else if(weatherType == "Thunderstorm")
        {
            RainEnvironment.SetActive(true);

            ThunderstormPrefab.SetActive(true);

            RenderSettings.skybox = rainSkyBox;

            lightIntensity *= 0.3f;
        }

        else if(weatherType == "Clouds")
        {

        }

        else if(weatherType == "Mist" || weatherType == "Fog")
        {
            RenderSettings.fogDensity = 0.06f;
            RenderSettings.fogColor = Color.white;
        }

        RenderSettings.ambientIntensity = lightIntensity;
    }

    // Checks if any of the animation flags are set, if so then perform the current frame of animation
    private void PerformAnimations()
    {
        // Used in all animations and calculates how far through the animation the current frame is,
        // based on the current time and the start time of the animation. This is then used to 
        // determine the position/sizes that the objects should be based on a start and end position/size
        float percentage;

        // If the intro animation is set to true, then perform the animation
        if (inIntroAnimation)
        {
            // Calculates the percentage that the current frame is through the animation
            percentage = (introAnimationDurationTime - Time.time) / introAnimationDurationTime;

            // If the percentage is not less than 0, then calculate the position between the start and end positions
            // based on the current percentage. This gives a linear transition between the beginning and final position
            // based on the current time frame
            if (percentage > 0)
                avatarModel.transform.position = (FinalPosition * (1 - percentage)) + (BeginningPosition * (percentage));

            // If the percentage is less than 0, then the animation has finished. Set the intro animation flag to false,
            // and perform the correct animation for finishing the intro animation (this is waving and then idle animation)
            else
            {
                inIntroAnimation = false;
                meshHandler.FinishWalkAnimation(0.5f, true);
            }
        }

        // If the speech bubble animation is set to true and it is not the reverse animation, then perform the animation
        if (executeSpeechBubbleAnimation && !reverseSpeechBubbleAnimation)
        {

            // Calculates the percentage that the avatar is through the animation (from 0-1), based on the animaton duration time
            // and the amount of time that has passed since the start of the animation
            percentage = ((speechBubbleAnimationStartTime + speechBubbleAnimationDurationTime) - Time.time) / speechBubbleAnimationDurationTime;

            // Returns the amount of lines that are occupied by the returned response message, this is used for determining the final size of the speech
            // bubble container
            int lineCount = responseText.textInfo.lineCount;

            // If percentage is greater than 0, then the animation is still taking place
            if (percentage > 0)
            {
                speechBubbleScaler.transform.localScale = new Vector3(1.0f * (1 - percentage),
                                                                     ((float)lineCount / 7.0f) * (1 - percentage),
                                                                     1.0f * (1 - percentage));

                avatarModel.transform.position = (SidePosition * (1 - percentage)) + (FinalPosition * (percentage));
            }

            // Otherwise, the animation has finished 
            else
            {
                // The text in the speech bubble should be shown, and is set to an transparancy is set to 1 (not see-through)
                responseText.color = new Color32(255, 255, 255, 255);

                // Animation flag is set to false, meaning that the animation should not continue
                executeSpeechBubbleAnimation = false;
            }
        }

        // If the speech bubble animation is set to true and it is the reverse animation, then perform the animation
        if (executeSpeechBubbleAnimation && reverseSpeechBubbleAnimation)
        {
            // Calculates the percentage that the avatar is through the animation(from 0 - 1), based on the animaton duration time
            // and the amount of time that has passed since the start of the animation
            percentage = ((speechBubbleAnimationStartTime + speechBubbleAnimationDurationTime) - Time.time) / speechBubbleAnimationDurationTime;

            // If percentage is greater than 0, then the animation is still taking place
            if (percentage > 0)
                
                // Transition the avatar position back to its default position in front of the camera
                avatarModel.transform.position = (SidePosition * percentage) + (FinalPosition * (1 - percentage));

            // Otherwise the animation has finished, and set the flag to false
            else
                executeSpeechBubbleAnimation = false;
        }
    }

    // Executed once the user has entered a message, if this is the case, then toggle the animation phase
    // so that the avatar is in the 'thinking' state (waiting for a response message from watson)
    protected override void RenderUserMessage(string message)
    {
        meshHandler.ToggleAnimationPhase();
    }

    // Executed when a response has been given from watson
    protected override void RenderChatbotResponseMessage(string message)
    {
        // Determines how long the avatar should explain the response message and how thus how long the 
        // speech bubble should appear on the screen
        int timeToWait = message.Length / charsPerSecond;

        // The speechbubble and response text are set to appear in the environment
        responseContainer.SetActive(true);

        // Message is set as the text in the environment
        responseText.text = message;

        // Sets that the speech bubble animation should occur, and sets the start time of the animation 
        // to the current frame
        executeSpeechBubbleAnimation = true;
        reverseSpeechBubbleAnimation = false;
        speechBubbleAnimationStartTime = Time.time;

        // Response text is set to transparent; the text should only show once the animation is complete
        responseText.color = new Color32(255, 255, 255, 0);

        // Performs the animation; this is set as a coroutine as the function needs to pause while the 
        // avatar is 'explaining' the returned text (the speech bubble is shown in the environment). Hence
        // once the function will wait the appropriate amount of seconds before proceeding, and this can only
        // be done in a Couroutine
        StartCoroutine(RenderExplinationAnimationWithDelay(timeToWait));
    }

    // Performs the avatar explination animation and displays the speech bubble to the user
    private IEnumerator RenderExplinationAnimationWithDelay(int timeToWait)
    {
        // Does not allow the user to enter in new messages until the current message has 
        // properly been rendered on the screen and 'explained'
        allowInputs = false;

        // Avatar animation is transitioned from the 'thinking' animation to the 'explination' animation
        meshHandler.ToggleAnimationPhase();

        // Function waits the appropraite amount of seconds so that the user can correctly read the 
        // returned message from Watson
        yield return new WaitForSeconds(timeToWait);

        // Once the appropriate amount of seconds has expired for the speechbubble to be shown,
        // then the avatar should transition back to the default idle state
        meshHandler.ToggleAnimationPhase();

        // The speech bubble text is set to empty since there is now no message to show
        responseText.text = "";

        // Sets the speech bubble animation flags so that the reverse of the animation occurs,
        // this means that the avatar transitions back to its default position in front of the camera
        executeSpeechBubbleAnimation = true;
        reverseSpeechBubbleAnimation = true;
        speechBubbleAnimationStartTime = Time.time;

        // Speech bubble and text container is hidden in the environment
        responseContainer.SetActive(false);

        // The user is now allowed to enter another question (now that the explination of the previous
        // input has finished)
        allowInputs = true;
    }

    // Executed if the colour blind mode is set
    protected override void SetColourBlindSprites()
    {
        // Avatar and chatbot icons are set to their appropriate black-and-white alternatives
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToARSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }

    // Executed after every update frame and determines if the avatar should perform a 'random' animation
    private void PerformRandomAnimation()
    {
        
        // Determines the amount of seconds that have passed since when the last random animation occured
        int secondsDifference = (int)DateTime.Now.Subtract(timeOfLastRandomAnimation).TotalSeconds;

        // If the amount of seconds since the last random animation is less than the set minimum for
        // a random animation to occur, then return and no animation occures
        if (secondsDifference < secondsToWaitBetweenRandomAnimations)
            return;

        // If a random number is selected between 0-10000 that is lower than the chance of a 
        // random animation to occur, then play the animaton
        if (Random.Range(0, 10000) < chanceOfRandomAnimation)
        {
            // A random animation is played if the avatar is currently in the idle animation
            meshHandler.PerformRandomAnimationIfIdle();

            // Time that the last random animation occured is set to the current time
            timeOfLastRandomAnimation = DateTime.Now;
        }
    }

    // Executed if the user clicks the tts button (aka the audio button in the UI)
    public void OnTTSButtonClickAvatar()
    {
        // If audio is currently set to play, then set any audio that is currently playing from the avatar
        // to mute (since the user has clicked to mute any audio)
        if (useTTS)
            meshHandler.MuteAudio();

        // Otherwise unmute any audio
        else
            meshHandler.UnmuteAudio();

        // Method defined in the parent class that handles TTS audio if the button is clicked
        OnTTSButtonClick();
    }
     
}