using Avalonia.Controls;

namespace Thor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MethodTextBox.Text = "GET";
    }
}