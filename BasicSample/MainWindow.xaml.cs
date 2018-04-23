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
            this.Activated += MW_Activated;
        }

        private void MW_Activated(object sender, EventArgs e)
        {
            var temp = @"
             Header Header Header/
            Menu Content SubMenu/
            Footer Footer Footer/
";
            GridExtra.GridEx.SetTemplateArea(sampleGrid, temp);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
 
    }
}
