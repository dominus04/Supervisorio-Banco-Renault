using BCrypt.Net;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models.Enums;
using System.Security;
using System.Windows;


namespace Supervisório_Banco_Renault.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? TagRFID { get; set; }
        public string? HashedPassword { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User()
        {
            
        }

        public static User GetNullUser()
        {
            return new User() {Id = Guid.Empty,  Name = string.Empty, AccessLevel = AccessLevel.None, TagRFID = string.Empty};
        }

        public static User NewUser(string name, string tagRFID, AccessLevel accessLevel, string password)
        {
            return new User() { Id = new Guid(), Name = name, TagRFID = tagRFID, AccessLevel = accessLevel, HashedPassword = BCrypt.Net.BCrypt.HashPassword(password)};
        }

        public bool TryLogin(string password)
        {   
            if (BCrypt.Net.BCrypt.Verify(password, HashedPassword))
            {
                return true;
            }
            return false;
        }
    }
}
