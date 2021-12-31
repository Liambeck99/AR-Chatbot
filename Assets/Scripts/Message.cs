using System;

// Class that acts as a data structure for storing information about an individual message in a conversation
[Serializable]
public class Message
{
    public string text;
    public bool userWasSpeaker;
    public string timeProcessed;
}
