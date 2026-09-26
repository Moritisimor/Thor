using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Thor;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    private int _concurrentDownloads;
    private readonly Lock _lock = new();

    private void IncrementDownloads()
    {
        lock (_lock)
        {
            _concurrentDownloads++;
            DownloadsLabel.Content = $"Downloads: {_concurrentDownloads}";
        }
    }
    
    private void DecrementDownloads()
    {
        lock (_lock)
        {
            _concurrentDownloads--;
            DownloadsLabel.Content = $"Downloads: {_concurrentDownloads}";
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        HeadersListBox.Items.Add(new HeaderPair("User-Agent", "Thor"));
    }

    private async Task<HttpResponseMessage> SendRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Parse(MethodTextBox.Text), UrlTextBox.Text);
        request.Content = new StringContent(BodyTextBox?.Text ?? "");
        var contentTypeHeader = new MediaTypeHeaderValue(ContentTypeTextBox.Text ?? "text/plain");
        request.Content.Headers.ContentType = contentTypeHeader;

        if (CheckJsonCheckBox.IsChecked == true && (ContentTypeTextBox?.Text?.Contains("json") ?? false))
            System.Text.Json.JsonSerializer.Deserialize<object>(BodyTextBox?.Text ?? "");
            
        foreach (var header in HeadersListBox.Items)
            if (header is HeaderPair headerPair)
                request.Headers.Add(headerPair.Key, headerPair.Value);
            
        return await _httpClient.SendAsync(request);
    }

    private async void SendButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var response = await SendRequest();
            var resultWindow = new ResultWindow(response);
            resultWindow.Show(this);
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex);
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

            HeadersListBox.Items.Add(new HeaderPair(HeaderKeyTextBox.Text.Trim(), HeaderValueTextBox.Text.Trim()));
            HeaderKeyTextBox.Text = "";
            HeaderValueTextBox.Text = "";
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex);
            await errorWindow.ShowDialog(this);
        }
    }

    private void DeleteHeaderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (HeadersListBox.SelectedItem is not HeaderPair item)
            return;
        
        HeadersListBox.Items.Remove(item);
        HeadersListBox.SelectedItem = null;
    }

    private async void DownloadButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            IncrementDownloads();
            var topLevel = GetTopLevel(this);
            if (topLevel is null)
                throw new InvalidOperationException("Top level is null");
            
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Where should I save the file?"
            });
            
            if (file is null)
                return; // Nothing selected

            await using var stream = await file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(stream);

            var res = await SendRequest();
            await streamWriter.WriteAsync(await res.Content.ReadAsStringAsync());
            
            var resultWindow = new InfoWindow(
                "Download Successful", 
                $"File downloaded successfully and saved to {file.Path}");
            
            DecrementDownloads();
            await resultWindow.ShowDialog(this);
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex);
            await errorWindow.ShowDialog(this);
        }
    }
}
