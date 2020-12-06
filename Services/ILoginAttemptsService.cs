using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.Data.DTO;

namespace WebApplication3.Services
{
    public interface ILoginAttemptsService
    {
        public Task SaveFailAttemptAndSetLockoutIfNeeded(LoginAttemptAddModel loginAttempt);

        public Task<bool> IsIpLocked(string iP);

        public Task ResetLoginAttempts(LoginAttemptAddModel loginAttempt);

        public Task<List<string>> GetUserBlockedIps(string userId);

        public Task UnlockIP(string iP);
    }
}
