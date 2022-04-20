using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using Random = UnityEngine.Random;

public abstract class BaseAvatarScene : BaseSessionScene
{
    // Stores the time that a last 'random' animation occured, used for 
    // ensuring that random animations happen sparingly
    protected DateTime timeOfLastRandomAnimation;

    // The main avatar model
    protected GameObject avatarModel;

    // The 3D position that the avatar should end up in after every animation
    protected Vector3 FinalPosition;

    // The 3D position that the avatar should transition to when explaining information
    protected Vector3 SidePosition;

    // The chatbot object that displays the information and extends depending on the message size
    protected GameObject chatbotSpeechBubble;

    // The parent that holds both the speechbubble and text message
    protected GameObject responseContainer;

    // The parent of the 3D speech bubble, this scales the speech bubble in the downwards y direction
    // depending on the size of the message
    protected GameObject speechBubbleScaler;

    // The message that is to be displayed on the avatar speech bubble
    protected TextMeshPro responseText;

    // Flag for if the avatar should perform the animation for showing the speech bubble and 
    // transitoning to the side
    protected bool executeSpeechBubbleAnimation;

    // Determines whether the animation should be reversed or not (if true, then the speech bubble 
    // is removed and the avatar returns to the default position)
    protected bool reverseSpeechBubbleAnimation;

    // Sets the time when the animation was set, this is used for determining what position the avatar 
    // and speech bubble size should be in the current frame
    protected float speechBubbleAnimationStartTime;

    // Determines how long the speech bubble animation should occur for
    protected float speechBubbleAnimationDurationTime;

    // Determines the least amount of seconds to wait between every random animation
    protected int secondsToWaitBetweenRandomAnimations;

    // Determines how many characters should be 'read out' per second. Essentially determining how 
    // long the avatar should explain a message for
    protected int charsPerSecond;

    // Defines the chances that a random animation should occur if the avatar is in the idle state
    protected int chanceOfRandomAnimation;

    protected GameObject[] avatarModels;

    protected int currentAvatarIndex;

    public Sprite greenSwitchModelButtonSprite;
    public Sprite redSwitchModelButtonSprite;

    public Sprite blackActiveSwitchModelButtonSprite;
    public Sprite blackDeactiveSwitchModelButtonSprite;

    protected Sprite switchModelActiveSpriteToUse;
    protected Sprite switchModelDectiveSpriteToUse;

    protected Image switchModelImage;

    protected void ConfigureAvatar()
    {

        switchModelImage = GameObject.Find("SwitchModel").GetComponent<Image>();

        switchModelActiveSpriteToUse = greenSwitchModelButtonSprite;
        switchModelDectiveSpriteToUse = redSwitchModelButtonSprite;

        avatarModels = GameObject.FindGameObjectsWithTag("AvatarModel");

        string currentAvatarName = currentSessionHandler.ReturnStringFieldValue("currentAvatarName");

        currentAvatarIndex = -1;

        for (int i = 0; i < avatarModels.Length; i++) {
            if (avatarModels[i].name != currentAvatarName)
                avatarModels[i].SetActive(false);
            else
                currentAvatarIndex = i;
        }

        if (currentAvatarIndex == -1)
        {
            currentAvatarIndex = 1;
            avatarModels[1].SetActive(true);
        }

        // If Text-To-Speech is enabled, then this means that audio should be played, so
        // the avatar audio is set to volume = 1, otherwise it is muted (volume = 0)
        if (useTTS)
            avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().UnmuteAudio();
        else
            avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().MuteAudio();

        // Defines the final position that the avatar should end up after the intro animation has 
        // completed. In this case, it is in front of the camera
        FinalPosition = new Vector3(-0.05f, -1.94f, -0.5f);

        // The position that the avatar should transition to when explaining a returned message
        SidePosition = new Vector3(1.2f, -2.0f, -1.0f);

        // Sets the least amount of seconds to wait for when the avatar performs a random animation (if idle)
        secondsToWaitBetweenRandomAnimations = 10;

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
    }

