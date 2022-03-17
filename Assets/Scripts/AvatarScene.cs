// Script that is attached to the Avatar scene

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;

using Random = UnityEngine.Random;

public class AvatarScene : BaseAvatarScene
{
    // Sprites used for colour blind setting
    public Sprite blackSwitchToARSprite;
    public Sprite blackSwitchToChatbotSprite;

    // Flag for if the avatar should perform the intro walking animation
    private bool inIntroAnimation;

    // The 3D position that the avatar starts in when performing the intro walking animation
    protected Vector3 BeginningPosition;

    // Stores how long the avatar walking animation should take
    private float introAnimationDurationTime;
    
    // Sets whether to use the different weather environments based on the weather in Leeds
    public bool useWeatherBasedOnLocation;

    // The different material skybox objects that will be used for the different environments/times of the day
    public Material sunriseSkyBox;
    public Material daySkyBox;
    public Material sunsetSkyBox;
    public Material nightSkyBox;
    public Material snowSkyBox;
    public Material rainSkyBox;

    // Weatherhandler object that retrieves the correct weather data to be used for the different environment
    private WeatherHandler weatherHandler;

    // Flag for determining whether the weather data has been loaded yet
    bool weatherConfigured;
    
    // The main terrain used in the scene
    private GameObject MainTerrain;

    // Different weather environments
    private GameObject CloudEnvironment;
    private GameObject SnowEnvironment;
    private GameObject RainEnvironment;

    // Different rain prefabs for the three different environments: drizzle, rain and thunderstorm
    private GameObject DrizzlePrefab;
    private GameObject RainPrefab;
    private GameObject ThunderstormPrefab;

    // The lighting intensity to use for the scene
    private float currentSceneLightIntensity;

