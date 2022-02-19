// Script that is attached to the Avatar scene

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using Random = UnityEngine.Random;

public class AvatarScene : BaseSessionScene
{

    public Sprite blackSwitchToARSprite;
    public Sprite blackSwitchToChatbotSprite;

    private animationStateController animationController;

    private DateTime timeOfLastRandomAnimation;

    private GameObject xBotModel;

    private bool inIntroAnimation;

    private float introAnimationDurationTime;

    private Vector3 FinalPosition;
    private Vector3 BeginningPosition;

    private Vector3 SidePosition;

    GameObject chatbotSpeechBubble;
    GameObject responseContainer;
    GameObject speechBubbleScaler;
    TextMeshPro responseText;

    bool executeSpeechBubbleAnimation;
    bool reverseSpeechBubbleAnimation;
    float speechBubbleAnimationStartTime;
    float speechBubbleAnimationDurationTime;

    int charsPerSecond;

    string currentMessage;

    private void Start()
    {
        ConfigureScene();

        // find models in the scene
        xBotModel = GameObject.Find("xbot");

        animationController = xBotModel.GetComponent<animationStateController>();

        // Sets the time that a random animation occured 4 seconds in the past, this gives
        // a little time before it is possible for a random animation to occur
        timeOfLastRandomAnimation = DateTime.Now.AddSeconds(-4);

        if (useTTS)
            animationController.UnmuteAudio();
        else
            animationController.MuteAudio();

        introAnimationDurationTime = 2.5f;
        FinalPosition = new Vector3(-0.05f, -2.0f, -0.5f);
        BeginningPosition = new Vector3(-0.05f, -0.7f, 20.0f);

        SidePosition = new Vector3(1.0f, -2.0f, -1.0f);

        // There is a session in progress, therefore do not play intro animation
        if (currentSessionHandler.currentConversation.GetCurrentConversationSize() > 1)
        {
            inIntroAnimation = false;
            xBotModel.transform.position = FinalPosition;
            animationController.FinishWalkAnimation(0.01f, false);
        }
        else
            inIntroAnimation = true;

        chatbotSpeechBubble = GameObject.Find("ChatbotSpeechBubble");
        responseText = GameObject.Find("ResponseText").GetComponent<TextMeshPro>();

        responseContainer = GameObject.Find("ResponseContainer");
        speechBubbleScaler = GameObject.Find("SpeechBubbleScaler");

        responseContainer.SetActive(false);

        executeSpeechBubbleAnimation = false;
        reverseSpeechBubbleAnimation = false;
        speechBubbleAnimationDurationTime = 0.5f;
        speechBubbleAnimationStartTime = Time.time;

        charsPerSecond = 6;
    }

    private void Update()
    {
        UpdateScene();
        PerformRandomAnimation();
        RenderSettings.skybox.SetFloat("_Rotation", Time.time);

        if (inIntroAnimation)
        {
            float percentage = (introAnimationDurationTime - Time.time)/introAnimationDurationTime;

            if(percentage > 0)
                xBotModel.transform.position = (FinalPosition * (1 - percentage)) + (BeginningPosition * (percentage));
            
            else
            {
                inIntroAnimation = false;
                animationController.FinishWalkAnimation(0.5f, true);
            }
        }

        if (executeSpeechBubbleAnimation && !reverseSpeechBubbleAnimation)
        {

            float percentage = ((speechBubbleAnimationStartTime + speechBubbleAnimationDurationTime) - Time.time) / speechBubbleAnimationDurationTime;

            int lineCount = responseText.textInfo.lineCount;

            if (percentage > 0)
            {
                speechBubbleScaler.transform.localScale = new Vector3(1.0f * (1 - percentage),
                                                                     ((float)lineCount / 6.5f) * (1 - percentage),
                                                                     1.0f * (1 - percentage));

                xBotModel.transform.position = (SidePosition * (1 - percentage)) + (FinalPosition * (percentage));
            }

            else
            {
                responseText.color = new Color32(255, 255, 255, 255);
                executeSpeechBubbleAnimation = false;
            }
        }

        if (executeSpeechBubbleAnimation && reverseSpeechBubbleAnimation)
        {
            float percentage = ((speechBubbleAnimationStartTime + speechBubbleAnimationDurationTime) - Time.time) / speechBubbleAnimationDurationTime;

            if (percentage > 0)
                xBotModel.transform.position = (SidePosition * percentage) + (FinalPosition * (1 - percentage));
            else
                executeSpeechBubbleAnimation = false;
        }
    }

    protected override void RenderUserMessage(string message)
    {
        animationController.ToggleAnimationPhase();
    }

    protected override void RenderChatbotResponseMessage(string message)
    {
        int timeToWait = message.Length / charsPerSecond;

        responseContainer.SetActive(true);

        responseText.text = message;

        executeSpeechBubbleAnimation = true;
        reverseSpeechBubbleAnimation = false;
        speechBubbleAnimationStartTime = Time.time;

        responseText.color = new Color32(255, 255, 255, 0);

        StartCoroutine(RenderExplinationAnimationWithDelay(timeToWait));
    }

    private IEnumerator RenderExplinationAnimationWithDelay(int timeToWait)
    {
        // Does not allow the user to enter in new messages until the current message has 
        // properly been rendered on the screen
        allowInputs = false;

        animationController.ToggleAnimationPhase();

        yield return new WaitForSeconds(timeToWait);

        animationController.ToggleAnimationPhase();

        allowInputs = true;

        responseText.text = "";

        executeSpeechBubbleAnimation = true;
        reverseSpeechBubbleAnimation = true;
        speechBubbleAnimationStartTime = Time.time;
        responseContainer.SetActive(false);
    }

    protected override void SetColourBlindSprites()
    {
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToARSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }

    private void PerformRandomAnimation()
    {
        int secondsToWait = 10;

        int secondsDifference = (int)DateTime.Now.Subtract(timeOfLastRandomAnimation).TotalSeconds;

        if (secondsDifference < secondsToWait)
            return;

        int chanceOfRandomAnimation = 1;

        if (Random.Range(0, 1000) < chanceOfRandomAnimation)
        {
            animationController.PerformRandomAnimationIfIdle();
            timeOfLastRandomAnimation = DateTime.Now;
        }
    }

    public void OnTTSButtonClickAvatar()
    {
        if (useTTS)
            animationController.MuteAudio();
        else
            animationController.UnmuteAudio();

        OnTTSButtonClick();
    }
     
}