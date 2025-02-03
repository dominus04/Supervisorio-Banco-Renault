using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class UserEditVM : BaseVM
    {
        private User? _user;

        private bool isUpdate = false;

        private readonly IUserRepository _userRepository;

        public User? User
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

        public async Task<bool> AddOrUpdateUser()
        {

            if (User == null)
                return false;

            try
            {
                if (isUpdate)
                {
                    await _userRepository.UpdateUser(User);

                }
                else
                {
                    await _userRepository.AddUser(User);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
