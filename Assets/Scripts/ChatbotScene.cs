using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatbotScene : BaseUIScene
{
    private ConversationHandler currentConversation;

    private GameObject KeyboardInputField;

    ConversationRenderer conversationRenderer;

    private void Start()
    {
        //UpdateColoursIfColourBlindMode();

        // Creates a new conversation handler object and sets whether to save previous messages or not
        // depending on the user's current settings 
        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        // Retrieves the conversation renderer script
        GameObject conversationObject = GameObject.Find("ConversationRenderer");
        conversationRenderer = conversationObject.GetComponent<ConversationRenderer>();

        // Adds the default greetings message
        currentConversation.AddNewMessage("Hello there, how can I help you today?", false);

        // Configures and renders all messages in the conversation
        conversationRenderer.ConfigureConversation();
        conversationRenderer.SetConversation(currentConversation);
        conversationRenderer.RenderConversation();

        KeyboardInputField = GameObject.Find("KeyboardInputField");
        KeyboardInputField.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void OnKeyboardClick()
    {
        if (KeyboardInputField.activeInHierarchy)
            KeyboardInputField.SetActive(false);
        else
        {
            KeyboardInputField.SetActive(true);

            // Broken
            Text currentKeyboardInputText = GameObject.Find("KeyboardInputText").GetComponent<Text>();

            currentKeyboardInputText.text = "";

            Debug.Log(currentKeyboardInputText.text);


        }
    }

    public void OnKeyboardSubmit(string message)
    {
        // Add message checking here

        KeyboardInputField.SetActive(false);

        currentConversation.AddNewMessage(message, true);

        conversationRenderer.RenderConversation();

        // Add string simplification here

        // Include Watson exchange here

        string exampleResponseMessage = "This is an example of what a response would look like...";

        currentConversation.AddNewMessage(exampleResponseMessage, false);

        conversationRenderer.RenderConversation();
    }
}
