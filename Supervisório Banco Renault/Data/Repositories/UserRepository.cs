using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;

namespace Supervisório_Banco_Renault.Data.Repositories
{
    public interface IUserRepository
    {
        Task<ObservableCollection<User>> GetAllActiveUsers();
        Task<User?> GetUserByRFID(string tagRFID);
        Task<bool> AddUser(User user);
        Task<bool> RemoveUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> VerifyData(User user);
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
            return new ObservableCollection<User>(await _context.Users.Where(u => u.AccessLevel != Models.Enums.AccessLevel.SuperUser).ToListAsync());
        }

        public async Task<User?> GetUserByRFID(string tagRFID)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.TagRFID == tagRFID);
        }

        public async Task<bool> AddUser(User user)
        {
            await VerifyData(user);
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUser(User user)
        {
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
            await VerifyData(user);
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> VerifyData(User user)
        {
            User? userByRFID = await GetUserByRFID(user.TagRFID);

            if (userByRFID != null && userByRFID.Id != user.Id)
                throw new Exception("Já existe um usuário com essa tag RFID.");
            if (user.TagRFID == string.Empty)
                throw new Exception("Tag RFID não pode ser nula.");
            if (user.Name == string.Empty)
                throw new Exception("Nome não pode ser nulo.");
            if(user.AccessLevel == Models.Enums.AccessLevel.None)
                throw new Exception("O nível de acesso precisa ser definido.");

            return true;
        }

    }
}
