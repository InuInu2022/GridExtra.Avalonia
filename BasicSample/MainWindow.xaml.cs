using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;


namespace BasicSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public class MainWindow : Window
    {
        private Grid sampleGrid;

        public MainWindow()
        {
            InitializeComponent();
            this.sampleGrid = this.FindControl<Grid>("grid");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var temp = @"
            Header  Header
            Menu    SubMenu
            Content Content
            Footer  Footer
";
            GridExtra.GridEx.SetTemplateArea(sampleGrid, temp);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
