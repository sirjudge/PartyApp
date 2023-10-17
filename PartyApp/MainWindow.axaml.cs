using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private List<string> _chatMessages = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SaveMessage(Message message)
    {
        
    }

    private bool ValidateMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
            return false;
        if (string.IsNullOrWhiteSpace(message.Author))
            return false;
        return true;
    }
    
    public void SendMessageToChat(object sender, RoutedEventArgs e)
    {
        var messageText = MessageBox.Text;
        var name = MessageName.Text;
        var message = new Message(messageText, name, DateTime.Now);

        if (!ValidateMessage(message))
        {
            Console.WriteLine("Invalid Message");
            return;
        } 
        _chatMessages.Add(message.ToString());
        UpdateChatWindow();
    }

    public void UpdateChatWindow()
    {
        MessageBox.Text = string.Empty;
        MessageName.Text = string.Empty;
        
        var stringBuilder = new StringBuilder();
        foreach (var message in _chatMessages)
            stringBuilder.AppendLine(message);

        ChatBox.Text = stringBuilder.ToString();
    }
}