using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Services
{
    public interface IPasswordEndcodingHelper
    {
        public string EncryptString_Aes(string plainText, string userPasswordHash, out byte[] IV);
        public string DecryptString_Aes(string text, string userPasswordHash, byte[] IV);
    }
}
