using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient(typeof(SplashScreenWindow));
        }
    }

   
}
