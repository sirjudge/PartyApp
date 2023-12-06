using System;

namespace PartyApp;

public static class PhotoTaker
{
    public static void TakePhoto()
    {
        try
        {
            SavePhoto();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Whoops oopsie couldn't take a photo:{e.Message}");
        }
    }

    private static void SavePhoto()
    {
        throw new NotImplementedException();
    }
}