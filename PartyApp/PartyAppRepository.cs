using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PartyApp;

public class PartyAppRepository
{
    private readonly string SqliteConnectionString;
    public PartyAppRepository(bool reinitializeDbFile = false)
    {
        SqliteConnectionString = "Data Source=party.db";
        using var connection = new SqliteConnection(SqliteConnectionString);
        connection.Open();
        InitMessageDb(connection, reinitializeDbFile);
        
    }

    private void InitMessageDb(SqliteConnection connection, bool reinitializeDbFile)
    {
        const string tableExistsQueryString = "select name FROM sqlite_master where type='table' and name='Message'";
        using var tableExistsQuery = new SqliteCommand(tableExistsQueryString, connection);
        using var reader = tableExistsQuery.ExecuteReader();
        if (!reader.HasRows)
        {
            var createMessageTable =
                "CREATE TABLE IF NOT EXISTS Message " +
                "(Id INTEGER PRIMARY KEY, " +
                "Text TEXT NOT NULL, " +
                "Author TEXT NOT NULL, " +
                "DateSubmitted TEXT NOT NULL, " +
                "Guid TEXT NOT NULL)"; 
            using var createMessageTableCommand = new SqliteCommand(createMessageTable, connection);
            createMessageTableCommand.CommandType = CommandType.Text;
            createMessageTableCommand.ExecuteNonQuery();
        }
        
        if (!reinitializeDbFile) return;
        
        const string deleteQuery = "delete from Message";
        using var deleteCommand = new SqliteCommand(deleteQuery, connection);
        deleteCommand.ExecuteNonQuery();
    }

    public void InsertMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) || string.IsNullOrWhiteSpace(message.Author))
            return;
         
        using var connection = new SqliteConnection(SqliteConnectionString);
        connection.Open();
        var insertMessage = "INSERT INTO Message (Text, Author, DateSubmitted,Guid) VALUES (@Text, @Author, @DateSubmitted, @Guid)";
        using var insertMessageCommand = new SqliteCommand(insertMessage, connection);
        insertMessageCommand.CommandType = CommandType.Text;
        insertMessageCommand.Parameters.AddWithValue("@Text", message.Text);
        insertMessageCommand.Parameters.AddWithValue("@Author", message.Author);
        insertMessageCommand.Parameters.AddWithValue("@DateSubmitted", message.DateSubmitted);
        insertMessageCommand.Parameters.AddWithValue("@Guid", message.Guid.ToString());
        var rowsInserted = insertMessageCommand.ExecuteNonQuery();
        if(rowsInserted == 0) 
            Console.Error.WriteLine("Whoops, couldn't insert message into database");
        else 
            Console.WriteLine("Successfully inserted message into database");
    }

    public List<Message> GetMessages()
    {
        using var connection = new SqliteConnection(SqliteConnectionString);
        var selectMessages = "SELECT * FROM Message";
        using var selectMessagesCommand = new SqliteCommand(selectMessages, connection);
        selectMessagesCommand.CommandType = CommandType.Text;
        connection.Open();
        using var reader = selectMessagesCommand.ExecuteReader();
        var messages = new List<Message>();
        while (reader.Read())
        {
            var message = new Message(reader.GetString(1), reader.GetString(2), DateTime.Parse(reader.GetString(3)));
            messages.Add(message);
        }

        return messages;
    }

    public void DeleteMessage(Message message)
    {
        using var sqliteConnection = new SqliteConnection(SqliteConnectionString);
        sqliteConnection.Open();
        var deleteMessage = "DELETE FROM Message WHERE Id = @Id";
        using var deleteMessageCommand = new SqliteCommand(deleteMessage, sqliteConnection);
        deleteMessageCommand.CommandType = CommandType.Text;
        deleteMessageCommand.Parameters.AddWithValue("@Id", message.Guid);
        var rowsDeleted = deleteMessageCommand.ExecuteNonQuery();
        if (rowsDeleted == 0)
            Console.Error.WriteLine("Whoops, couldn't delete message from database");
        else
            Console.WriteLine("Successfully deleted message from database");
    }
    
    public void ClearMessages()
    {
        
    }
}