﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Data.Models
{
    public class PageKey
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string EncryptedPassword { get; set; }

        public string IV { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
