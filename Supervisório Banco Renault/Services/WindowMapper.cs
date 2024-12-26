using Supervisório_Banco_Renault.ViewModels;
using Supervisório_Banco_Renault.Views;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    // Responsible class for mapping the Window based on the viewmodel
    public class WindowMapper
    {
        private readonly Dictionary<Type, Type> _windowMapping = new();
        public WindowMapper()
        {
            RegisterMapping<OP10_MainWindowVM, OP10_MainWindow>();
            RegisterMapping<OP20_MainWindowVM, OP20_MainWindow>();
        }

        public void RegisterMapping<TViewModel, TWindow>() where TViewModel : WindowBaseVM where TWindow : Window
        {
            _windowMapping[typeof(TViewModel)] = typeof(TWindow);
        }

        public Type? GetWindowTypeForViewModel(Type viewModelType)
        {
            _windowMapping.TryGetValue(viewModelType, out var windowType);
            return windowType;
        }
    }
}
