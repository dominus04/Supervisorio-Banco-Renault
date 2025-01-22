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

        public static User NewUser(string name, string tagRFID, AccessLevel accessLevel)
        {
            return new User() { Id = new Guid(), Name = name, TagRFID = tagRFID, AccessLevel = accessLevel};
        }

    }
}
