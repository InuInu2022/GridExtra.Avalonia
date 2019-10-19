using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Logging.Serilog;
using Avalonia.Logging;

namespace ResponsiveGridSample
{
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
                .UsePlatformDetect();
    }
}
