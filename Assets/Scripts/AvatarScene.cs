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

    private void Start()
    {
        ConfigureScene();
    }

    private void Update()
    {
        UpdateScene();
    }

    protected override void RenderUserMessage(string message)
    {

    }

    protected override void RenderChatbotResponseMessage(string message)
    {

    }

    protected override void SetColourBlindSprites()
    {
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();
        Image switchChatbot = GameObject.Find("SwitchChatbot").GetComponent<Image>();

        switchAvatar.sprite = blackSwitchToARSprite;
        switchChatbot.sprite = blackSwitchToChatbotSprite;
    }
}