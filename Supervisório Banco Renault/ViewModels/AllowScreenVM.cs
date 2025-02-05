using Supervisório_Banco_Renault.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class AllowScreenVM : BaseVM
    {
        private UserRepository _userRepository;

        private string _userTag;

        public string UserTag
        {
            get => _userTag;
            set
            {
                _userTag = value;
                OnPropertyChanged(nameof(UserTag));
            }
        }

        public AllowScreenVM(IUserRepository userRepository)
        {
            _userRepository = (UserRepository)userRepository;
        }
    }
}
