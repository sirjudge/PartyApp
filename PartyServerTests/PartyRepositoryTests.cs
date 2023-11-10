
using PartyApp;
using PartyModels;

namespace PhotoTakerTest;

public class PartyRepositoryTests
{
    [Test]
    public void InitializePartyRepository()
    {
        var repo = new PartyAppRepository();
        Assert.NotNull(repo);
        Assert.True(File.Exists("party.db"));
    }
   
    [Test]
    public void InsertValidMessage()
    {
        var repo = new PartyAppRepository();
        var message = new Message("messageText", "messageAuthor", DateTime.Now);
        repo.InsertMessage(message);
    }

    [Test]
    public void InsertInvalidMessageFails()
    {
        try
        {
            var message = new Message(null, null, DateTime.Now);
            var repo = new PartyAppRepository();
            repo.InsertMessage(message);
            Assert.Fail("Exception should have been thrown during validation");
        }
        catch (Exception e)
        {
            //ignore because it's as expected
        }
    }

    [Test]
    public void GetMessage()
    {
        var repo = new PartyAppRepository();
        var messages = repo.GetMessages();
        Assert.That(messages,Is.Not.Empty);
        foreach (var message in messages)
        {
           Assert.Multiple(() =>
           {
               Assert.That(!string.IsNullOrWhiteSpace(message.Text), Is.True);
               Assert.That(!string.IsNullOrWhiteSpace(message.Author), Is.True);
               Assert.That(message.DateSubmitted, Is.Not.Empty);
               Assert.That(message.DateSubmitted, Is.LessThanOrEqualTo(DateTime.Now));
           }); 
        }
    }
}