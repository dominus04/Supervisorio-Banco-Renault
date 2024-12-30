using Supervisório_Banco_Renault.Views;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class BaseVM : INotifyPropertyChanged
    {
        //PropertyChanged Method and Event

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
