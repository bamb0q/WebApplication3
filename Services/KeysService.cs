using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Data.DTO;
using WebApplication3.Data.Models;

namespace WebApplication3.Services
{
    public class KeysService : IKeysService
    {
        private readonly ApplicationDbContext context;
        private readonly IPasswordEndcodingHelper passwordEndcodingHelper;
        public KeysService(ApplicationDbContext context, IPasswordEndcodingHelper passwordEndcodingHelper)
        {
            this.context = context;
            this.passwordEndcodingHelper = passwordEndcodingHelper;
        }

        public async Task<List<PageKey>> GetUserPageKeysAsync(string userId)
        {
            return await this.context.PageKeys.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task AddUserPageKeyAsync(PageKeyAddModel pageKeyAdd)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(x => x.Id == pageKeyAdd.UserId);
            byte[] IV;
            var encryptedPassword = this.passwordEndcodingHelper.EncryptString_Aes(pageKeyAdd.Password, user.PasswordHash, out IV);
            var pageKeyToAdd = new PageKey
            {
                Name = pageKeyAdd.Name,
                UserId = user.Id,
                EncryptedPassword = encryptedPassword,
                IV = Convert.ToBase64String(IV),
            };
            context.PageKeys.Add(pageKeyToAdd);
            await context.SaveChangesAsync();
        }

        public async Task<PageKey> GetUserPageKeyAsync(string userId, Guid pageKeyId)
        {
            return await this.context.PageKeys.FirstOrDefaultAsync(x => x.Id == pageKeyId && x.UserId == userId);
        }

        public async Task<PageKey> GetDecryptedPageKeyPassword(string userId, Guid pageKeyId)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var pageKey = await this.context.PageKeys.FirstOrDefaultAsync(x => x.Id == pageKeyId && x.UserId == userId);
            pageKey.EncryptedPassword = this.passwordEndcodingHelper.DecryptString_Aes(pageKey.EncryptedPassword, user.PasswordHash, Convert.FromBase64String(pageKey.IV));
            return pageKey;
        }
    }
}
