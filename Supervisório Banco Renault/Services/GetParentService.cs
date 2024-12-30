using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    public class GetParentService
    {
        // Method that returns the type of main windows to choose how VM use
        public static Type GetParent(DependencyObject obj)
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
