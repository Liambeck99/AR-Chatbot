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

        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));

        CheckPermissions();
        //ConfigureTTSandSTT();
        ConfigureInputs();
        ConfigureConversation();
        ConfigureConversationRenderer();
        ConfigureMicrophone();
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
        UpdateCheckMicrophoneRecording();
    }

    public override void OnKeyboardSubmit(string message)
    {
        // Checks that the message is valid
        if (!CheckMessageIsValid(message))
            return;

        // Keyboard input field is made inactive
        KeyboardInputField.SetActive(false);

        // Adds the new message to the conversation
        currentConversation.AddNewMessage(message, true);

        // Renders new message
        conversationRenderer.RenderConversation();

        message = SimplifyMessageString(message);

        // Gets the Watson response message
        string watsonResponseMessage = GetWatsonResponse(message);

        // Adds new message to conversation and renders it
        currentConversation.AddNewMessage(watsonResponseMessage, false);
        conversationRenderer.RenderConversation();
    }
}
