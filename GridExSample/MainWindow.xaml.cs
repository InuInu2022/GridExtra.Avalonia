using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GridExSample;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public class MainWindow : Window
{
    private Grid sampleGrid;
    private string newTempArea = @"
        Header  Header
        Menu    SubMenu
        Content Content
        Footer  Footer";
    private string oldTempArea = "";

    public MainWindow()
    {
        InitializeComponent();
        this.sampleGrid = this.FindControl<Grid>("grid");
    }

    public void Button_Click(object sender, RoutedEventArgs e)
    {
        oldTempArea = GridExtra.Avalonia.GridEx.GetTemplateArea(sampleGrid);
        
        GridExtra.Avalonia.GridEx.SetTemplateArea(sampleGrid, newTempArea);

        newTempArea = oldTempArea;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
