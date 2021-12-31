using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatbotScene : BaseUIScene
{
    private ConversationHandler currentConversation;

    private float currentConversationHeadPosition;

    private GameObject templateUserSpeechBubble;

    private GameObject templateChatbotSpeechBubble;

    private float distanceBetweenSpeechBubbles;

    private void Start()
    {
        //UpdateColoursIfColourBlindMode();

        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));

        templateUserSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleUser")[0];
        templateChatbotSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubbleChatbot")[0];

        templateUserSpeechBubble.SetActive(false);
        templateChatbotSpeechBubble.SetActive(false);

        distanceBetweenSpeechBubbles = 100;
        float defaultSpeechBubbleHeight = 200;

        currentConversationHeadPosition = templateChatbotSpeechBubble.transform.position.y;

        Vector3 newSpeechBubblePosition = templateChatbotSpeechBubble.transform.position;
        newSpeechBubblePosition.y = currentConversationHeadPosition;
        AddNewSpeechBubble(newSpeechBubblePosition, false, "Hello there, how can I help you today?");

        UpdateConversationHeadPosition(defaultSpeechBubbleHeight);
    }

    private void Update()
    {

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
