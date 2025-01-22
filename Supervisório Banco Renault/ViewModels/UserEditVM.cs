using Supervisório_Banco_Renault.Core;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class UserEditVM : BaseVM
    {
        private User _user = User.GetNullUser();

        private bool isUpdate = false;

        private readonly IUserRepository _userRepository;

        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public UserEditVM(IUserRepository userRepository, User user)
        {
            _userRepository = userRepository;
            User = user;
            isUpdate = true;
        }

        public UserEditVM(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
            User = User.NewUser(string.Empty, string.Empty, Models.Enums.AccessLevel.None);
        }

        public void AddOrUpdateUser()
        { 
            try
            {
                if (isUpdate)
                {
                    _userRepository.UpdateUser(User);

                }
                else
                {
                    _userRepository.AddUser(User);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
