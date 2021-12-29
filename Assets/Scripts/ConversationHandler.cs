using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class ConversationHandler {

    public class AllConversations
    {
        public List<Conversation> conversationList;
    }

    private AllConversations currentConversation;

    private AllConversations prevConversations;

    private string fileName = "Assets/Data/PreviousConversations.json";

    private bool hasLoadedPrevConversations;

    public ConversationHandler()
    {
        currentConversation = new AllConversations();
        hasLoadedPrevConversations = false;
    }

    // Writes the current JSON object into the specified file at file location
    public void WriteJson()
    {
        string jsonString;

        if (hasLoadedPrevConversations)
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
            File.WriteAllText(fileName, jsonString);
        }
    }

    // Reads JSON from file location into current JSON object
    public void ReadJson()
    {
        string jsonString = File.ReadAllText(fileName);
        currentConversation = JsonUtility.FromJson<AllConversations>(jsonString);
        hasLoadedPrevConversations = true;
    }

    public void AddNewConversation(string newText, bool wasUserSpeaker)
    {
        Conversation newConversation = new Conversation();
        newConversation.text = newText;
        newConversation.userWasSpeaker = wasUserSpeaker;
        newConversation.timeProcessed = DateTime.Now;

        currentConversation.conversationList.Add(newConversation);
    }

    public void GetConversationAtPosition(int i)
    {


    }

}