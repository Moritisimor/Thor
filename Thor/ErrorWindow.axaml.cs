using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Thor;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(string title, string message)
    {
        InitializeComponent();
        TitleLabel.Content = title;
        MessageTextBlock.Text = message;
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}