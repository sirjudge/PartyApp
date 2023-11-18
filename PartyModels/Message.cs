using System.Data;
using Serilog.Core;

namespace PartyModels;

public class Message
{
    public int MessageId { get; set; }
    public string? Text { get; set; }
    public string? Author { get; set; }
    public DateTime DateSubmitted { get; set; }
    public Guid Guid { get; set; }
    public int NumberUpvotes { get; set; }
    public int NumberDownvotes { get; set; }

    public Message(){}
    
    public Message(string? text, string? author, DateTime dateSubmitted)
    {
        Text = text;
        Author = author;
        DateSubmitted = dateSubmitted;
        Guid = Guid.NewGuid();
    }

    public Message(IDataReader reader, Logger logger)
    {
        MessageId = reader.GetInt16(reader.GetOrdinal("Id"));
        Text = reader.GetString(reader.GetOrdinal("Text"));
        Author = reader.GetString(reader.GetOrdinal("Author"));
        
        var dateTimeString = reader.GetString(reader.GetOrdinal("DateSubmitted"));
        if (DateTime.TryParse(dateTimeString, out var parsedDate)) DateSubmitted = parsedDate;
        else
        {
            var message = $"Could not parse date from string:{dateTimeString}";
            logger.Warning(message);
            DateSubmitted = DateTime.Now;
        }

        var guidString = reader.GetString(reader.GetOrdinal("Guid"));
        if (Guid.TryParse(guidString, out var parsedGuid)) Guid = parsedGuid;
        else
        {
            logger.Warning($"Could not parse guid from string:{guidString}");
            Guid = Guid.NewGuid();
        }
    }
    
    public override string ToString()
        =>  $"{Author}-{DateSubmitted.ToShortTimeString()}:{Text}";
    
    public void AddUpvote() => NumberUpvotes++;
    public void AddDownvote() => NumberDownvotes++;
    public int GetVibes() => NumberUpvotes - NumberDownvotes;
}