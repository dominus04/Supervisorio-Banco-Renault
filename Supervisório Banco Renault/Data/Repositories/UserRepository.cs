using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Data.Repositories
{
    public interface IUserRepository
    {
        bool AddUser(User user);
        ObservableCollection<User> GetAllActiveUsers();
        User? GetUserById(Guid id);
        bool DeleteUserById(Guid id);
        bool UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {

        public readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool AddUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteUserById(Guid id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public ObservableCollection<User> GetAllActiveUsers()
        {
            return new ObservableCollection<User>(_context.Users.ToList());
        }

        public User? GetUserById(Guid id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public bool UpdateUser(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }
    }
}
