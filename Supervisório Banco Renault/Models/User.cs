using Supervisório_Banco_Renault.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public User(string name, string tagRFID, AccessLevel accessLevel)
        {
            Id = Guid.NewGuid();
            Name = name;
            TagRFID = tagRFID;
            AccessLevel = accessLevel;
        }

    }
}
