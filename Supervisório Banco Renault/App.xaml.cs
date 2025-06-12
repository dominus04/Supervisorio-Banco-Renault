using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using S7.Net;
using Supervisório_Banco_Renault.Data;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.IO;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            //  Create log folder and delete odl
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logFolderPath = Path.Combine(appDataPath, "Supervisorio Banco Renault", "logs");

            if (!Directory.Exists(logFolderPath))
                Directory.CreateDirectory(logFolderPath);

            try
            {
                if (Directory.Exists(logFolderPath))
                {
                    var arquivos = Directory.GetFiles(logFolderPath, "*.log");

                    foreach (var arquivo in arquivos)
                    {
                        var info = new FileInfo(arquivo);
                        if (info.LastWriteTime < DateTime.Now.AddDays(-7))
                        {
                            info.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Erro ao limpar logs antigos");
            }


            // Get Unhandled Exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            logger.Trace("Aplicação iniciada.");

            ConfigureServices();

            // Building the service
            _serviceProvider = services.BuildServiceProvider();

            logger.Trace("Services da injeção de dependência configurados.");

            // Timer responsible for update hour and date each one minute
            _watchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _watchTimer.Tick += WatchTimerTick;
            _watchTimer.Start();

            // Updating db if migration is needed
            ApplyMigration();
            logger.Trace("Migrations do banco aplicadas.");


            // Getting the injected WindowManager from the service provider and opening the two screens
            var windowManager = _serviceProvider.GetRequiredService<WindowManager>();
            _mainWindowOP20 = (OP20_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP20_MainWindowVM>())!;
            _mainWindowOP10 = (OP10_MainWindow)windowManager.ShowWindow(_serviceProvider.GetRequiredService<OP10_MainWindowVM>())!;
            logger.Trace("Telas abertas.");

            WatchTimerTick(this, EventArgs.Empty);
        }

        // Function to send to the header the command to update hour and date
        private void WatchTimerTick(object? sender, EventArgs e)
        {
            _mainWindowOP20?.HeaderUC.UpdateHourAndDate();
            _ = _mainWindowOP20?.VerifyEmergency();
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
            services.AddTransient<OP10_TraceabilityVM>();

            // Adding the OP20 VMs to service
            services.AddSingleton<OP20_MainWindowVM>();
            services.AddScoped<OP20_AutomaticVM>();
            services.AddScoped<OP20_ManualVM>();
            services.AddTransient<OP20_EmergencyVM>();
            services.AddTransient<OP20_TraceabilityVM>();


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

        protected override void OnExit(ExitEventArgs e)
        {
            
            var mre = new ManualResetEventSlim(false);

            PlcConnection plcConnection = _serviceProvider.GetService<PlcConnection>()!;

            Task.Run(async () =>
            {
                await plcConnection.DeactivateOP20Automatic();
                await plcConnection.DeactivateOP10Automatic();
                await plcConnection.DeactivateOP20Manual();
                await plcConnection.DeactivateOP10Manual();
                mre.Set();
            });

            mre.Wait();

            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(e.Exception, "Unhandled UI exception occurred!");

            // Optionally inform the user in the UI
            MessageBox.Show($"Um erro não esperado aconteceu com a interface:\n\n{e.Exception.Message}\n\nPor favor verifique os logs para mais detalhes.",
                            "Erro na aplicação", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent the application from crashing immediately.
            // Setting e.Handled = true; tells WPF that you've dealt with the exception.
            // Use this with caution, as the application's state might be inconsistent.
            e.Handled = true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Fatal(ex, "Unhandled non-UI exception occurred!");

            MessageBox.Show("Um erro crítico aconteceu nos processamentos internos da aplicação. Ela pode estar instável e deve ser reiniciada.",
                            "Erro crítico", MessageBoxButton.OK, MessageBoxImage.Error);

            // In many cases, for unhandled non-UI exceptions, it's safer to allow the application to terminate.
            // If e.IsTerminating is true, the runtime is already planning to shut down.
            if (e.IsTerminating)
            {
                logger.Info("Application is terminating due to an unhandled non-UI exception.");
                // Perform any final cleanup before termination.
            }
        }

    }
}
