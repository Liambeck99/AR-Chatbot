using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatbotScene : BaseSessionScene
{
    ConversationRenderer conversationRenderer;

    private void Start()
    {
        //UpdateColoursIfColourBlindMode();

        ConfigureInputs();
        ConfigureConversation();
        ConfigureConversationRenderer();
    }

    private void ConfigureConversationRenderer()
    {
        // Retrieves the conversation renderer script
        GameObject conversationObject = GameObject.Find("ConversationRenderer");
        conversationRenderer = conversationObject.GetComponent<ConversationRenderer>();

        // Configures and renders all messages in the conversation
        conversationRenderer.ConfigureConversation();
        conversationRenderer.SetConversation(currentConversation);
        conversationRenderer.RenderConversation();
    }

    private void Update()
    {
        
    }

    public override void OnKeyboardSubmit(string message)
    {
        // Add implementation
        if (!CheckMessageIsValid(message))
            return;

        KeyboardInputField.SetActive(false);

        currentConversation.AddNewMessage(message, true);

        conversationRenderer.RenderConversation();

        message = SimplifyMessageString(message);

        string watsonResponseMessage = GetWatsonResponse(message);

        currentConversation.AddNewMessage(watsonResponseMessage, false);

        conversationRenderer.RenderConversation();
    }
}
