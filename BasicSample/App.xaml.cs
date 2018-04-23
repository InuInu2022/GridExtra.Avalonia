using Avalonia;
using Avalonia.Markup.Xaml;

namespace BasicSample.Wpf
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI();

    }
}
