using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using PartyModels;
using Serilog;
using Serilog.Core;

namespace PartyServer;


public class PartyAppRepository
{
    private readonly string SqliteConnectionString;
    private readonly Logger Logger;
    public PartyAppRepository(Logger logger, bool reinitializeDbFile = false)
    {
        Logger = logger;
        SqliteConnectionString = "Data Source=party.db";
        using var connection = new SQLiteConnection(SqliteConnectionString);
        connection.Open();
        InitMessageDb(connection, reinitializeDbFile);
        connection.Close();
    }

    private void InitMessageDb(SQLiteConnection connection, bool reinitializeDbFile)
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

        if (!reinitializeDbFile)
        {
            Logger.Information("Not reinitializing db file, returning early");
            return;
        }
        
        const string deleteQuery = "delete from Message";
        using var deleteCommand = new SQLiteCommand(deleteQuery, connection);
        deleteCommand.ExecuteNonQuery();
        Logger.Information("Deleted all rows from Message table");
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
        using var connection = new SQLiteConnection(SqliteConnectionString);
        const string selectMessages = "SELECT * FROM Message";
        using var selectMessagesCommand = new SQLiteCommand(selectMessages, connection);
        selectMessagesCommand.CommandType = CommandType.Text;
        connection.Open();
        using var reader = selectMessagesCommand.ExecuteReader();
        var messages = new List<Message>();
        while (reader.Read())
        {
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