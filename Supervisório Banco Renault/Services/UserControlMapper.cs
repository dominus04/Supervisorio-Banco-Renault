using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.Services
{
    // Responsible class for mapping the UserControl based on the viewmodel
    public class UserControlMapper
    {
        private readonly Dictionary<Type, Type> _userControlMapping = [];

        public UserControlMapper()
        {
            //Defining the view for each OP10 VM
            RegisterMapping<OP10_AutomaticVM, OP10_Automatic>();
            RegisterMapping<LoginVM, Login>();
            RegisterMapping<OP10_ManualVM,  OP10_Manual>();

            //Defining the view for each OP20 VM
            RegisterMapping<OP20_AutomaticVM,  OP20_Automatic>();
        }

        private void RegisterMapping<TViewModel, TWindow>() where TViewModel : BaseVM where TWindow : UserControl
        {
            _userControlMapping[typeof(TViewModel)] = typeof(TWindow);
        }

        public Type? GetPageTypeForViewModel(Type viewModelType)
        {
            _userControlMapping.TryGetValue(viewModelType, out var pageType);
            return pageType;
        }
    }
}
