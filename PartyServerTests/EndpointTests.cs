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
        const string baseUrl = "http://localhost:";
        const int port = 5000;
        var returnUrl = baseUrl + port;
        
        return type switch
        {
            EndpointType.GetMessage => baseUrl + "/GetMessages",
            EndpointType.InsertMessage => baseUrl + "/InsertMessage",
            EndpointType.Downvote => baseUrl + "/Downvote",
            EndpointType.HealthCheck => baseUrl + "/HealthCheck",
            EndpointType.Upvote => baseUrl + "/Upvote",
            _ => throw new NotSupportedException("Endpoint type not currently supported")
        };
    }
    
    [Test]
    public void GetMessages()
    {
        var endpoint = GetEndpointUrl(EndpointType.GetMessage);
    }
    
    [Test]
    public void InsertMessage()
    {
        var endpoint = GetEndpointUrl(EndpointType.InsertMessage);
    }
    
    [Test]
    public void Upvote()
    {
        var endpoint = GetEndpointUrl(EndpointType.Upvote);
    }
    
    [Test]
    public void Downvote()
    {
        var endpoint = GetEndpointUrl(EndpointType.Downvote);
    }
    
    [Test]
    public void HealthCheck()
    {
        var endpoint = GetEndpointUrl(EndpointType.HealthCheck);
    }
    
}