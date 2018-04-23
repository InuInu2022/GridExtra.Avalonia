using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace BasicSample.Wpf
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var grid = this.FindControl<Grid>("grid");
            var temp = @"
            Header  Header
            Menu    SubMenu
            Content Content
            Footer  Footer
";
            GridExtra.GridEx.SetTemplateArea(grid, temp);
        }
    }
}
