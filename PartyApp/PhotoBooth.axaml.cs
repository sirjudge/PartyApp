using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PartyApp;

public partial class PhotoBoothWindow : Window
{
    public PhotoBoothWindow()
    {
        InitializeComponent();
    }

    private void TakePhoto(object? sender, RoutedEventArgs e)
    {
        //load webcam/camera
        //take photo
        //send photo to backend
    }
}