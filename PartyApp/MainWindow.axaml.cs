using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PartyModels;
using Serilog;
using Serilog.Core;

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private List<Message> _chatMessages = new();
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5000";
    private readonly Logger logger;
    
    public MainWindow()
    {
        logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        _httpClient = new HttpClient();
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
        InitializeChatBox();
    }
    
    private void InitializeChatBox()
    {
        logger.Information("InitializeChatBox called");
        ChatBox.Text = string.Empty;
        var messages = new List<Message>();
        _chatMessages.Clear();
        try
        {
            var response = _httpClient.GetAsync($"{BaseUrl}/GetMessages").Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.Error($"Non ok status code:{response.StatusCode}, response:{response.ReasonPhrase}");
                return;
            }

            var responseString = response.Content.ReadAsStringAsync().Result;
            if (responseString is null or "\"[]\"")
            {
                logger.Warning("No messages returned from server");
                return;
            }

            logger.Information($"ResponseString:{responseString.ToString()}");
            messages = JsonSerializer.Deserialize<List<Message>>(responseString);
            _chatMessages.AddRange(messages);
        }
        catch (Exception e)
        {
            logger.Error("Ran into excpetion generating chatbox:" + e.Message + " StackTrace:" + e.StackTrace);
        }
        logger.Information($"response was ok. Added {messages.Count} messages to the chat list");
        ChatBox.Text = MessageListToString(_chatMessages);
    }
    
    private void TakePhoto(object sender, RoutedEventArgs e)
    {
        PhotoTaker.TakePhoto();
    }
    
    public void LogMessageToDb(object sender, RoutedEventArgs e)
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
        var response = _httpClient.PostAsync($"{BaseUrl}/InsertMessage", jsonContent).Result;
        if (!response.IsSuccessStatusCode)
        {
            logger.Error($"response code from InsertMessage not ok:{response.StatusCode}, response:{response.ReasonPhrase}");
            return;
        }
        
        logger.Information($"Successfully inserted message into database:{message.Guid}" );
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