using System.Data;
using Serilog.Core;

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

    public Message(IDataReader reader, Logger logger)
    {
        Text = reader.GetString(0);
        Author = reader.GetString(1);
        
        //TODO: solve this issue later
        var dateTimeString = reader.GetString(2);
        if (DateTime.TryParse(dateTimeString, out var parsedDate)) DateSubmitted = parsedDate;
        else
        {
            var message = $"Could not parse date from string:{dateTimeString}";
            logger.Warning(message);
            DateSubmitted = DateTime.Now;
        }

        //TODO: Solve this later as well
        var guidString = reader.GetString(3);
        if (Guid.TryParse(guidString, out var parsedGuid)) Guid = parsedGuid;
        else
        {
            logger.Warning($"Could not parse guid from string:{guidString}");
            Guid = Guid.NewGuid();
        }
    }
    
    public override string ToString()
        =>  $"{Author}-{DateSubmitted}:{Text}";
    
    public void AddUpvote() => NumberUpvotes++;
    public void AddDownvote() => NumberDownvotes++;
    public int GetVibes() => NumberUpvotes - NumberDownvotes;
}