using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Lógica interna para OP20_Emergency.xaml
    /// </summary>
    public partial class OP20_Emergency : Window
    {
        public OP20_Emergency()
        {
            InitializeComponent();
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            this.Left = (1920 - this.Width) / 2;
            this.Top = (1080 - this.Height) / 2;
        }
    }
}
