using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class BaseSessionScene : BaseUIScene
{
    protected ConversationHandler currentConversation;

    protected GameObject KeyboardInputField;

    protected void ConfigureConversation()
    {
        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        currentConversation.LoadSessionConversation();

        float timeBeforeRestartSession = 3.0f;

        if (currentConversation.GetCurrentConversationSize() > 0)
        {
            Message lastMessageSaved = currentConversation.GetNewMessageAtIndex(currentConversation.GetCurrentConversationSize() - 1);

            DateTime lastTimeSent = DateTime.ParseExact(lastMessageSaved.timeProcessed, currentConversation.dateFormatUsed,
                                                        System.Globalization.CultureInfo.InvariantCulture);

            float minutesDifference = (float)DateTime.Now.Subtract(lastTimeSent).TotalMinutes;

            if (minutesDifference > timeBeforeRestartSession)
            {
                currentConversation.ResetSessionConversation();
                currentConversation.ResetCurrentConversation();

                currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
            }
        }

        // Adds the default greetings message
        else
            currentConversation.AddNewMessage("Hello there, how can I help you today?", false);
    }

    protected void ConfigureInputs()
    {
        KeyboardInputField = GameObject.Find("KeyboardInputField");
        KeyboardInputField.SetActive(false);
    }

    protected void OnKeyboardClick()
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

    public abstract void OnKeyboardSubmit(string message);

    protected string GetWatsonResponse(string message)
    {
        // Watson exchange goes here

        return "This is an example of what a response would look like...";
    }

    protected bool CheckMessageIsValid(string message)
    {
        // Add message checking

        return true;
    }

    protected string SimplifyMessageString(string message)
    {
        // Add message simplification

        return message;
    }
}
