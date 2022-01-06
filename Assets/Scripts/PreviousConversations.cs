// Script that is attached to the previous conversations scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreviousConversations: BaseUIScene
{
    private ConversationHandler prevConversation;

    private void Start()
    {
        LoadSettings();

        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();

        // Creates a new conversation handler object and loads all previous saved messages
        prevConversation = new ConversationHandler(CreateRelativeFilePath("PreviousConversations"), 
                                                   CreateRelativeFilePath("CurrentSession"));
        prevConversation.LoadPrevConversation();

        // Retrieves the conversation renderer script
        GameObject conversationObject = GameObject.Find("ConversationRenderer");
        ConversationRenderer conversationRenderer = conversationObject.GetComponent<ConversationRenderer>();

        // Configures and renders all messages in the previous conversation
        conversationRenderer.ConfigureConversation();
        conversationRenderer.SetConversation(prevConversation);
        conversationRenderer.RenderConversation();
    }

    private void Update()
    {
        UpdateFade();
    }
}