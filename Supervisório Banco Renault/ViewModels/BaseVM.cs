using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
