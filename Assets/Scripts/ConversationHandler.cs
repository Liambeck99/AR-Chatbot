using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// Handles a conversation that is/has occured between the user and chatbot
public class ConversationHandler {

    // Holds a list of all messages that have occured in the conversation
    // This is stored as a class as it can then be easily converted to JSON
    public class Conversation
    {
        public List<Message> messageList;
    }

    // Holds the messages for the current conversation 
    private Conversation currentConversation;

    // Holds the messages for all the previous messages that are currently saved 
    private Conversation prevConversations;

    // File path to the previous conversations file
    private string fileName = "Assets/Data/PreviousConversations.json";

    // Stores whether or not the previous conversation has been loaded
    private bool hasLoadedPrevConversations;

    public ConversationHandler()
    {
        currentConversation = new Conversation();
        hasLoadedPrevConversations = false;
    }

    // TODO: FIX METHOD
    public void WriteJson()
    {
        //string jsonString;

        //
        /*if (hasLoadedPrevConversations)
        {
            jsonString = JsonUtility.ToJson(currentConversation);
            File.WriteAllText(fileName, jsonString);
        }
        else
        {
            if (prevConversations.conversationList.Count == 0) {
                jsonString = File.ReadAllText(fileName);
                prevConversations = JsonUtility.FromJson<AllConversations>(jsonString);
            }

            AllConversations allConversations = prevConversations;

            for (int i = 0; i < currentConversation.conversationList.Count; i++)
                allConversations.conversationList.Add(currentConversation.conversationList[i]);

            jsonString = JsonUtility.ToJson(allConversations);
            File.WriteAllText(fileName, jsonString);*/
        //}
    }

    // Reads previous conversation file 
    public void ReadJson()
    {
        string jsonString = File.ReadAllText(fileName);
        prevConversations = JsonUtility.FromJson<Conversation>(jsonString);
        hasLoadedPrevConversations = true;
    }
    
    // Adds a new message to the conversation
    public void AddNewMessage(string newText, bool wasUserSpeaker)
    {
        Message newConversation = new Message();
        newConversation.text = newText;
        newConversation.userWasSpeaker = wasUserSpeaker;
        newConversation.timeProcessed = DateTime.Now;

        currentConversation.messageList.Add(newConversation);
    }

    public void GetConversationAtPosition(int i)
    {


    }

    // Sets the previous conversation file to an empty list
    public void resetPrevConversations()
    {
        Conversation resetConversation = new Conversation();
        File.WriteAllText(fileName, JsonUtility.ToJson(resetConversation));
    }

}
