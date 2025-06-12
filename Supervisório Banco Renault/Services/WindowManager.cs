using Microsoft.Extensions.DependencyInjection;
using NLog;
using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.Windows;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.Services
{
    // Responsible class for managing the opening of windows and pages
    public class WindowManager
    {
        private readonly WindowMapper _windowMapper;
        private readonly UserControlMapper _userControlMapper;
        private readonly IServiceProvider _serviceProvider;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public WindowManager(WindowMapper windowMapper, IServiceProvider serviceProvider)
        {
            _windowMapper = windowMapper;
            _userControlMapper = new UserControlMapper();
            _serviceProvider = serviceProvider;
        }


        //Method to open the OP10 and OP20 windows
        public Window? ShowWindow(WindowBaseVM windowVM)
        {
            Type? windowType = _windowMapper.GetWindowTypeForViewModel(windowVM.GetType());

            if (windowType != null)
            {
                var window = Activator.CreateInstance(windowType, this) as Window;
                window.DataContext = windowVM;

                if (window is OP10_MainWindow)
                {
                    windowVM.PageChanged += ((OP10_MainWindow)window).OnPageSelected;
                }
                else if (window is OP20_MainWindow)
                {
                    windowVM.PageChanged += ((OP20_MainWindow)window).OnPageSelected;
                }

                if (windowType == typeof(OP10_MainWindow))
                {
                    window.Left = 1920;
                }

                window.Show();
                window.WindowState = System.Windows.WindowState.Maximized;

                return window;

            }
                return null;
        }



        public void ShowPage(string pageVM, Grid grid)
        {
            Type? vmName = Type.GetType("Supervisório_Banco_Renault.ViewModels." + pageVM + "VM");
            var viewModel = _serviceProvider.GetRequiredService(vmName);
            Type? pageType = _userControlMapper.GetPageTypeForViewModel(vmName);

            if (pageType != null)
            {
                var page = Activator.CreateInstance(pageType) as UserControl;
                page.DataContext = viewModel;

                grid.Children.Clear();
                grid.Children.Add(page);
                logger.Trace($"Foi chamada a página {pageType.ToString().Split(".")[2]}");
            }
        }
    }
}
