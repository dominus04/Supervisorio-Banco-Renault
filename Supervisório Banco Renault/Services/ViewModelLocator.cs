using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.ViewModels;

namespace Supervisório_Banco_Renault.Services
{
    //Class responsible for keeping the contorl and adding the ViewModels to the service provider (Dependency Injection)
    public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public OP10_MainWindowVM MainWindowOP10VM => _serviceProvider.GetRequiredService<OP10_MainWindowVM>();
        public OP20_MainWindowVM MainWindowOP20VM => _serviceProvider.GetRequiredService<OP20_MainWindowVM>();
        public OP10_AutomaticVM AutomaticOP10VM => _serviceProvider.GetRequiredService<OP10_AutomaticVM>();
    }
}
