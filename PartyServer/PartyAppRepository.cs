using System.Data;
using System.Data.SQLite;
using PartyModels;
using Serilog.Core;

namespace PartyServer;


public class PartyAppRepository
{
    private const string SqliteConnectionString = "Data Source=party.db";
    private readonly Logger _logger;
    public PartyAppRepository(Logger logger)
    {
        _logger = logger;
        InitMessageDb();
    }

    private void InitMessageDb()
    {
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();
        const string tableExistsQueryString = 
            "select name " +
            "FROM sqlite_master " +
            "where type='table' and name='Message'";
        using var tableExistsQuery = new SQLiteCommand(tableExistsQueryString, connection);
        using var reader = tableExistsQuery.ExecuteReader();

        if (reader.HasRows)
        {
            _logger.Information("Message table already exists, skipping creation");
            return;
        }
        
        _logger.Information("Message table does not exist, creating it");
        const string createMessageTable = 
            "CREATE TABLE IF NOT EXISTS Message " +
            "(Id INTEGER PRIMARY KEY, " +
            "Text TEXT NOT NULL, " +
            "Author TEXT NOT NULL, " +
            "DateSubmitted TEXT NOT NULL, " +
            "Guid TEXT NOT NULL)"; 
        using var createMessageTableCommand = new SQLiteCommand(createMessageTable, connection);
        createMessageTableCommand.CommandType = CommandType.Text;
        createMessageTableCommand.ExecuteNonQuery();
    }

    public void InsertMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) || string.IsNullOrWhiteSpace(message.Author))
        {
            _logger.Error("Could not insert message into database");
            throw new DataException("both message text and author must be populated");
        }
       
        // Open connection and build query
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();
        const string insertMessage = 
            "INSERT INTO Message (Text, Author, DateSubmitted,Guid) " +
            "VALUES (@Text, @Author, @DateSubmitted, @Guid)";
        using var insertMessageCommand = new SQLiteCommand(insertMessage, connection);
        insertMessageCommand.CommandType = CommandType.Text;
        insertMessageCommand.Parameters.AddWithValue("@Text", message.Text);
        insertMessageCommand.Parameters.AddWithValue("@Author", message.Author);
        insertMessageCommand.Parameters.AddWithValue("@DateSubmitted", message.DateSubmitted);
        insertMessageCommand.Parameters.AddWithValue("@Guid", message.Guid.ToString());
        
        var rowsInserted = insertMessageCommand.ExecuteNonQuery();
        if(rowsInserted == 0) 
            _logger.Error("Whoops, couldn't insert message into database");
        else 
            _logger.Information("Successfully inserted message into database");
    }

    public List<Message> GetMessages()
    {
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();

        const string selectQuery = "SELECT Text,Author,DateSubmitted,Guid FROM Message";
        using var selectMessagesCommand = new SQLiteCommand(selectQuery, connection);
        selectMessagesCommand.CommandType = CommandType.Text;
        using var reader = selectMessagesCommand.ExecuteReader();
        if (!reader.HasRows)
        {
            _logger.Warning("Could nto find any rows");
            return new List<Message>();
        }
        
        var messages = new List<Message>();
        while (reader.Read())
            messages.Add(new Message(reader,_logger));
        
        _logger.Information("Returning {MessageCount} # of messages", messages.Count);
        return messages;
    }

    public void DeleteMessage(Message message)
    {
        using var sqliteConnection = new SQLiteConnection(SqliteConnectionString);
        sqliteConnection.Open();
        var deleteMessage = "DELETE FROM Message WHERE Id = @Id";
        using var deleteMessageCommand = new SQLiteCommand(deleteMessage, sqliteConnection);
        deleteMessageCommand.CommandType = CommandType.Text;
        deleteMessageCommand.Parameters.AddWithValue("@Id", message.Guid);
        var rowsDeleted = deleteMessageCommand.ExecuteNonQuery();
        if (rowsDeleted == 0)
            _logger.Error("Whoops, couldn't delete message from database");
        else
            _logger.Information("Successfully deleted message from database");
    }

    public void UpvoteMessage(Guid messageGuid)
    {
        _logger.Information("upvote called for guid {Guid}", messageGuid.ToString("D"));
        throw new NotImplementedException();
    }

    public void DownvoteMessage(Guid messageGuid)
    {
        _logger.Information("Downvote called for guid {Guid}", messageGuid.ToString("D"));
        throw new NotImplementedException();
    }
}