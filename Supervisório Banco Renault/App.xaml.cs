using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using S7.Net;
using Supervisório_Banco_Renault.Data;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.Windows;
using System.Windows.Threading;

namespace Supervisório_Banco_Renault
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly DispatcherTimer _watchTimer;
        public OP10_MainWindow? _mainWindowOP10;
        public OP20_MainWindow? _mainWindowOP20;
        private readonly IServiceCollection services = new ServiceCollection();
        public readonly IServiceProvider _serviceProvider;

        public App()
        {

            ConfigureServices();

            // Building the service
            _serviceProvider = services.BuildServiceProvider();

            // Timer responsible for update hour and date each one minute
            _watchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _watchTimer.Tick += WatchTimerTick;
            _watchTimer.Start();

            // Updating db if migration is needed
            ApplyMigration();

            // Getting the injected WindowManager from the service provider and opening the two screens
            var windowManager = _serviceProvider.GetRequiredService<WindowManager>();
            _mainWindowOP20 = (OP20_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP20_MainWindowVM>())!;
            _mainWindowOP10 = (OP10_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP10_MainWindowVM>())!;

            WatchTimerTick(this, EventArgs.Empty);
        }

        // Function to send to the header the command to update hour and date
        private void WatchTimerTick(object? sender, EventArgs e)
        {
            _mainWindowOP20?.HeaderUC.UpdateHourAndDate();
            _mainWindowOP10?.HeaderUC.UpdateHourAndDate();
        }

        // Function to configure the services
        private void ConfigureServices()
        {
            // Adding the injected DB classes to service
            services.AddDbContext<AppDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IOP10_TraceabilityRepository, OP10_TraceabilityRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IOP20_TraceabilityRepository, OP20_TraceabilityRepository>();

            // Adding the OP10 VMs to the service
            services.AddSingleton<OP10_MainWindowVM>();
            services.AddScoped<OP10_AutomaticVM>();
            services.AddScoped<OP10_ManualVM>();

            // Adding the OP20 VMs to service
            services.AddSingleton<OP20_MainWindowVM>();
            services.AddScoped<OP20_AutomaticVM>();


            // Adding the common VMs to service
            services.AddTransient<LoginVM>();
            services.AddTransient<LogoffVM>();
            services.AddTransient<UsersManagerVM>();
            services.AddTransient<RecipesManagerVM>();
            services.AddTransient<LabelsManagerVM>();
            services.AddTransient<AllowScreenVM>();

            // Adding the injected classes to service
            services.AddSingleton<ViewModelLocator>();
            services.AddSingleton<WindowMapper>();
            services.AddSingleton<UserControlMapper>();
            services.AddSingleton<WindowManager>();
            services.AddSingleton<PlcConnection>(provider => new PlcConnection(CpuType.S71200, "192.168.1.1", 0, 1));

            // Adding the services functions to the service


        }

        // Function to apply migration to the DB if needed
        private void ApplyMigration()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            PlcConnection plcConnection = _serviceProvider.GetService<PlcConnection>()!;
            await plcConnection.DeactivateOP20Automatic();
            await plcConnection.DeactivateOP10Automatic();
        }
    }

}
