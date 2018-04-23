using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Logging.Serilog;
using Avalonia.Logging;

namespace BasicSample
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
                .UseReactiveUI()
                .LogToDebug(LogEventLevel.Verbose);

    }
}
