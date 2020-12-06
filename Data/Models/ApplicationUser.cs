using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool HashType { get; set; }

        public string PasswordSalt { get; set; }

        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; }

        public virtual ICollection<PageKey> PageKeys { get; set; }
    }
}