    public void OnSwitchModelClick()
    {

        string currentStateName = avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().GetCurrentStateName();

        if (currentStateName != "Breathing Idle")
            return;

        float currentStateDuration = avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().GetCurrentStateDuration();

        avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().SetDefaultValues();

        avatarModels[currentAvatarIndex].SetActive(false);

        currentAvatarIndex = (currentAvatarIndex + 1) % avatarModels.Length;

        avatarModels[currentAvatarIndex].SetActive(true);

        avatarModels[currentAvatarIndex].transform.position = FinalPosition;

        avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().PlayAnimation(currentStateName, currentStateDuration);

        Debug.Log(avatarModels[currentAvatarIndex].name);

        currentSessionHandler.UpdateStringField("currentAvatarName", avatarModels[currentAvatarIndex].name);
    }


    // Displays the response speechbubble and performs the explination animation
    protected void ShowChatbotSpeechBubbleAndPerformAnimation(string message)
    {
        switchModelImage.sprite = switchModelDectiveSpriteToUse;

        // Determines how long the avatar should explain the response message and how thus how long the 
        // speech bubble should appear on the screen
        int timeToWait = message.Length / charsPerSecond;

        timeToWait = Math.Max(timeToWait, 3);

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
    protected IEnumerator RenderExplinationAnimationWithDelay(int timeToWait)
    {
        // Does not allow the user to enter in new messages until the current message has 
        // properly been rendered on the screen and 'explained'
        allowInputs = false;

        // Avatar animation is transitioned from the 'thinking' animation to the 'explination' animation
        avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().ToggleAnimationPhase();

        // Function waits the appropraite amount of seconds so that the user can correctly read the 
        // returned message from Watson
        yield return new WaitForSeconds(timeToWait);

        // Once the appropriate amount of seconds has expired for the speechbubble to be shown,
        // then the avatar should transition back to the default idle state
        avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().ToggleAnimationPhase();

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

        switchModelImage.sprite = switchModelActiveSpriteToUse;
    }

    // Executed after every update frame and determines if the avatar should perform a 'random' animation
    protected void PerformRandomAnimation()
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
            avatarModels[currentAvatarIndex].GetComponent<AvatarMeshHandler>().PerformRandomAnimationIfIdle();

            // Time that the last random animation occured is set to the current time
            timeOfLastRandomAnimation = DateTime.Now;
        }
    }

    protected void PerformResponseAnimationsIfSet()
    {
        // Used in all animations and calculates how far through the animation the current frame is,
        // based on the current time and the start time of the animation. This is then used to 
        // determine the position/sizes that the objects should be based on a start and end position/size
        float percentage;

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

                avatarModels[currentAvatarIndex].transform.position = (SidePosition * (1 - percentage)) + (FinalPosition * (percentage));
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
                avatarModels[currentAvatarIndex].transform.position = (SidePosition * percentage) + (FinalPosition * (1 - percentage));

            // Otherwise the animation has finished, and set the flag to false
            else
                executeSpeechBubbleAnimation = false;
        }
    }

    // Executed if the user clicks the tts button (aka the audio button in the UI)
    public void OnTTSButtonClickAvatar()
    {

        // Loops through all of the avaliable avatar models
        for (int i = 0; i < avatarModels.Length; i++)
        {
            // If audio is currently set to play, then set any audio that is currently playing from the avatar
            // to mute (since the user has clicked to mute any audio)
            if (useTTS)
                avatarModels[i].GetComponent<AvatarMeshHandler>().MuteAudio();

            // Otherwise unmute any audio
            else
                avatarModels[i].GetComponent<AvatarMeshHandler>().UnmuteAudio();
        }

        // Method defined in the parent class that handles TTS audio if the button is clicked
        OnTTSButtonClick();
    }
}
