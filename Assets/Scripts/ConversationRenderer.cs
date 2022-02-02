﻿// Class that handles how conversation messages are rendered on the screen (as speech bubbles)

// Note: this class needs improvement as the speech bubbles are not properly rendered yet 

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationRenderer : MonoBehaviour
{
    // Stores a conversation of messages that has been updated by the SetConversation method
    private ConversationHandler currentConversation;

    // Stores the current position of the conversation in the relative Conversation Container object.
    // This has the function of setting the position of a new speech bubble so that all messages
    // render linearly and with correct distance from one another
    private float currentConversationHeadPosition;

    // Stores a reference to the template user and chatbot speech bubble
    private GameObject templateUserSpeechBubble;
    private GameObject templateChatbotSpeechBubble;

    // Container that holds all speechbubbles
    private RectTransform canvasRectTransform;

    // Stores the distance between the speechbubbles that should be used when updating the 
    // conversation head position
    private float distanceBetweenSpeechBubbles;

    // Number of messages that have currently been rendered on the screen
    private int currentRenderedMessages;

    // Holds the position of the top of the scroll container
    private float defaultHeight;

    // Sets whether or not to use timestamps to seperate the conversation
    private bool useTimeStamps;

    // The number of minutes before a new timestamp should be used
    private int minutesForNewTimeStamp;

    // Stores reference to the timestamp object (if enabled)
    private GameObject timeStamp;

    // Configures conversation data before rendering
    public void ConfigureConversation(bool enableTimeStamps)
    {
        // Template user and chatbot speech bubbles are found
        templateUserSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleUser")[0];
        templateChatbotSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleChatbot")[0];

        // These are deactivated as thse should not be shown to the user
        templateUserSpeechBubble.SetActive(false);
        templateChatbotSpeechBubble.SetActive(false);

        // Default distance between speech bubbles is set to 100
        distanceBetweenSpeechBubbles = 100;

        // Sets the current head position at the bottom of the conversation container,
        currentConversationHeadPosition = templateUserSpeechBubble.transform.position.y;

        // Default top of the container is set to the first speech bubble position
        defaultHeight = currentConversationHeadPosition;

        canvasRectTransform = GameObject.Find("ConversationContainer").GetComponent<RectTransform>();

        currentRenderedMessages = 0;

        if (enableTimeStamps)
        {
            timeStamp = GameObject.Find("TimeStampTemplate");
            timeStamp.SetActive(false);
        }

        useTimeStamps = enableTimeStamps;

        minutesForNewTimeStamp = 10;
    }

    // Updates the conversation head position based on the previous speechbubble height, as 
    // well as the distance that should occur between two speech bubbles
    private void UpdateConversationHeadPosition(float speechBubbleHeight)
    {
        currentConversationHeadPosition -= speechBubbleHeight / 2 + distanceBetweenSpeechBubbles;
    }

    // Updates the stored conversation
    public void SetConversation(ConversationHandler newConversation)
    {
        currentConversation = newConversation;
    }

    // Renders a new speech bubble on the screen with position and text
    private void AddNewSpeechBubble(Vector3 position, bool isUserSpeaker, string message)
    {

        // Sets the object to clone as the user speech bubble if the message was written by the user, or the chatbot
        // speech bubble if the message was generated by the chatbot
        GameObject speechBubbleToClone;
        if (isUserSpeaker)
            speechBubbleToClone = templateUserSpeechBubble;
        else
            speechBubbleToClone = templateChatbotSpeechBubble;

        // Clones a new speech bubble object
        GameObject newSpeechBubble = Instantiate(speechBubbleToClone, position, templateUserSpeechBubble.transform.rotation);

        // Speech bubble is made visible
        newSpeechBubble.SetActive(true);

        // Speechbubble is set inside of the container
        newSpeechBubble.transform.SetParent(GameObject.Find("ConversationContainer").transform);

        // Text is updated to the current message
        newSpeechBubble.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = message;

        // Conversation head position is updated so that the next speech bubble is correctly rendered 
        // underneath the new speech bubble
        UpdateConversationHeadPosition(200.0f);
    }

    // Renders a new timestamp on the screen with position and time
    private void AddNewTimeStamp(Vector3 position, string time)
    {
        // Clones a new timestamp object
        GameObject newTimeStamp = Instantiate(timeStamp, position, timeStamp.transform.rotation);

        // Timestamp is made visible
        newTimeStamp.SetActive(true);

        // Timestamp is set inside of the container
        newTimeStamp.transform.SetParent(GameObject.Find("ConversationContainer").transform);

        // Timestamp text is updated to the date time
        newTimeStamp.GetComponent<UnityEngine.UI.Text>().text = time;

        // Conversation head position is updated so that the next speech bubble is correctly rendered 
        // underneath the timestamp
        UpdateConversationHeadPosition(100.0f);

    }

    // Renders all messages in the conversation
    public void RenderConversation()
    {
        int numMessages = currentConversation.GetCurrentConversationSize();

        // Gets the first message in the conversation
        Message newMessage;
        Vector3 newSpeechBubblePosition;

        // Saves the previous message that was processed in the loop (used for comparing message times)
        Message prevMessage = null;
        Vector3 newTimeStampPosition;

        // Resets container size and position for correct conversation rendering
        // (all speech bubbles are loaded without bias of the current scroll position)
        canvasRectTransform.sizeDelta = new Vector2(0, 0);
        canvasRectTransform.localPosition = new Vector3(0, 0, 0);

        // Loops through all messages in the current conversation handler object
        for (int i = currentRenderedMessages; i < numMessages; i++)
        {
            // Gets current message
            newMessage = currentConversation.GetNewMessageAtIndex(i);

            // If there exists a previous message and timestamps should be used
            if(prevMessage != null && useTimeStamps)
            {
                // Gets datetime for the current message
                DateTime newMessageTimeSent = DateTime.ParseExact(newMessage.timeProcessed, 
                                                                  ConversationHandler.dateFormatUsed,
                                                                  System.Globalization.CultureInfo.InvariantCulture);

                // Gets datetime for the previous message
                DateTime prevMessageTimeSent = DateTime.ParseExact(prevMessage.timeProcessed,
                                                                  ConversationHandler.dateFormatUsed,
                                                                  System.Globalization.CultureInfo.InvariantCulture);

                // Gets the number of minutes difference between the previous message and the current message
                int minutesDifference = (int)newMessageTimeSent.Subtract(prevMessageTimeSent).TotalMinutes;

                // If the number of minutes difference between the two messages is larger than the 
                // set minutes needed for a new timestamp, then create a new timestamp object
                if (minutesDifference > minutesForNewTimeStamp)
                {
                    // Set new position at the head of the conversation
                    newTimeStampPosition = timeStamp.transform.position;
                    newTimeStampPosition.y = currentConversationHeadPosition;
                    
                    // Create a new timestamp
                    AddNewTimeStamp(newTimeStampPosition, newMessage.timeProcessed);
                }
            }

            // Gets appropriate speech bubble position depending on whether to create a speech bubble for 
            // the user or chatbot
            if (newMessage.userWasSpeaker)
                newSpeechBubblePosition = templateUserSpeechBubble.transform.position;
            else
                newSpeechBubblePosition = templateChatbotSpeechBubble.transform.position;

            // Y value (relative to the conversation container) is set to the current conversation head position
            newSpeechBubblePosition.y = currentConversationHeadPosition;

            // New speech bubble is rendered
            AddNewSpeechBubble(newSpeechBubblePosition, newMessage.userWasSpeaker, newMessage.text);

            prevMessage = newMessage;

            currentRenderedMessages++;
        }

        // Sets the current height of the scroll container as the size of the number of speech
        // bubbles currently rendered
        float currentHeight = defaultHeight - currentConversationHeadPosition;
        canvasRectTransform.sizeDelta = new Vector2(0, currentHeight);

        // Sets scroll to be positioned at the bottom of the screen
        canvasRectTransform.localPosition = new Vector3(0, currentHeight/2, 0);
    }

}
