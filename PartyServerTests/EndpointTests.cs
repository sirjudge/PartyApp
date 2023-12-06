using System.Text;
using System.Text.Json;
using Message = PartyModels.Message;

namespace PartyServerApp;

public class EndpointTests
{
    private enum EndpointType
    {
        GetMessage,
        InsertMessage,
        Upvote,
        Downvote,
        HealthCheck
    }

    private string GetEndpointUrl(EndpointType type)
    {
        const int port = 5046;
        const string baseUrl = "http://localhost:";
        var endpointUrl = baseUrl + port; 
        return type switch
        {
            EndpointType.GetMessage => endpointUrl + "/GetMessages",
            EndpointType.InsertMessage => endpointUrl + "/InsertMessage",
            EndpointType.Downvote => endpointUrl + "/Downvote",
            EndpointType.HealthCheck => endpointUrl + "/HealthCheck",
            EndpointType.Upvote => endpointUrl + "/Upvote",
            _ => throw new NotSupportedException("Endpoint type not currently supported")
        };
    }
    
    [Test]
    public void GetMessages()
    {
        var endpoint = GetEndpointUrl(EndpointType.GetMessage);
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync(endpoint).Result;
        Assert.That(response.IsSuccessStatusCode);
    }
    
    [Test]
    public void InsertMessage()
    {
        var endpoint = GetEndpointUrl(EndpointType.InsertMessage);
        var message = new Message("text", "author", DateTime.Now);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post,endpoint);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
        var httpClient = new HttpClient();
        var response = httpClient.SendAsync(requestMessage).Result;
        if (!response.IsSuccessStatusCode)
           Assert.Fail($"response was not ok status code. Expected 200ok but got {response.StatusCode} with reason:{response.ReasonPhrase}"); 
    }
    
    [Test]
    public void Upvote()
    {
        var endpoint = GetEndpointUrl(EndpointType.Upvote);
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync(endpoint).Result;
        Assert.That(response.IsSuccessStatusCode);
    }
    
    [Test]
    public void Downvote()
    {
        var endpoint = GetEndpointUrl(EndpointType.Downvote);
        // var httpClient = new HttpClient();
        // var response = httpClient.GetAsync(endpoint).Result;
        // Assert.That(response.IsSuccessStatusCode);
    }
    
    [Test]
    public void HealthCheck()
    {
        var endpoint = GetEndpointUrl(EndpointType.HealthCheck);
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync(endpoint).Result;
        Assert.That(response.IsSuccessStatusCode);
    }
    
}