using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class WindowBaseVM : BaseVM
    {
        // Menu list, the itens in the list has the MenuItemModel class and will be showed on side menu

        protected ObservableCollection<MenuItemModel> _menuItems = [];

        public ObservableCollection<MenuItemModel> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }

        // Logged user for the screen
        protected User _loggedUser = User.GetNullUser();

        public User LoggedUser
        {
            get => _loggedUser;
            set
            {
                _loggedUser = value;
                OnPropertyChanged(nameof(LoggedUser));
            }
        }

        // Name of the current page
        protected string? _currentPage;

        public string? CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public event EventHandler<string>? PageChanged;

        public void ChangePage(string page)
        {
            PageChanged?.Invoke(this, page);
        }

    }
}
