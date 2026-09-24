using System;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Thor;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    
    public MainWindow()
    {
        InitializeComponent();
        MethodTextBox.Text = "GET";
        _httpClient = new HttpClient();
    }

    private async void SendButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Parse(MethodTextBox.Text), UrlTextBox.Text);
            var response = await _httpClient.SendAsync(request);
            Console.WriteLine($"Status: {response.StatusCode}, Body: {response.Content.ReadAsStringAsync().Result}");
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.GetType().ToString(), ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }
}