// Script that is attached to the AR scene
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ARScene : BaseAvatarScene
{
    // Stores the current phase of the tutorial the user is at
    private int tutorialPhase;
    // Stores the number of steps in the tutorial
    private int maxTutorialPhases;

    // Holds references to all GameObjects used in the tutorial
    private GameObject[] tutorialElements;

    private AvatarMeshHandler animationController = new AvatarMeshHandler();

    public Sprite blackSwitchToAvatarSprite;
    public Sprite blackSwitchToChatbotSprite;


    /*// Refences the avatar that performs these animations 
    private GameObject avatarModel;
   

    protected AvatarMeshHandler meshHandler;

    // Stores the time that a last 'random' animation occured, used for 
    // ensuring that random animations happen sparingly
    protected DateTime timeOfLastRandomAnimation;

    // Determines the least amount of seconds to wait between every random animation
    protected int secondsToWaitBetweenRandomAnimations;*/

    private void Start()
    {
        ConfigureScene();

        ConfigureARScene(0);

        //ConfigureAvatars(0);
        ToggleTutorial(false);
    }

    // Handle models select from dropdown
    public void HandleDropdownSelection(int value)
    {
        if (value == 0)
        {
            ConfigureARScene(0);
        }
        if (value == 1)
        {
            ConfigureARScene(1);
        }
        if (value == 2)
        {
            ConfigureARScene(2);
        }
    }


    private void ConfigureAvatars(int value)
    {
        
        // set displayed model to model 1
        avatarModel = models[value];
        meshHandler = avatarModel.GetComponent<AvatarMeshHandler>();

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

        // If Text-To-Speech is enabled, then this means that audio should be played, so
        // the avatar audio is set to volume = 1, otherwise it is muted (volume = 0)
        if (useTTS)
            meshHandler.UnmuteAudio();
        else
            meshHandler.MuteAudio();

        // Sets the least amount of seconds to wait for when the avatar performs a random animation (if idle)
        secondsToWaitBetweenRandomAnimations = 10;

        // Retrieves speech bubble and text objects
        chatbotSpeechBubble = GameObject.Find("ChatbotSpeechBubble");
        //responseText = GameObject.Find("ResponseText").GetComponent<TextMeshPro>();

        // Parent object of the speechbubble and text, this is used for handling whether all of
        // the objects should be active or not
        responseContainer = GameObject.Find("ResponseContainer");

        // Parent object of the speech bubble, used for correct Y scaling when changing the size of
        // the speech bubble. This must be here if the speechbubble is to correctly scale in only downwards
        // in the y direction
        speechBubbleScaler = GameObject.Find("SpeechBubbleScaler");

        // Speech bubble is automatically deactivated
        //responseContainer.SetActive(false);

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
        // Tutorial starts at phase 0
        tutorialPhase = 0;
        // The number of phases is the number of child objects from the 'Tutorial' container, 
        // this is set dynamically so that more tutorial phases can be added without having to
        // change this script
        maxTutorialPhases = GameObject.Find("Tutorial").transform.childCount;

        tutorialElements = new GameObject[maxTutorialPhases];

        // Adds all children of 'tutorial' to the references array
        for (int i = 0; i < maxTutorialPhases; i++)
            tutorialElements[i] = GameObject.Find("Tutorial").transform.GetChild(i).gameObject;
        
        // If the user has completed the tutorial, then don't activate the tutorial
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            ToggleTutorial(false);
        // Otherwise, enable the tutorial
        else
            ToggleTutorial(true);
    }

    private void Update()
    {
        UpdateScene();

        UpdateAvatar();
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

    // Sets the tutorial to the next phase
    public void UpdateTutorial()
    {
        // If the tutorial is at the last phase, then disable the tutorial and update the settings to not
        // show the tutorial again
        if (tutorialPhase == maxTutorialPhases-1)
        {
            ToggleTutorial(false);
            currentSettings.UpdateField("completeTutorial", true);
            currentSettings.WriteJson();
        }
        // Otherwise, disable the current tutorial phase, increment the counter and activate the next phase of the tutorial
        else if (tutorialPhase < maxTutorialPhases-1)
        {
            tutorialElements[tutorialPhase].SetActive(false);
            tutorialPhase++;
            tutorialElements[tutorialPhase].SetActive(true);
        }
    }

    // Activates/deactivates the tutorial
    private void ToggleTutorial(bool activate)
    {
        // Activates/deactivates the tutorial button, which covers most of the screen that the user clicks on
        // to progress in the tutorial process
        GameObject TutorialButton = GameObject.Find("TutorialButton");
        TutorialButton.SetActive(activate);

        // Activates/deactivates the main tutorial container
        GameObject TutorialContainer = GameObject.Find("Tutorial");
        TutorialContainer.SetActive(activate);

        // Condition for if the tutorial needs to be activated
        if (activate)
        {
            // Activates first phase (as this should be shown first)
            tutorialElements[0].SetActive(true);

            // Deactivates all other phases in the tutorial, as these will be shown incrementally
            for(int i=1; i < maxTutorialPhases; i++)
                tutorialElements[i].SetActive(false);
        }
    }

    // Executes if the user clicks to go to the Avatar scene 
    public void GoToAvatarIfTutorialFinished()
    {
        // User can only switch scenes once the tutorial is complete
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnAvatarClick();
    }

    // Executes if the user clicks to go to the Chatbot scene
    public void GoToChatbotIfTutorialFinished()
    {
        // User can only switch scenes once the tutorial is complete
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnChatbotClick();
    }

    // Executes if the user clicks the keyboard button
    public void GetKeyboardInputIfTutorialFinished()
    {
        // User can only bring up the keyboard input if the tutorial is finished
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnKeyboardClick();
    }

    // Executes if the user clicks the microphone button
    public void GetMicrophoneInputIfTutorialFinished()
    {
        // User can only bring up the microphone input if the tutorial is finished
        if (currentSettings.ReturnFieldValue("completeTutorial"))
            OnMicroPhoneClick();
    }

    protected override void RenderUserMessage(string message)
    {
        meshHandler.ToggleAnimationPhase();
    }

    protected override void RenderChatbotResponseMessage(string message)
    {

        ShowChatbotSpeechBubbleAndPerformAnimation(message);
    }

    protected override void SetColourBlindSprites()
    {
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();
        Image switchAvatar = GameObject.Find("SwitchToAvatar").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToAvatarSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }
}

