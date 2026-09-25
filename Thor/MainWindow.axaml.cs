using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Thor;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    
    public MainWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        HeadersListBox.Items.Add("User-Agent: Thor");
    }

    private async void SendButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Parse(MethodTextBox.Text), UrlTextBox.Text);
            request.Content = new StringContent(BodyTextBox?.Text ?? "");
            var contentTypeHeader = new MediaTypeHeaderValue(ContentTypeTextBox.Text ?? "text/plain");
            request.Content.Headers.ContentType = contentTypeHeader;

            if (CheckJsonCheckBox.IsChecked == true && (ContentTypeTextBox?.Text?.Contains("json") ?? false))
                System.Text.Json.JsonSerializer.Deserialize<object>(BodyTextBox?.Text ?? "");

            var response = await _httpClient.SendAsync(request);
            var resultWindow = new ResultWindow(response);
            resultWindow.Show(this);
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.GetType().ToString(), ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }

    private async void AddHeaderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(HeaderKeyTextBox.Text))
                throw new InvalidOperationException("Bad Header key!");
            
            if (string.IsNullOrWhiteSpace(HeaderValueTextBox.Text))
                throw new InvalidOperationException("Bad Header value!");
            
            if (HeaderKeyTextBox.Text.Contains(' ') 
                || HeaderKeyTextBox.Text.Contains('\t')
                || HeaderKeyTextBox.Text.Contains('\n')
                || HeaderKeyTextBox.Text.Contains('\r'))
                throw new InvalidOperationException("Header keys may not contain whitespace");

            HeadersListBox.Items.Add($"{HeaderKeyTextBox.Text.Trim()}: {HeaderValueTextBox.Text.Trim()}");
            HeaderKeyTextBox.Text = "";
            HeaderValueTextBox.Text = "";
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.GetType().ToString(), ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }

    private void DeleteHeaderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (HeadersListBox.SelectedItem is not string item)
            return;
        
        HeadersListBox.Items.Remove(item);
        HeadersListBox.SelectedItem = null;
    }
}
