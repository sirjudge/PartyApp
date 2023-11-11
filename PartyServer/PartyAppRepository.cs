using System.Data;
using System.Data.SQLite;
using PartyModels;
using Serilog.Core;

namespace PartyServer;


public class PartyAppRepository
{
    private readonly string SqliteConnectionString;
    private readonly Logger Logger;
    public PartyAppRepository(Logger logger)
    {
        Logger = logger;
        SqliteConnectionString = "Data Source=party.db";
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();
        InitMessageDb(connection);
        connection.Close();
    }

    private void InitMessageDb(SQLiteConnection connection)
    {
        const string tableExistsQueryString = 
            "select name " +
            "FROM sqlite_master " +
            "where type='table' and name='Message'";
        using var tableExistsQuery = new SQLiteCommand(tableExistsQueryString, connection);
        using var reader = tableExistsQuery.ExecuteReader();
        if (!reader.HasRows)
        {
            Logger.Information("Message table does not exist, creating it");
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

        /*
         TODO: this is logic to clear the db out before running
        if (!reinitializeDbFile)
        {
            Logger.Information("Not reinitializing db file, returning early");
            return;
        }
        
        const string deleteQuery = "delete from Message";
        using var deleteCommand = new SQLiteCommand(deleteQuery, connection);
        deleteCommand.ExecuteNonQuery();
        Logger.Information("Deleted all rows from Message table");
        */
    }

    public void InsertMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) || string.IsNullOrWhiteSpace(message.Author))
            return;
         
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
            Logger.Error("Whoops, couldn't insert message into database");
        else 
            Logger.Information("Successfully inserted message into database");
    }

    public List<Message> GetMessages()
    {
        Logger.Information($"SqliteConnectionString:{SqliteConnectionString}");
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();
        Logger.Information("Opened connection to party db");
        
        using var selectMessagesCommand = new SQLiteCommand("SELECT * FROM Message", connection);
        selectMessagesCommand.CommandType = CommandType.Text;
        using var reader = selectMessagesCommand.ExecuteReader();
        Logger.Information("Executed reader on party db");
        var messages = new List<Message>();

        if (!reader.HasRows)
        {
            Logger.Warning("Could nto find any rows");
            return messages;
        }
        
        while (reader.Read())
        {
            Logger.Debug("Reading message row from database");
            var message = new Message(reader.GetString(1), reader.GetString(2), DateTime.Parse(reader.GetString(3)));
            messages.Add(message);
        }
        
        Logger.Information("Returning {MessageCount} # of messages", messages.Count);
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
            Logger.Error("Whoops, couldn't delete message from database");
        else
            Logger.Information("Successfully deleted message from database");
    }

    public void UpvoteMessage(Guid messageGuid)
    {
        Logger.Information("upvote called for guid {Guid}", messageGuid.ToString("D"));
        throw new NotImplementedException();
    }

    public void DownvoteMessage(Guid messageGuid)
    {
        Logger.Information("Downvote called for guid {Guid}", messageGuid.ToString("D"));
        throw new NotImplementedException();
    }
}