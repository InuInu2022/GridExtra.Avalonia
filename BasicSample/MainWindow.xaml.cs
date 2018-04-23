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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var grid = this.FindControl<Grid>("grid");
            var temp = @"
            Header  Header
            Menu    SubMenu
            Content Content
            Footer  Footer
";
            
            grid.LayoutUpdated += (i,o) => 
            {

                Debug.Print("Yolo");
            };
            
            GridExtra.GridEx.SetTemplateArea(grid, temp);
        }
    }
}
