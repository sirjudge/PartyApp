
namespace PartyModels;

public class Message
{
    public string? Text { get; set; }
    public string? Author { get; set; }
    public DateTime DateSubmitted { get; set; }
    public Guid Guid { get; set; }
    public int NumberUpvotes { get; set; }
    public int NumberDownvotes { get; set; }

    public Message(string? text, string? author, DateTime dateSubmitted)
    {
       Text = text;
       Author = author;
       DateSubmitted = dateSubmitted;
       Guid = Guid.NewGuid();
    }
    
    public override string ToString()
        =>  $"{Author}-{DateSubmitted}:{Text}";
    
    public void AddUpvote() => NumberUpvotes++;
    public void AddDownvote() => NumberDownvotes++;
    public int GetVibes() => NumberUpvotes - NumberDownvotes;
}