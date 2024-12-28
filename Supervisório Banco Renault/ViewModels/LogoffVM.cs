using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class LogoffVM : BaseVM
    {


        public IServiceProvider _serviceProvider;
        public UserRepository _userRepository;

        public LogoffVM(IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = (UserRepository)userRepository;
        }

        public void Logoff(DependencyObject obj)
        {
            Type type = GetParent(obj);
            WindowBaseVM vm;

                if (type == typeof(OP10_MainWindow))
                {
                    vm = _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
                    vm.LoggedUser = User.GetNullUser();
                }
                else
                {
                    vm = _serviceProvider.GetRequiredService<OP20_MainWindowVM>();
                    vm.LoggedUser = User.GetNullUser();
                }
            vm.ChangePage("Login");
        }
    }
}
