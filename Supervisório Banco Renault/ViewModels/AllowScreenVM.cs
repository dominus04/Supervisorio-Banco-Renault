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
    public class AllowScreenVM(IUserRepository userRepository) : BaseVM
    {

        public bool IsAllowed { get; set; } = false;
        public Visibility QuestionVisibility { get; set; } = Visibility.Visible;

        private string? _message;

        public string? Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string? _userTag = "";

        public string? UserTag
        {
            get { return _userTag; }
            set
            {
                _userTag = value;
                OnPropertyChanged(nameof(UserTag));
            }
        }

        public async Task TryAllow()
        {
            User? user = await userRepository.GetUserByRFID(UserTag);
            if(user != null && user.AccessLevel >= Models.Enums.AccessLevel.Lideranca)
            {
                IsAllowed = true;
            }
            else 
            {
                MessageBox.Show("Usuário não autorizado");
                IsAllowed = false;
            }

        }

    }
}
