using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private List<Message> _chatMessages = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TakePhoto(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private void SaveMessage(Message message)
    {
        Messenger.SaveMessage(message);
    }

    
    public void SendMessageToChat(object sender, RoutedEventArgs e)
    {
        var messageText = MessageBox.Text;
        var name = MessageName.Text;
        var message = new Message(messageText, name, DateTime.Now);

        if (!Messenger.ValidateMessage(message))
        {
            Console.WriteLine("Invalid Message");
            return;
        }
        
        _chatMessages.Add(message);
       
        UpdateChatWindow();
    }

    public void UpdateChatWindow()
    {
        MessageBox.Text = string.Empty;
        MessageName.Text = string.Empty;
        ChatBox.Text = Messenger.MessageListToString(_chatMessages);
    }
}