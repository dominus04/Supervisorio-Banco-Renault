using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.UserControls;
using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        private void OnPageSelected(string viewName)
        {

            OP10_MainWindowVM vm = DataContext as OP10_MainWindowVM;

            vm.CurrentPage = viewName;
            _windowManager.ShowPage(viewName, MainGridView);
        }
    }
}
