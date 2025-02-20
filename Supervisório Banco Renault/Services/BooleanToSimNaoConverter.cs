using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Supervisório_Banco_Renault.Services
{
    internal class BooleanToSimNaoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? "Sim" : "Não";
            }
            return "Desconhecido";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Equals("Sim", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
