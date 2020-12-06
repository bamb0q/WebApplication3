using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data.Models;

namespace WebApplication3.Data.DTO
{
    public class LoginAttemptAddModel
    {
        public ApplicationUser User { get; set; }

        public string IP { get; set; }

        public DateTime LoginTime { get; set; }

        public bool LoginResult { get; set; }
    }
}
