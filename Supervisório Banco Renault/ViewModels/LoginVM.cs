using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.Services;
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

        // Method to verify the existence of the user and login in the correct main window
        public async void Login(DependencyObject obj, string password)
        {
            Type type = GetParentService.GetParent(obj);

            var user = await _userRepository.GetUserByRFID(RFID);

            if (user != null && user.TryLogin(password))
            {
                if (type == typeof(OP10_MainWindow))
                {
                    OP10_MainWindowVM vm = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
                    vm.LoggedUser = user;
                    vm.ChangePage("OP10_Automatic");
                }
                else
                {
                    OP20_MainWindowVM vm = _serviceProvider.GetRequiredService<OP20_MainWindowVM>();
                    vm.LoggedUser = user;
                    vm.ChangePage("OP20_Automatic");
                }
            }
            else if(user == null)
            {
                MessageBox.Show("Usuário não encontrado.");
            }
            else
            {
                MessageBox.Show("Senha incorreta.");
            }
        }
    }
}
