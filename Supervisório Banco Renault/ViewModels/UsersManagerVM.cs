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

        public void AddOrUpdateUser(Type t, bool isUpdate)
        {
            Application.Current.Windows.OfType<UserEdit>().FirstOrDefault()?.Close();
            UserEditVM vm = new(_userRepository);

            if (isUpdate && SelectedUser != null) 
            {
                vm = new(_userRepository, SelectedUser);
            }
            else if (isUpdate && SelectedUser == null)
            {
                MessageBox.Show("Selecione um usuário para editar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserEdit userEdit = new();
            userEdit.DataContext = vm;

            userEdit.Top = 140;

            //if (t == typeof(OP20_MainWindow))
            //    userEdit.Left = ((1920 - userEdit.Width) / 2) - 1920;
            //else
            //    userEdit.Left = (1920 - userEdit.Width) / 2;

            userEdit.Show();

            userEdit.Closed += (sender, e) =>
            {
                LoadUsers();
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
                OnPropertyChanged(nameof(Users));
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
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

    }
}
