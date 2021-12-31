using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// Handles a conversation that is/has occured between the user and chatbot
public class ConversationHandler {

    // Holds a list of all messages that have occured in the conversation
    // This is stored as a class as it can then be easily converted to JSON
    [Serializable]
    public class Conversation
    {
        public List<Message> messageList;
    }

    // Holds the messages for the current conversation 
    private Conversation currentConversation;

    // File path to the previous conversations file
    private string fileName = "Assets/Data/PreviousConversations.json";

    // Stores whether or not to write new messages to previous conversation log
    private bool saveNewMessages;

    public ConversationHandler()
    {
        currentConversation = new Conversation();
        currentConversation.messageList = new List<Message>();
    }

    public void setSaveMessages(bool saveMessages)
    {
        saveNewMessages = saveMessages;
    }

    // Reads previous conversation file 
    public void LoadPrevConversation()
    {
        string jsonString = File.ReadAllText(fileName);
        currentConversation = JsonUtility.FromJson<Conversation>(jsonString);
    }
    
    // Adds a new message to the conversation
    public void AddNewMessage(string newText, bool wasUserSpeaker)
    {
        // Creates new Message object with properties as the passed arguments
        Message newConversation = new Message();
        newConversation.text = newText;
        newConversation.userWasSpeaker = wasUserSpeaker;
        newConversation.timeProcessed = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

        currentConversation.messageList.Add(newConversation);

        // Writes to previous conversations by loading all previous messages, adding the new message to the list
        // and then writing the conversation to the file
        if (saveNewMessages)
        {
            Conversation prevConversation = JsonUtility.FromJson<Conversation>(File.ReadAllText(fileName));

            if (prevConversation is null)
                prevConversation = new Conversation();

            if (prevConversation.messageList is null)
                prevConversation.messageList = new List<Message>();

            prevConversation.messageList.Add(newConversation);

            File.WriteAllText(fileName, JsonUtility.ToJson(prevConversation));
        }
    }

    public Message GetNewMessageAtIndex(int i)
    {
        return currentConversation.messageList[i];
    }

    public int GetCurrentConversationSize()
    {
        return currentConversation.messageList.Count;
    }

    // Sets the previous conversation file to an empty list
    public void resetPrevConversations()
    {
        Conversation resetConversation = new Conversation();
        File.WriteAllText(fileName, JsonUtility.ToJson(resetConversation));
    }

}
