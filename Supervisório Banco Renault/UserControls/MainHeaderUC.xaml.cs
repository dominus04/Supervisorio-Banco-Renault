using Supervisório_Banco_Renault.Models;
using System.Windows;
using System.Windows.Controls;

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
            set => SetValue(HeaderTitleProperty, value);
        }

        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register("HeaderTitle", typeof(string), typeof(MainHeaderUC));

        public User LoggedUser
        {
            get => (User)GetValue(LoggedUserProperty);
            set => SetValue(LoggedUserProperty, value);
        }

        public static readonly DependencyProperty LoggedUserProperty = DependencyProperty.Register("LoggedUser", typeof(User), typeof(MainHeaderUC));

        public void UpdateHourAndDate()
        {
            currentTime.Content = DateTime.Now.ToString("HH:mm");
            currentDate.Content = DateTime.Now.ToString("dd/MM/yy");
        }

    }
}
