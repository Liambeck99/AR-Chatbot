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
        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();

        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

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