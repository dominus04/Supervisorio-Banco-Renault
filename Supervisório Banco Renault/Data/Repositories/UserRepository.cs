using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;

namespace Supervisório_Banco_Renault.Data.Repositories
{
    public interface IUserRepository
    {
        Task<ObservableCollection<User>> GetAllActiveUsers();
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByRFID(string tagRFID);
        Task<bool> AddUser(User user);
        Task<bool> DeleteUserById(Guid id);
        Task<bool> UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {

        public readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<User>> GetAllActiveUsers()
        {
            return new ObservableCollection<User>(await _context.Users.ToListAsync());
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetUserByRFID(string tagRFID)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.TagRFID == tagRFID);
        }

        public async Task<bool> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
