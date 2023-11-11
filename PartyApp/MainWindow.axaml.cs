using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PartyModels;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole;

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private List<Message> _chatMessages = new();
    private HttpClient HttpClient;
    private const string _baseUrl = "http://localhost:5046";
    private Logger logger;
    public MainWindow()
    {
        logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        HttpClient = new HttpClient();
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
        InitializeChatBox();
    }
    
    private void InitializeChatBox()
    {
        logger.Information("InitializeChatBox called");
        ChatBox.Text = string.Empty;
   
        _chatMessages.Clear();

        var response = HttpClient.GetAsync($"{_baseUrl}/GetMessages").Result;
        if (response.StatusCode != HttpStatusCode.OK)
            return;
       
        var responseString = response.Content.ReadAsStringAsync().Result;
        logger.Information($"response was ok. ResponseString:{responseString}");
        var messages = JsonSerializer.Deserialize<List<Message>>(responseString); 
        _chatMessages.AddRange(messages); 
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
        var response = HttpClient.PostAsync($"{_baseUrl}/InsertMessage", jsonContent).Result;
        if (!response.IsSuccessStatusCode)
        {
            logger.Error($"response code from InsertMessage not ok:{response.StatusCode}, response:{response.ReasonPhrase}");
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