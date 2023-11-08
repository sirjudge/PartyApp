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
    private readonly PartyAppRepository _partyAppRepository = new(false);
    public MainWindow()
    {
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
        InitializeChatBox();
    }

    private void InitializeChatBox()
    {
        ChatBox.Text = string.Empty;
        var messages = _partyAppRepository.GetMessages();
        _chatMessages.Clear();
        _chatMessages.AddRange(messages);
        ChatBox.Text = MessageListToString(_chatMessages);
    }
    
    private void TakePhoto(object sender, RoutedEventArgs e)
    {
        PhotoTaker.TakePhoto();
    }
    
    public void SendMessageToChat(object sender, RoutedEventArgs e)
    {
        var messageText = MessageBox.Text;
        var name = MessageName.Text;
        if (string.IsNullOrEmpty(messageText) || string.IsNullOrEmpty(name))
        {
            MessageErrorText.IsVisible = true;
            return;
        }
            
        MessageErrorText.IsVisible = false;
        var message = new Message(messageText, name, DateTime.Now);
        var partyAppRepository = new PartyAppRepository();
        partyAppRepository.InsertMessage(message);   
        
        _chatMessages.Add(message);
        MessageBox.Text = string.Empty;
        MessageName.Text = string.Empty;
        ChatBox.Text = MessageListToString(_chatMessages);
    }
    
    private static string MessageListToString(List<Message> messageList)
    {
        var stringBuilder = new StringBuilder();
        foreach (var message in messageList)
            stringBuilder.AppendLine(message.ToString());
    
        return stringBuilder.ToString();
    }
}