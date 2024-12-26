using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Core;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class LoginVM : BaseVM
    {

        public IServiceProvider _serviceProvider;

        public LoginVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            OP10_MainWindowVM op10Main = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
        }

        // Property to bind the RFID login
        private string _rfid;

        public string RFID
        {
            get { return _rfid; }
            set
            {
                _rfid = value;
                OnPropertyChanged(nameof(RFID));
            }
        }

        // Method that returns the type of main windows to choose how VM use
        private Type GetParent(DependencyObject obj)
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(obj);
            if (dependencyObject.GetType() == typeof(OP20_MainWindow) || dependencyObject.GetType() == typeof(OP10_MainWindow))
            {
                return dependencyObject.GetType();
            }
            else
            {
                return GetParent(dependencyObject);
            }
        }

        public void Login(DependencyObject obj)
        {
            Type type = GetParent(obj);

            MessageBox.Show(RFID);

            if(type == typeof(OP10_MainWindow))
            {
                OP10_MainWindowVM vm = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
                vm.LoggedUser = new Models.User("Shaolin Matador de Porco", string.Empty, Models.Enums.AccessLevel.Administrador);
                vm.ChangePage("OP10_Automatic");
            }
            else if (type == typeof(OP20_MainWindow)) {
                OP20_MainWindowVM vm = _serviceProvider.GetRequiredService<OP20_MainWindowVM>();
                vm.LoggedUser = new Models.User("Pedrinho", string.Empty, Models.Enums.AccessLevel.Operador);
                vm.ChangePage("OP20_Automatic");
            }
        }
    }
}
