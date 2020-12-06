﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Data.Models
{
    public class LoginAttempt
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string IP { get; set; }

        public DateTime LoginTime { get; set; }

        public bool LoginResult { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("IP")]
        public virtual IPLockout IPLockout { get; set; }
    }
}
