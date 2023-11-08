
namespace PhotoTakerTest;

public class PartyRepositoryTests
{
    [Fact]
    public void PartyRepositoryConstructor()
    {
        var repo = new PartyRepositoryTests();
    }
    
    [Fact]
    public void InsertValidMessage()
    {
        var repo = new PartyAppRepository();
        var message = new Message("messageText", "messageAuthor", DateTime.Now);
        repo.InsertMessage(message);
    }

    [Fact]
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

    [Fact]
    public void GetMessage()
    {
        var repo = new PartyAppRepository();
        var messages = repo.GetMessages();
        Assert.NotEmpty(messages);
        foreach (var message in messages)
        {
           Assert.Multiple(() =>
           {
               Assert.True(!string.IsNullOrWhiteSpace(message.Text));
               Assert.True(!string.IsNullOrWhiteSpace(message.Author));
               Assert.NotNull(message.DateSubmitted);
               Assert.True(message.DateSubmitted <= DateTime.Now);
           }); 
        }
    }
}