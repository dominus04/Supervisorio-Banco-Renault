using Supervisório_Banco_Renault.Core;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class UsersManagerVM : BaseVM
    {

        public UserRepository _userRepository;

        public UsersManagerVM(IUserRepository userRepository) 
        {
            _userRepository = (UserRepository)userRepository;

            LoadUsers();
        }

        public void AddUser()
        {
            UserEditVM vm = new (_userRepository);
            UserEdit userEdit = new();
            userEdit.DataContext = vm;
            userEdit.ShowDialog();
            LoadUsers();
        }

        public void UpdateUser()
        {
            if (SelectedUser != null)
            {
                UserEditVM vm = new (_userRepository, SelectedUser);
                UserEdit userEdit = new();
                userEdit.DataContext = vm;
                userEdit.ShowDialog();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Selecione um usuário para editar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        //Observable collection with all active users
        public ObservableCollection<User> _users = [];

        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        //Selected User in the Users View
        public User? _selectedUser;

        public User? SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

    }
}
