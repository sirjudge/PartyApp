using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PartyApp;

public class PartyAppRepository
{
    private readonly string SqliteConnectionString;
    public PartyAppRepository()
    {
        if (File.Exists("party.sqlite")) return;
        SqliteConnectionString = "Data Source=party.db";
        using var connection = new SqliteConnection(SqliteConnectionString);
        connection.Open();
        InitMessageDb(connection);
    }

    private void InitMessageDb(SqliteConnection connection)
    {
       var createMessageTable = "CREATE TABLE IF NOT EXISTS Message (Id INTEGER PRIMARY KEY, Text TEXT NOT NULL, Author TEXT NOT NULL, DateSubmitted TEXT NOT NULL)"; 
       using var createMessageTableCommand = new SqliteCommand(createMessageTable, connection);
       createMessageTableCommand.CommandType = CommandType.Text;
       createMessageTableCommand.ExecuteNonQuery();
    }

    public void InsertMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) || string.IsNullOrWhiteSpace(message.Author))
            return;
         
        using var connection = new SqliteConnection(SqliteConnectionString);
        connection.Open();
        var insertMessage = "INSERT INTO Message (Text, Author, DateSubmitted) VALUES (@Text, @Author, @DateSubmitted)";
        using var insertMessageCommand = new SqliteCommand(insertMessage, connection);
        insertMessageCommand.CommandType = CommandType.Text;
        insertMessageCommand.Parameters.AddWithValue("@Text", message.Text);
        insertMessageCommand.Parameters.AddWithValue("@Author", message.Author);
        insertMessageCommand.Parameters.AddWithValue("@DateSubmitted", message.DateSubmitted);
        var rowsInserted = insertMessageCommand.ExecuteNonQuery();
        if(rowsInserted == 0) 
            Console.Error.WriteLine("Whoops, couldn't insert message into database");
        else 
            Console.WriteLine("Successfully inserted message into database");
    }
}