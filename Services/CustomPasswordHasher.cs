using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Data.Models;

namespace WebApplication3.Services
{

    public class CustomPasswordHasher : IPasswordHasher<ApplicationUser>
    {
        private readonly IConfiguration config;
        private readonly string pepper;
        public CustomPasswordHasher(IConfiguration config)
        {
            this.config = config;
            this.pepper = this.config["Pepper"];
        }

        public string HashPassword(ApplicationUser user, string password)
        {
            user.PasswordSalt = getSalt();
            return user.HashType ? HashPasswordAsSha(password, user.PasswordSalt) : HashPasswordAsHmac(password, user.PasswordSalt);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            return user.HashType ? VerifySha(hashedPassword, providedPassword, user) : VerifyHmac(hashedPassword, providedPassword, user);
        }

        private string HashPasswordAsSha(string password, string salt)
        {
            using (var sha512 = SHA512.Create())
            {
                var hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(salt + password + this.pepper));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string HashPasswordAsHmac(string password, string salt)
        {
            using (var hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(this.pepper)))
            {
                var hashedBytes = hmac512.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private PasswordVerificationResult VerifySha(string hashedPassword, string providedPassword, ApplicationUser user)
        {
            return hashedPassword == HashPasswordAsSha(providedPassword, user.PasswordSalt) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private PasswordVerificationResult VerifyHmac(string hashedPassword, string providedPassword, ApplicationUser user)
        {
            return hashedPassword == HashPasswordAsHmac(providedPassword, user.PasswordSalt) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private static string getSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
