// Script that is attached to the Avatar scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AvatarScene : BaseSessionScene
{

    public Sprite blackSwitchToARSprite;
    public Sprite blackSwitchToChatbotSprite;

    private animationStateController animationController;

    private void Start()
    {
        ConfigureScene();
        GameObject animationObject = GameObject.Find("AnimationStateController");
        animationController = animationObject.GetComponent<animationStateController>();
    }

    private void Update()
    {
        UpdateScene();
    }

    protected override void RenderUserMessage(string message)
    {
        animationController.ToggleAnimationPhase();
    }

    protected override void RenderChatbotResponseMessage(string message)
    {
        int charsPerSecond = 10;
        int timeToWait = message.Length / charsPerSecond;
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
    }

    protected override void SetColourBlindSprites()
    {
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToARSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }
}