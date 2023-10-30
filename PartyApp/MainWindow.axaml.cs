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
    private readonly PartyAppRepository _partyAppRepository = new();
    public MainWindow()
    {
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
    }

    private void TakePhoto(object sender, RoutedEventArgs e)
    {
        PhotoTaker.TakePhoto();
    }
    
    private void SaveMessage(Message message)
    {
        var partyAppRepository = new PartyAppRepository();
        partyAppRepository.InsertMessage(message);        
    }

    private bool ValidateMessage(Message message)
    {
        return !string.IsNullOrEmpty(message.Text) 
               && !string.IsNullOrEmpty(message.Author);
    }
    
    public void SendMessageToChat(object sender, RoutedEventArgs e)
    {
        var messageText = MessageBox.Text;
        var name = MessageName.Text;
        var message = new Message(messageText, name, DateTime.Now);
        if (!ValidateMessage(message))
        {
            return;
            messageErrorText.IsVisible = true;
        }
        else 
            messageErrorText.IsVisible = false;
        
        SaveMessage(message);
        
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