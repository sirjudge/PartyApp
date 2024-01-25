using PartyModels;
using PartyServer;
using Serilog;
using Serilog.Core;
namespace PartyServerApp;

public class PartyRepositoryTests
{
    private Logger _logger;

    [SetUp]
    public void InitLogger()
    {
        if (_logger is null)
        {
           _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger(); 
        }
    }
    
    
    [Test]
    public void InitializePartyRepository()
    {
        var repo = new PartyAppRepository(_logger);
        Assert.NotNull(repo);
        Assert.True(File.Exists("party.db"));
    }
  

    [Test]
    public void InsertInvalidMessageFails()
    {
        try
        {
            var message = new Message(null, null, DateTime.Now);
            var repo = new PartyAppRepository(_logger);
            repo.InsertMessage(message);
            Assert.Fail("Exception should have been thrown during validation");
        }
        catch (Exception e)
        {
            //ignore because it's as expected
            // if we call assert.pass it'll 
        }
    }
 
    [Test, Order(0)]
    public void InsertValidMessage()
    {
        var repo = new PartyAppRepository(_logger);
        var message = new Message("messageText", "messageAuthor", DateTime.Now);
        repo.InsertMessage(message);
    }
    
    [Test, Order(1)]
    public void GetMessage()
    {
        var repo = new PartyAppRepository(_logger);
        var messages = repo.GetMessages();
        if (messages.Count == 0)
        {
            Assert.Fail("expected > 0 messages to be returned from the database");
            return;
        }
        
        foreach (var message in messages)
        {
            _logger.Information(message.ToString());
           Assert.Multiple(() =>
           {
               Assert.That(!string.IsNullOrWhiteSpace(message.Text), Is.True);
               Assert.That(!string.IsNullOrWhiteSpace(message.Author), Is.True);
               Assert.That(message.Guid, Is.Not.Empty);
               Assert.That(message.DateSubmitted, Is.LessThanOrEqualTo(DateTime.Now));
           }); 
        }
    }
}