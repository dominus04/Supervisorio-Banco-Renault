using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Supervisório_Banco_Renault.UserControls
{
    /// <summary>
    /// Interação lógica para MainHeaderUC.xam
    /// </summary>
    public partial class MainHeaderUC : UserControl
    {
        public MainHeaderUC()
        {
            InitializeComponent();
        }

        public string HeaderTitle
        {
            get => (string)GetValue(HeaderTitleProperty);
            set { SetValue(HeaderTitleProperty, value); }
        }

        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register("HeaderTitle", typeof(string), typeof(MainHeaderUC));

        public User LoggedUser
        {
            get => (User)GetValue(LoggedUserProperty);
            set { SetValue(LoggedUserProperty, value); }
        }

        public static readonly DependencyProperty LoggedUserProperty = DependencyProperty.Register("LoggedUser", typeof(User), typeof(MainHeaderUC));

        public void UpdateHourAndDate()
        {
            currentTime.Content = DateTime.Now.ToString("HH:mm");
            currentDate.Content = DateTime.Now.ToString("dd/MM/yy");
        }

    }
}
