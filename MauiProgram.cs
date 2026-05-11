using Microsoft.Extensions.Logging;

namespace SpecialCalculator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            // WinUI TextBox draws a bottom focus line; hide it when Entry is placed inside a styled Border.
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("FlatBorderlessEntry", (handler, _) =>
            {
                if (handler.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
                {
                    textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    textBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                }
            });
#endif

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
