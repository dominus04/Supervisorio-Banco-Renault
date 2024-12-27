using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.Views;
using System.Windows;
using System.Windows.Media;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class LoginVM : BaseVM
    {

        public IServiceProvider _serviceProvider;
        public UserRepository _userRepository;

        public LoginVM(IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = (UserRepository)userRepository;

            OP10_MainWindowVM op10Main = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
        }

        // Property to bind the RFID login
        private string _rfid = string.Empty;

        public string RFID
        {
            get => _rfid;
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

        public async void Login(DependencyObject obj)
        {
            Type type = GetParent(obj);

            await _userRepository.DeleteUserById(new Guid("A1474C91-14E6-4F42-B66C-48AD4F08F68A"));

            var user = await _userRepository.GetUserByRFID(RFID);
            if (user != null)
            {
                if (type == typeof(OP10_MainWindow))
                {
                    OP10_MainWindowVM vm = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
                    vm.LoggedUser = user;
                    vm.ChangePage("OP10_Automatic");
                }
                else if (type == typeof(OP20_MainWindow))
                {
                    OP20_MainWindowVM vm = _serviceProvider.GetRequiredService<OP20_MainWindowVM>();
                    vm.LoggedUser = user;
                    vm.ChangePage("OP20_Automatic");
                }
            }
            else
            {
                MessageBox.Show("Usuário não encontrado");
            }
        }
    }
}
