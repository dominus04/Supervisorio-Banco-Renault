using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
