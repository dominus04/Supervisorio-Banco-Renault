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
    /// Lógica interna para AllowScreen.xaml
    /// </summary>
    public partial class AllowScreen : Window
    {
        public AllowScreen()
        {
            InitializeComponent();
        }

        private void CancelButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ButtonReadMouseDown(object sender, MouseButtonEventArgs e)
        {
            textRFID.Text = "";
            textRFID.Focus();
        }

        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AllowScreenVM vm = (AllowScreenVM)DataContext;
                await vm.TryAllow();
                if (vm.IsAllowed)
                {
                    this.Close();
                }
            }
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            this.Left = (1920 - this.Width) / 2;
            this.Top = (1080 - this.Height) / 2;
        }
    }
}
