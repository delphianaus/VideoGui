using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Wpf.Ui.Appearance;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set theme
            ApplicationThemeManager.Apply(
                ApplicationTheme.Dark
            );
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient(typeof(SplashScreenWindow));
        }
    }


}
