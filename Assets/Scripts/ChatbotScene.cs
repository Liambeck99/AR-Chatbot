using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatbotScene : BaseUIScene
{
    private ConversationHandler currentConversation;

    private void Start()
    {
        //UpdateColoursIfColourBlindMode();

        // Creates a new conversation handler object and sets whether to save previous messages or not
        // depending on the user's current settings 
        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        // Retrieves the conversation renderer script
        GameObject conversationObject = GameObject.Find("ConversationRenderer");
        ConversationRenderer conversationRenderer = conversationObject.GetComponent<ConversationRenderer>();

        // Adds the default greetings message
        currentConversation.AddNewMessage("Hello there, how can I help you today?", false);

        // Configures and renders all messages in the conversation
        conversationRenderer.ConfigureConversation();
        conversationRenderer.SetConversation(currentConversation);
        conversationRenderer.RenderConversation();
    }

    private void Update()
    {

    }
}
