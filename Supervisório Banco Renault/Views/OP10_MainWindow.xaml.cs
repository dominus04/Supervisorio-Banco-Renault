using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Lógica interna para MainWindowOP10.xaml
    /// </summary>
    public partial class OP10_MainWindow : Window
    {
        // Window manager injected by dependency injection
        WindowManager _windowManager { get; set; }

        public OP10_MainWindow(WindowManager windowManager)
        {
            InitializeComponent();
            _windowManager = windowManager;
        }

        // Function to change the view on menu pageSelected
        public void OnPageSelected(object sender, string viewName)
        {
            OP10_MainWindowVM vm = DataContext as OP10_MainWindowVM;

            vm.CurrentPage = viewName;
            _windowManager.ShowPage(viewName, MainGridView);
        }

        private void MainGridView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainGridView.Children.Count > 0)
                MainGridView.Children[0].Focus();
        }
    }
}
