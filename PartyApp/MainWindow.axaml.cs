using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PartyModels;

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private List<Message> _chatMessages = new();
    private HttpClient HttpClient;    
    private readonly string _baseUrl = "http://localhost:5046/";
    public MainWindow()
    {
        HttpClient = new HttpClient();
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
        InitializeChatBox();
    }

    private void InitializeChatBox()
    {
        ChatBox.Text = string.Empty;
   
        _chatMessages.Clear();

        HttpClient.GetAsync(_baseUrl);
        
        _chatMessages.AddRange(new List<Message>(){});
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
        var jsonContent = new StringContent(JsonSerializer.Serialize(message));
        HttpClient.PostAsync($"{_baseUrl}/GetMessages", jsonContent);
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