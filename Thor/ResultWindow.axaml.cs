using System.Linq;
using System.Net.Http;
using Avalonia.Controls;
using System.Text;

namespace Thor;

public partial class ResultWindow : Window
{
    public ResultWindow()
    {
        InitializeComponent();
    }

    public ResultWindow(HttpResponseMessage response)
    {
        InitializeComponent();
        StatusLabel.Content = $"Status: {response.StatusCode}";
        foreach (var (k, v) in response.Headers.ToDictionary())
        {
            var builder = new StringBuilder();
            foreach (var value in v)
                builder.Append(value);
            
            HeadersListBox.Items.Add($"{k}: {builder}");
        }
        
        BodyTextBlock.Text = response.Content.ReadAsStringAsync().Result;
    }
}