using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using System.Windows;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Lógica interna para MainWindowOP20.xaml
    /// </summary>
    public partial class OP20_MainWindow : Window
    {

        // Window manager injected by dependency injection
        WindowManager _windowManager { get; set; }

        public OP20_MainWindow(WindowManager windowManager)
        {
            InitializeComponent();
            _windowManager = windowManager;
        }

        // Function to change the view on menu pageSelected
        private void OnPageSelected(string viewName)
        {

            OP10_MainWindowVM vm = DataContext as OP10_MainWindowVM;

            vm.CurrentPage = viewName;
            _windowManager.ShowPage(viewName, MainGridView);
        }

    }
}
