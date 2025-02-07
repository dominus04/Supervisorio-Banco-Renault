using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class UsersManagerVM : BaseVM
    {

        #region Properties

        //Observable collection with all active users
        public ObservableCollection<User> _users = [];

        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        //Selected User in the Users View
        public User? _selectedUser;

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public UserRepository _userRepository;

        #endregion

        public UsersManagerVM(IUserRepository userRepository, IServiceProvider serviceProvider)
        {
            _userRepository = (UserRepository)userRepository;
            _serviceProvider = serviceProvider;

            LoadUsers();
        }

        public void AddOrUpdateUser(Type t, bool isUpdate)
        {
            Application.Current.Windows.OfType<UserEdit>().FirstOrDefault()?.Close();
            UserEditVM vm = new(_userRepository);
            WindowBaseVM mainVM = (WindowBaseVM)_serviceProvider.GetService(typeof(OP20_MainWindowVM))!;


            if (isUpdate && SelectedUser != null)
            {
                vm = new(_userRepository, SelectedUser);
            }
            else if (isUpdate && SelectedUser == null)
            {
                MessageBox.Show("Selecione um usuário para editar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserEdit userEdit = new()
            {
                DataContext = vm,

                Top = 140
            };

            userEdit.Left = (1920 - userEdit.Width) / 2;

            if (t == typeof(OP10_MainWindow))
            {
                userEdit.Left += 1920;
                mainVM = (WindowBaseVM)_serviceProvider.GetService(typeof(OP10_MainWindowVM))!;
            }

            mainVM.ScreenControl = false;

            userEdit.Show();

            userEdit.Closed += (sender, e) =>
            {
                LoadUsers();
                mainVM.ScreenControl = true;
            };
        }

        public async void LoadUsers()
        {
            Users = await _userRepository.GetAllActiveUsers();
        }

        public async void RemoveUser()
        {
            if (SelectedUser != null)
            {
                try
                {
                    await _userRepository.RemoveUser(SelectedUser);
                    Users.Remove(SelectedUser);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao remover o usuário: " + ex.ToString(), "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um usuário para remover", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
}
