﻿using System;

namespace PartyApp;

public class Message
{
    public string? Text { get; set; }
    public string? Author { get; set; }
    public DateTime DateSubmitted { get; set; }
    public Guid Guid { get; set; }

    public Message(string? text, string? author, DateTime dateSubmitted)
    {
       Text = text;
       Author = author;
       DateSubmitted = dateSubmitted;
       Guid = Guid.NewGuid();
    }
    
    public override string ToString()
        =>  $"{Author}-{DateSubmitted}:{Text}";
}