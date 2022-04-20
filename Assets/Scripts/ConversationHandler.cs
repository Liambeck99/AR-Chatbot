// Handles a 'conversation' between the user and chatbot. Essentially, this stores a list of 
// Message objects that hold information about what was said, by whom and when. A series
// of methods are included that make it easy to interface with the conversation object

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
    private string prevConversationFilePath;

    // File path to the session conversation file
    private string sessionConversationFilePath;

    // Stores whether or not to write new messages to previous conversation log
    private bool saveNewMessages;

    // The format used for representing the timeprocessed datetime object
    public static string dateFormatUsed = "MM/dd/yyyy HH:mm:ss";

    public ConversationHandler(string newPrevConversationFilePath, string newSessionConversationFilePath)
    {
        prevConversationFilePath = newPrevConversationFilePath;
        sessionConversationFilePath = newSessionConversationFilePath;

        Debug.Log(prevConversationFilePath);

        ResetCurrentConversation();
        CheckConversationFilesExist();
    }

    public void CheckConversationFilesExist()
    {
        if (!File.Exists(prevConversationFilePath))
            ResetPrevConversations();

        if (!File.Exists(sessionConversationFilePath))
            ResetSessionConversation();
    }

    // Creates a new conversation object
    public void ResetCurrentConversation()
    {
        currentConversation = new Conversation();
        currentConversation.messageList = new List<Message>();
    }

    // Sets whether or not to save messages to the previous message file
    public void setSaveMessages(bool saveMessages)
    {
        saveNewMessages = saveMessages;
    }

    // Reads previous conversation file 
    public void LoadPrevConversation()
    {
        string jsonString = File.ReadAllText(prevConversationFilePath);
        currentConversation = JsonUtility.FromJson<Conversation>(jsonString);
    }
    
    // Reads current session conversation file
    public void LoadSessionConversation()
    {
        string jsonString = File.ReadAllText(sessionConversationFilePath);
        currentConversation = JsonUtility.FromJson<Conversation>(jsonString);
    }

    public void AddMessageToConversationWithFilePath(string filePath, Message newMessage)
    {
        Conversation savedConversation = JsonUtility.FromJson<Conversation>(File.ReadAllText(filePath));

        if (savedConversation is null)
            savedConversation = new Conversation();

        if (savedConversation.messageList is null)
            savedConversation.messageList = new List<Message>();

        savedConversation.messageList.Add(newMessage);

        File.WriteAllText(filePath, JsonUtility.ToJson(savedConversation));
    }

    // Adds a new message to the conversation
    public void AddNewMessage(string newText, bool wasUserSpeaker)
    {
        // Creates new Message object with properties as the passed arguments
        Message newMessage = new Message();
        newMessage.text = newText;
        newMessage.userWasSpeaker = wasUserSpeaker;
        newMessage.timeProcessed = DateTime.Now.ToString(dateFormatUsed);

        currentConversation.messageList.Add(newMessage);

        // Writes to previous conversations by loading all previous messages, adding the new message to the list
        // and then writing the conversation to the file
        if (saveNewMessages)
            AddMessageToConversationWithFilePath(prevConversationFilePath, newMessage);
        
        AddMessageToConversationWithFilePath(sessionConversationFilePath, newMessage);
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
    public void ResetPrevConversations()
    {
        Conversation resetConversation = new Conversation();
        File.WriteAllText(prevConversationFilePath, JsonUtility.ToJson(resetConversation));
    }

    public void ResetSessionConversation()
    {
        Conversation resetConversation = new Conversation();
        File.WriteAllText(sessionConversationFilePath, JsonUtility.ToJson(resetConversation));
    }
}
