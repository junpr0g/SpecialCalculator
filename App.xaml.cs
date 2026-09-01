using Microsoft.Extensions.DependencyInjection;

namespace SpecialCalculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppThemeService.ApplySavedTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}