    // Material shader that is used for the background clouds in the scene
    public Material cloudMaterial;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());

        // Configures the scene correctly
        ConfigureScene();

        // Configures all avatar properties correctly
        ConfigureAvatar();

        // Sets the time that a random animation occured 4 seconds in the past, this gives
        // a little time before it is possible for a random animation to occur
        timeOfLastRandomAnimation = DateTime.Now.AddSeconds(-4);

        // Defines how long the intro animation should last
        introAnimationDurationTime = 2.5f;

        // Defines where the avatar should be at the start of the intro animation
        BeginningPosition = new Vector3(-0.05f, -0.88f, 20.0f);

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

        // Sets the default environment values, so that if weather enviornments are not used then the scene is still
        // loaded correctly
        SetDefaultEnvironment();

        // If it has been set to alter the environment based on the weather
        if (useWeatherBasedOnLocation)
        {
            // Set that the weather has not been configured
            weatherConfigured = false;

            // Get weather handler from the scene and get the required weather data
            weatherHandler = GameObject.Find("WeatherHandler").GetComponent<WeatherHandler>();
            weatherHandler.GetWeatherInfo();
        }
        else
        {
            // Since weather should not be used, set that is has been configured
            weatherConfigured = true;

            // Weather data is not to be loaded so deactivate the loading background
            GameObject.Find("LoadingBackground").SetActive(false);
        }
    }

    private void Update()
    {
        // If weather has not been configured yet
        if (!weatherConfigured)
        {
            // Check if the weather data has correctly been retrieved fromt the API
            if (weatherHandler.HasFinishedSearch())
            {
                // Configure the weather environments if weather data was correctly returned
                if (weatherHandler.HasCorrectlyRetrievedWeatherData())
                    ConfigureWeatherAndLightingSystem();

                // Perform an update on the avatar (e.g. animations)
                UpdateAvatar();

                // Weather data has been loaded so remove the loading background
                GameObject.Find("LoadingBackground").SetActive(false);

                weatherConfigured = true;
            }
        }

        // Weather has been configured, so just perform the usual updates on the scene
        else
        {
            UpdateScene();

            // Rotates the skybox by a factor of the current time, this gives the effect that the clouds are moving
            RenderSettings.skybox.SetFloat("_Rotation", Time.time);

            // Perform an update on the avatar (e.g. animations)
            UpdateAvatar();
        }

        if (watsonResponseMessage != null)
            HandleWatsonResponse();
    }

    private void UpdateAvatar()
    {
        // If the avatar is idle, then it has the chance to perform a random animation
        PerformRandomAnimation();

        // Performs the explination animation and speechbubble animation if set
        PerformResponseAnimationsIfSet();

        // Performs the intro animation if set
        PerformIntroAnimationIfSet();
    }

    // Sets default environment values (so that if the weather is not used, then the scene will still be loaded)
    private void SetDefaultEnvironment()
    {
        // Gets important environment objects from the scene
        MainTerrain = GameObject.Find("Terrain");

        CloudEnvironment = GameObject.Find("Cloud_Environment");
        SnowEnvironment = GameObject.Find("Snow_Environment");
        RainEnvironment = GameObject.Find("Rain_Environment");

        DrizzlePrefab = GameObject.Find("DrizzlePrefab");
        RainPrefab = GameObject.Find("RainPrefab");
        ThunderstormPrefab = GameObject.Find("ThunderstormPrefab");

        // Deactivate all environments that are weather related
        CloudEnvironment.SetActive(false);
        SnowEnvironment.SetActive(false);
        RainEnvironment.SetActive(false);

        DrizzlePrefab.SetActive(false);
        RainPrefab.SetActive(false);
        ThunderstormPrefab.SetActive(false);

        // Background clouds are automatically set to 'clear'
        // (density = 2 means that no clouds will be shown in that material shader)
        cloudMaterial.SetFloat("_Density", 2.0f);

        // Set defaut skybox to sunset (as it looks the nicest)
        RenderSettings.skybox = sunsetSkyBox;
    }

    // Executed when weather is to be used and sets all environment objects to their correct values
    private void ConfigureWeatherAndLightingSystem()
    {
        // Gets the current sunset and sunrise epoch times, and determines which skybox to used based on the 
        // current time of day
        TransformSkyBoxDependingOnTimeOfDay(weatherHandler.GetSunriseEpoch(), weatherHandler.GetSunsetEpoch());

        // Gets the current wind speed and alters wind zone environment objects to correctly correlate with the 
        // wind speed in Leeds
        TransformSceneWithWind(weatherHandler.GetWindSpeed());

        // Gets the current weather type (e.g. clouds, rain, mist) and sets activates the correct environment 
        // based on the current weather type in Leeds
        TransformSceneWithWeather(weatherHandler.GetWeatherType());

        // Gets the current percentage of clouds in the sky and alters the clouds in the environment to 
        // correctly show this
        TransformSceneWithClouds(weatherHandler.GetCloudPercentage());

        Debug.Log("Current Weather: " + weatherHandler.GetWeatherType());
        Debug.Log("Current Wind: " + weatherHandler.GetWindSpeed());
        Debug.Log("Current Cloud Percentage: " + weatherHandler.GetCloudPercentage());

        // Sets the final scene ambient intensity to the calculated light intensity to use (based on the weather)
        RenderSettings.ambientIntensity = currentSceneLightIntensity;
    }

    // Alters the skybox based on the current time compared to when the sunrises and sunsets
    private void TransformSkyBoxDependingOnTimeOfDay(int sunriseEpochTime, int sunsetEpochTime)
    {
        // Gets the Linux epoch start time in seconds 
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int currentEpochTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        // Determines how long after the sunrise or sunset time starts to show their respective skybox. 
        // E.g. An hour would mean that between the start of the sunrise and one hour after that, 
        // set the skybox to the sunrise material
        int sunsetAndSunriseDurations = 60 * 60;

        // If the current epoch time is less than the sunrise or after the sunset (plus duration time), then this 
        // means that it is currently night time. Therefore, set lighting to a low value and set the skybox to the night material
        if (currentEpochTime <= sunriseEpochTime || currentEpochTime >= (sunsetEpochTime + sunsetAndSunriseDurations))
        {
            currentSceneLightIntensity = 0.4f;
            RenderSettings.skybox = nightSkyBox;
        }

        // Otherwise, if the time is inbetween the sunrise and the sunrise duration, then display the sunrise skybox and set 
        // appropriate lighting ambient intensity
        else if (currentEpochTime < (sunriseEpochTime + sunsetAndSunriseDurations))
        {
            currentSceneLightIntensity = 0.8f;
            RenderSettings.skybox = sunriseSkyBox;
        }

        // Else, if the current time is inbetween the end of the sunrise but less than the sunset time, then it is 
        // the middle of the day
        else if (currentEpochTime < sunsetEpochTime)
        {
            currentSceneLightIntensity = 1.1f;
            RenderSettings.skybox = daySkyBox;
        }

        // If it is not any of the above times, then it is currently sunset and the appropriate skybox should be set
        else
        {
            currentSceneLightIntensity = 0.8f;
            RenderSettings.skybox = sunsetSkyBox;
        }
    }

    // Cloud density in the scene is altered to mimic the current cloud percentage in Leeds
    private void TransformSceneWithClouds(float cloudPercentage)
    {
        // If the cloud percentage is less than 0.15, then the sky is effectively clear, and 
        // simply return, keeping the cloud environment deactivated
        if (cloudPercentage < 0.15f)
            return;

        CloudEnvironment.SetActive(true);

        // The cloud percentage is converted into a value between 2-0, with 0 being full density.
        // This is because these are the values that are used in the shader material
        float cloudDensity = 2.0f - (cloudPercentage * 2.0f);

        // Shader material is updated with the new cloud density
        cloudMaterial.SetFloat("_Density", cloudDensity);
    }

    // The current windzone in the scene is updated to mimic the wind in Leeds
    private void TransformSceneWithWind(float windSpeed)
    {
        // Based on the current wind speed in Leeds, an estimation for the equivalent for
        // the virtual environment is calculated. The current maximum wind speed is set 11,
        // since anymore than this would result in essentially horizontal trees
        float windSpeedMain = Math.Min((float)windSpeed / 3.0f, 11.0f);
        float windTurbulance = Math.Max((float)windSpeed / 15.0f, 0.5f);

        // Windzone is updated with the wind speeds
        WindZone windZone = GameObject.Find("PF CTI Windzone").GetComponent<WindZone>();

        windZone.windMain = windSpeedMain;
        windZone.windTurbulence = windTurbulance;
    }

    // The scene is updated so that the current weather type in Leeds correlates to the 
    // weather environment to use in the scene
    private void TransformSceneWithWeather(string weatherType)
    {
        switch (weatherType)
        {
            // It is currently snowing in Leeds
            case "Snow":
                
                // Hide the current terrain and use the snow terrain instead
                MainTerrain.SetActive(false);
                SnowEnvironment.SetActive(true);

                // Set heavy fog that has a white tint to it (mimics snow colour)
                RenderSettings.fogDensity = 0.036f;
                RenderSettings.fogColor = Color.white;

                // Set the skybox to the snow skybox material
                RenderSettings.skybox = snowSkyBox;

                // Increase the light intensity in the scene, this is to mimic how light reflects
                // heavily off of snow, making the everything seem brighter
                currentSceneLightIntensity *= 1.40f;
                break;

            // Currently (slightly) raining in Leeds
            case "Drizzle":
                
                // Activate the drizzle rain prefab (low amount of rain particles)
                RainEnvironment.SetActive(true);
                DrizzlePrefab.SetActive(true);

                // Reduce lighting since it is raining
                currentSceneLightIntensity *= 0.65f;
                break;

            // Currently raining in Leeds
            case "Rain":

                // Activate the rain prefab (medium amount of rain particles)
                RainEnvironment.SetActive(true);
                RainPrefab.SetActive(true);

                // Use the rain skybox since the bright sky should not be used
                RenderSettings.skybox = rainSkyBox;

                // Reduce lighting since it is raining
                currentSceneLightIntensity *= 0.45f;
                break;

            // There is currently a storm in Leeds
            case "Thunderstorm":

                // Activate the storm prefab (large amount of rain particles)
                RainEnvironment.SetActive(true);
                ThunderstormPrefab.SetActive(true);

                // Use the rain skybox since the bright sky should not be used
                RenderSettings.skybox = rainSkyBox;

                // Reduce lighting since it is raining
                currentSceneLightIntensity *= 0.3f;
                break;

            // There is currently a lot of fog in Leeds
            case "Fog":

                // Set the fog density to high and change the colour of the fog to white
                RenderSettings.fogDensity = 0.06f;
                RenderSettings.fogColor = Color.white;
                break;
            
            // There is currently some mist in Leeds
            case "Mist":

                // Set the fog density to medium and change the colour of the fog to white
                RenderSettings.fogDensity = 0.03f;
                RenderSettings.fogColor = Color.white;
                break;
        }
    }

    // Checks if any of the animation flags are set, if so then perform the current frame of animation
    private void PerformIntroAnimationIfSet()
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
        ShowChatbotSpeechBubbleAndPerformAnimation(message);
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
}