using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data.DTO;
using WebApplication3.Data.Models;

namespace WebApplication3.Services
{
    public interface IKeysService
    {
        public Task<List<PageKey>> GetUserPageKeysAsync(string userId);

        public Task AddUserPageKeyAsync(PageKeyAddModel pageKeyAdd);

        public Task<PageKey> GetUserPageKeyAsync(string userId, Guid pageKeyId);

        public Task<PageKey> GetDecryptedPageKeyPassword(string userId, Guid pageKeyId);
    }
}
