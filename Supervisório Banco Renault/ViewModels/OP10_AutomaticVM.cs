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
    public class OP10_AutomaticVM : BaseVM
    {

        public IServiceProvider serviceProvider;

        public OP10_AutomaticVM(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }



    }
}
