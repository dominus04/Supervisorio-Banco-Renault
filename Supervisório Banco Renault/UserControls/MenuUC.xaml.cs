using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.UserControls
{
    /// <summary>
    /// Interação lógica para MenuUC.xam
    /// </summary>
    public partial class MenuUC : UserControl, INotifyPropertyChanged
    {
        public MenuUC()
        {
            InitializeComponent();
        }

        // The event that controls the change of the view on the mainview
        public event Action<string>? PageSelected;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }

        // Filtered menu items

        private ObservableCollection<MenuItemModel>? _filteredMenu;

        public ObservableCollection<MenuItemModel>? FilteredMenu
        {
            get => _filteredMenu;
            set
            {
                _filteredMenu = value;
                OnPropertyChanged(nameof(FilteredMenu));
            }
        }


        // Gets the list of menu items from view
        public ObservableCollection<MenuItemModel> MenuItems
        {
            get => (ObservableCollection<MenuItemModel>)GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register("MenuItems", typeof(ObservableCollection<MenuItemModel>), typeof(MenuUC), new PropertyMetadata(null, OnDependencyPropertyChanged));

        // Receive the logged User
        public User LoggedUser
        {
            get => (User)GetValue(LoggedUserProperty);
            set => SetValue(LoggedUserProperty, value);
        }

        public static readonly DependencyProperty LoggedUserProperty = DependencyProperty.Register("LoggedUser", typeof(User), typeof(MenuUC), new PropertyMetadata(null, OnDependencyPropertyChanged));

        public static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuUC menuUc)
            {
                menuUc.RefreshFilteredMenu();
            }
        }

        public void RefreshFilteredMenu()
        {
            if (LoggedUser == null)
            {
                return;
            }
            else if (LoggedUser.AccessLevel == Models.Enums.AccessLevel.None)
            {
                FilteredMenu = new ObservableCollection<MenuItemModel>(MenuItems.Where(menuItem => menuItem.LoginLevel == Models.Enums.AccessLevel.None));
            }
            else
            {
                FilteredMenu = new ObservableCollection<MenuItemModel>(MenuItems.Where(menuItem => menuItem.LoginLevel <= LoggedUser.AccessLevel && menuItem.LoginLevel > Models.Enums.AccessLevel.None));
            }
        }

        // Invoke an event on the view to change the screen
        private void ChangeView(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = MenuList.SelectedItem as MenuItemModel;
            if (selectedItem != null)
            {
                PageSelected?.Invoke(selectedItem.ViewName);
            }
        }

    }
}
