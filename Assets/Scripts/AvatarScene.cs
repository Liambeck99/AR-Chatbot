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
        animationController.ToggleAnimationPhase();
        animationController.ToggleAnimationPhase();
    }

    protected override void SetColourBlindSprites()
    {
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToARSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }
}