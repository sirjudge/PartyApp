using System.Collections.Generic;
using System.Text;

namespace PartyApp;

public static class Messenger
{
    public static void SaveMessage(Message message)
    {
      
    }

    public static bool ValidateMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
            return false;
        if (string.IsNullOrWhiteSpace(message.Author))
            return false;
        return true;
    }

    public static string MessageListToString(List<Message> messageList)
    {
        var stringBuilder = new StringBuilder();
        foreach (var message in messageList)
            stringBuilder.AppendLine(message.ToString());
        return stringBuilder.ToString();
    }
}