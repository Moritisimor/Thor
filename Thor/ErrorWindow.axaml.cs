using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Thor;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(Exception ex)
    {
        InitializeComponent();
        TitleLabel.Content = ex.GetType().ToString();
        MessageTextBlock.Text = ex.Message;
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}