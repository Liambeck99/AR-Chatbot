using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreviousConversations: BaseUIScene
{
    private ConversationHandler prevConversation;

    private float currentConversationHeadPosition;

    private GameObject templateUserSpeechBubble;

    private GameObject templateChatbotSpeechBubble;

    private float distanceBetweenSpeechBubbles;

    private void Start()
    {
        SetFade();
        SetFadeInSpeed(0.66f);
        UpdateColoursIfColourBlindMode();

        ConfigureConversation();
        RenderConversation();
    }

    private void Update()
    {
        UpdateFade();
    }

    private void ConfigureConversation()
    {
        prevConversation = new ConversationHandler();
        prevConversation.LoadPrevConversation();



        templateUserSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleUser")[0];
        templateChatbotSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleChatbot")[0];

        templateUserSpeechBubble.SetActive(false);
        templateChatbotSpeechBubble.SetActive(false);

        distanceBetweenSpeechBubbles = 100;

    }


    private void RenderConversation()
    {
        int numMessages = prevConversation.GetCurrentConversationSize();

        if (numMessages < 1)
            return;

        float defaultSpeechBubbleHeight = 200.0f; 

        Message newMessage = prevConversation.GetNewMessageAtIndex(0);
        Vector3 newSpeechBubblePosition;

        if (newMessage.userWasSpeaker)
        {
            currentConversationHeadPosition = templateUserSpeechBubble.transform.position.y;
            newSpeechBubblePosition = templateUserSpeechBubble.transform.position;
        }
        else
        {
            currentConversationHeadPosition = templateChatbotSpeechBubble.transform.position.y;
            newSpeechBubblePosition = templateChatbotSpeechBubble.transform.position;
        }

        newSpeechBubblePosition.y = currentConversationHeadPosition;
        AddNewSpeechBubble(newSpeechBubblePosition, newMessage.userWasSpeaker, newMessage.text);

        UpdateConversationHeadPosition(defaultSpeechBubbleHeight);

        for (int i = 1; i < numMessages; i++)
            {
                newMessage = prevConversation.GetNewMessageAtIndex(i);

            if (newMessage.userWasSpeaker)
                newSpeechBubblePosition = templateUserSpeechBubble.transform.position;
            else
                newSpeechBubblePosition = templateChatbotSpeechBubble.transform.position;

            newSpeechBubblePosition.y = currentConversationHeadPosition;
            AddNewSpeechBubble(newSpeechBubblePosition, newMessage.userWasSpeaker, newMessage.text);

            UpdateConversationHeadPosition(defaultSpeechBubbleHeight);
        }



    }


    private void AddNewSpeechBubble(Vector3 position, bool isUserSpeaker, string message)
    {
        RectTransform canvasRectTransform = GameObject.Find("ConversationContainer").GetComponent<RectTransform>();

        GameObject speechBubbleToClone;
        if (isUserSpeaker)
            speechBubbleToClone = templateUserSpeechBubble;
        else
            speechBubbleToClone = templateChatbotSpeechBubble;

        GameObject newSpeechBubble = Instantiate(speechBubbleToClone, position, templateUserSpeechBubble.transform.rotation);

        newSpeechBubble.SetActive(true);

        newSpeechBubble.transform.SetParent(GameObject.Find("ConversationContainer").transform);

        newSpeechBubble.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = message;
    }

    private void UpdateConversationHeadPosition(float speechBubbleHeight)
    {
        currentConversationHeadPosition -= speechBubbleHeight / 2 + distanceBetweenSpeechBubbles;
    }
}