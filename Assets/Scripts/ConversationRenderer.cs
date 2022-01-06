﻿// Class that handles how conversation messages are rendered on the screen (as speech bubbles)

// Note: this class needs improvement as the speech bubbles are not properly rendered yet 

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

    // Stores the distance between the speechbubbles that should be used when updating the 
    // conversation head position
    private float distanceBetweenSpeechBubbles;

    private int currentRenderedMessages;

    // Configures conversation data before rendering
    public void ConfigureConversation()
    {
        // Template user and chatbot speech bubbles are found
        templateUserSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleUser")[0];
        templateChatbotSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleChatbot")[0];

        // These are deactivated as thse should not be shown to the user
        templateUserSpeechBubble.SetActive(false);
        templateChatbotSpeechBubble.SetActive(false);

        // Default distance between speech bubbles is set to 100
        distanceBetweenSpeechBubbles = 100;

        currentConversationHeadPosition = templateUserSpeechBubble.transform.position.y;

        currentRenderedMessages = 0;
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
        // Retrieves the conversation container object
        RectTransform canvasRectTransform = GameObject.Find("ConversationContainer").GetComponent<RectTransform>();

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

    // Renders all messages in the conversation
    public void RenderConversation()
    {
        int numMessages = currentConversation.GetCurrentConversationSize();

        // Gets the first message in the conversation
        Message newMessage;
        Vector3 newSpeechBubblePosition;

        // Loops through all messages in the current conversation handler object
        for (int i = currentRenderedMessages; i < numMessages; i++)
        {
            // Gets current message
            newMessage = currentConversation.GetNewMessageAtIndex(i);

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

            currentRenderedMessages++;
        }
    }

}
