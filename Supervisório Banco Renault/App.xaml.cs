using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace Supervisório_Banco_Renault
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DispatcherTimer _watchTimer;
        public OP10_MainWindow? _mainWindowOP10;
        public OP20_MainWindow? _mainWindowOP20;
        private readonly IServiceCollection services = new ServiceCollection();
        private readonly IServiceProvider _serviceProvider;

        public App()
        {

            ConfigureServices();

            // Building the service
            _serviceProvider = services.BuildServiceProvider();

            // Timer responsible for update hour and date each one minute
            _watchTimer = new DispatcherTimer();
            _watchTimer.Interval = TimeSpan.FromMinutes(1);
            _watchTimer.Tick += WatchTimerTick;
            _watchTimer.Start();

            // Getting the injected WindowManager from the service provider and opening the two screens
            var windowManager = _serviceProvider.GetRequiredService<WindowManager>();
            _mainWindowOP10 = (OP10_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP10_MainWindowVM>());
            _mainWindowOP20 = (OP20_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP20_MainWindowVM>());

            WatchTimerTick(this, EventArgs.Empty);
        }

        // Function to send to the header the command to update hour and date
        private void WatchTimerTick(object? sender, EventArgs e)
        {
            _mainWindowOP10.HeaderUC.UpdateHourAndDate();
            _mainWindowOP20.HeaderUC.UpdateHourAndDate();
        }

        private  void ConfigureServices()
        {
            // Adding the injected DB classes to service
            services.AddDbContext<AppDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Adding the OP10 VMs to the service
            services.AddSingleton<OP10_MainWindowVM>();
            services.AddSingleton<OP10_AutomaticVM>();
            services.AddSingleton<OP10_ManualVM>();

            // Adding the OP20 VMs to service
            services.AddSingleton<OP20_MainWindowVM>();
            services.AddSingleton<OP20_AutomaticVM>();

            services.AddTransient<LoginVM>();

            // Adding the injected classes to service
            services.AddSingleton<ViewModelLocator>();
            services.AddSingleton<WindowMapper>();
            services.AddSingleton<UserControlMapper>();
            services.AddSingleton<WindowManager>();

            // Adding the services functions to the service
        }

    }

}
