using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AvatarScene : BaseSessionScene
{
    private void Start()
    {
        ConfigureInputs();
        ConfigureConversation();

        currentSettings = new SettingsHandler(CreateRelativeFilePath("ApplicationSettings"));
    }

    private void Update()
    {

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

        message = SimplifyMessageString(message);

        // Gets the Watson response message
        string watsonResponseMessage = GetWatsonResponse(message);

        // Adds new message to conversation and renders it
        currentConversation.AddNewMessage(watsonResponseMessage, false);
    }
}