using System.ComponentModel;

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
