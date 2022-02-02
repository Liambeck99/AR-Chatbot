// Script that is attached to the Chatbot scene. This includes overriding methods that
// are similar to the parent methods but also include speech bubble rendering after each
// message is created

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TextSpeech;

public class ChatbotScene : BaseSessionScene
{
    ConversationRenderer conversationRenderer;

    public Sprite blackSwitchToARSprite;
    public Sprite blackSwitchToAvatarSprite;

    private void Start()
    {
        ConfigureScene();

        ConfigureConversationRenderer();
    }

    private void ConfigureConversationRenderer()
    {
        // Retrieves the conversation renderer script
        GameObject conversationObject = GameObject.Find("ConversationRenderer");
        conversationRenderer = conversationObject.GetComponent<ConversationRenderer>();

        // Configures and renders all messages in the conversation
        conversationRenderer.ConfigureConversation(false);
        conversationRenderer.SetConversation(currentSessionHandler.currentConversation);

        if (currentSettings.ReturnFieldValue("useColourBlind"))
            conversationRenderer.useColourBlindMode();

        conversationRenderer.RenderConversation();
    }

    private void Update()
    {
        UpdateScene();
    }

    protected override void RenderUserMessage(string message)
    {
        conversationRenderer.RenderConversation();
    }

    protected override void RenderChatbotResponseMessage(string message)
    {
        conversationRenderer.RenderConversation();
    }

    protected override void SetColourBlindSprites()
    {
        Image switchAR = GameObject.Find("SwitchAR").GetComponent<Image>();
        Image switchAvatar = GameObject.Find("SwitchAvatar").GetComponent<Image>();

        switchAR.sprite = blackSwitchToARSprite;
        switchAvatar.sprite = blackSwitchToAvatarSprite;
    }
}
