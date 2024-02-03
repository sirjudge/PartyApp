using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PartyApp;

public partial class MainWindow : Window
{
    // This is gonna get real big, maybe use sqlite or sql docker container instead
    private readonly List<Message> _chatMessages = [];
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;
    private readonly string _baseUrl = Connection.GetServerUrl(true);
    public MainWindow()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        _httpClient = new HttpClient();
        Console.WriteLine("Initialized MainWindow");
        InitializeComponent();
        InitializeChatBox();
    }
    
    private void InitializeChatBox()
    {
        ChatBox.Text = string.Empty;
        var messages = new List<Message>(); 
        _chatMessages.Clear();
        try
        {
            var response = _httpClient.GetAsync($"{_baseUrl}/GetMessages").Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Error($"Non ok status code:{response.StatusCode}, response:{response.ReasonPhrase}");
                return;
            }

            var messageList = response.Content.ReadFromJsonAsync<List<Message>>().Result;
            if (messageList is null)
            {
                _logger.Warning("No messages returned from server");
                return;
            }
            
            _logger.Information("Successfully retrieved {MessageCount} messages from server", messageList.Count);
            
            _chatMessages.AddRange(messageList);
            _logger.Information("response was ok. Added {MessagesLength:} messages to the chat list", messages.Count());
            ChatBox.Text = MessageListToString(_chatMessages.ToList());
        }
        catch (Exception e)
        {
            _logger.Error("Ran into excpetion generating chatbox:" + e.Message + " StackTrace:" + e.StackTrace);
        }
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
        var requestMessage = new HttpRequestMessage(HttpMethod.Post,$"{_baseUrl}/InsertMessage");
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
        var response = _httpClient.SendAsync(requestMessage).Result;
        if (!response.IsSuccessStatusCode)
        {
            _logger.Error($"response code from InsertMessage not ok:{response.StatusCode}, response:{response.ReasonPhrase}");
            return;
        }
        
        _logger.Information($"Successfully inserted message into database:{message.Guid}" );
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

    private void ResetChat(object? sender, RoutedEventArgs e)
    {
        InitializeChatBox();
    }

    private void DeleteAllChat(object? sender, RoutedEventArgs e)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete,$"{_baseUrl}/DeleteAllMessages");
        var response = _httpClient.SendAsync(requestMessage).Result;
        if (response.StatusCode != HttpStatusCode.OK)
            _logger.Error($"response code from DeleteAllMessages not ok:{response.StatusCode}, response:{response.ReasonPhrase}");
        else
            _logger.Information("Successfully deleted all messages from server");

        InitializeChatBox();
    }

    private void OpenPhotoBooth(object? sender, RoutedEventArgs e)
    {
        var dialog = new PhotoBoothWindow();
        dialog.Show();
    }
}