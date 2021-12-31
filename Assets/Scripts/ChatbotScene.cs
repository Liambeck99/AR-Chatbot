using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatbotScene : BaseUIScene
{
    private ConversationHandler currentConversation;

    private float currentConversationHeadPosition;

    private GameObject defaultSpeechBubble;

    private float distanceBetweenSpeechBubbles;

    private void Start()
    {
        //UpdateColoursIfColourBlindMode();

        currentConversation = new ConversationHandler();
        currentConversation.setSaveMessages(currentSettings.ReturnFieldValue("saveConversations"));
        currentConversation.AddNewMessage("Hello there, how can I help you today?", false);

        defaultSpeechBubble = GameObject.FindGameObjectsWithTag("SpeechBubble")[0];

        distanceBetweenSpeechBubbles = 75;

        float defaultSpeechBubbleHeight = 200;

        RectTransform canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();

        currentConversationHeadPosition = defaultSpeechBubble.transform.position.y + defaultSpeechBubbleHeight/2 + distanceBetweenSpeechBubbles;

       
        AddNewSpeechBubble(new Vector3(540.0f, 960.0f, 0.0f), true, "this is a test yo");
    }

    private void Update()
    {

    }

    private void AddNewSpeechBubble(Vector3 position, bool isUserSpeaker, string message)
    {
        RectTransform canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();

        position.x += canvasRectTransform.rect.width/2 * canvasRectTransform.localScale.x;
        position.y += canvasRectTransform.rect.height/2 * canvasRectTransform.localScale.y;

        GameObject newSpeechBubble = Instantiate(defaultSpeechBubble, position, defaultSpeechBubble.transform.rotation);

        newSpeechBubble.transform.SetParent(GameObject.Find("Canvas").transform);

        newSpeechBubble.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = message;

        //newSpeechBubble
    }


}
