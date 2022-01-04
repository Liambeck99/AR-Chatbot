using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// The main parent class used for all scenes that interact with the AI in the project
// Set to abstract as this class should not be used individually
public abstract class BaseSessionScene : BaseUIScene
{
    // Stores the current conversation for the session
    protected ConversationHandler currentConversation;

    // The input field for allowing the user to enter in text to the chatbot
    protected GameObject KeyboardInputField;

    protected AudioSource microphoneInputClip;

    protected bool currentlyRecording;

    // Configures the current conversation so that all messages in the current session are loaded
    protected void ConfigureConversation()
    {
        // Creates a new Conversation Handler and sets whether to save messages
        // based on the 'save conversation' setting
        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        // Loads in session messages
        currentConversation.LoadSessionConversation();

        // The number of minutes that must have passed since the last message was said for the session
        // to expire and be reset
        float timeBeforeRestartSession = 3.0f;

        // Checks if the number of current messages in the session is more than 0 (indicates there is a session currently taking place)
        if (currentConversation.GetCurrentConversationSize() > 0)
        {
            // Gets the last saved message in the session
            Message lastMessageSaved = currentConversation.GetNewMessageAtIndex(currentConversation.GetCurrentConversationSize() - 1);

            // Gets the datetime for when that message was sent
            DateTime lastTimeSent = DateTime.ParseExact(lastMessageSaved.timeProcessed, currentConversation.dateFormatUsed,
                                                        System.Globalization.CultureInfo.InvariantCulture);

            // The number of minutes difference between the current time and the last message that was sent
            float minutesDifference = (float)DateTime.Now.Subtract(lastTimeSent).TotalMinutes;

            // If more time has passed since the last message was sent than the ttl, then the session is restarted
            if (minutesDifference > timeBeforeRestartSession)
            {
                currentConversation.ResetSessionConversation();
                currentConversation.ResetCurrentConversation();

                currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
            }
        }

        // Adds the default greetings message if no messages are currently stored in the session
        else
            currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
    }

    // Sets all input fields to default values when the scene is loaded
    protected void ConfigureInputs()
    {
        // Sets the keyboard input has inactive, meaning the user cannot view it until they click
        // the keyboard button
        KeyboardInputField = GameObject.Find("KeyboardInputField");
        KeyboardInputField.SetActive(false);
    }

    // Executes if the keyboard button is clicked
    protected void OnKeyboardClick()
    {
        // If the keyboard is active, then deactive the input
        if (KeyboardInputField.activeInHierarchy)
            KeyboardInputField.SetActive(false);
        else
        {
            // Input is set to active
            KeyboardInputField.SetActive(true);

            // Broken
            Text currentKeyboardInputText = GameObject.Find("KeyboardInputText").GetComponent<Text>();

            currentKeyboardInputText.text = "";

            Debug.Log(currentKeyboardInputText.text);
        }
    }

    public void ConfigureMicrophone()
    {
        currentlyRecording = false;
        microphoneInputClip = GetComponent<AudioSource>();
    }

    // Abstract class for when the user submits a keyboard message, each subclass
    // should handle this individually
    public abstract void OnKeyboardSubmit(string message);

    public void OnMicroPhoneClick()
    {
        if (!currentlyRecording)
        {
            currentlyRecording = true;
            microphoneInputClip.clip = Microphone.Start(null, true, 10, 48000);
        }
        else
        {
            currentlyRecording = false;
            Microphone.End(null);
            //microphoneInputClip.Play();
        }
    }

    // Gets a response from Watsom based on the message argument
    protected string GetWatsonResponse(string message)
    {
        // Watson exchange goes here

        return "This is an example of what a response would look like...";
    }

    // Checks the format and contents of the message to ensure it is valid for parsing to Watson
    protected bool CheckMessageIsValid(string message)
    {
        // Add message checking

        if (message.Length < 10)
            return false;
        else if (message.Length > 200)
            return false;
        
        return true;
    }

    // Simplifies the message so that it is more clear for when Watson parses it
    protected string SimplifyMessageString(string message)
    {
        // Add message simplification

        return message;
    }
}
