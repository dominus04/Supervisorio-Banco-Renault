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

        // Method that returns the type of main windows to choose how VM use
        protected Type GetParent(DependencyObject obj)
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
    }
}
