using System;

namespace PartyApp;

public static class Connection
{
       public static string GetServerUrl(bool debug)
           => debug ? "http://localhost:5046" : "http://nuggetbox:5046";
}