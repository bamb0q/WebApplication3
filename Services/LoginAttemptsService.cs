using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Data.DTO;
using WebApplication3.Data.Models;

namespace WebApplication3.Services
{
    public class LoginAttemptsService : ILoginAttemptsService
    {
        private readonly ApplicationDbContext context;

        public LoginAttemptsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task UnlockIP(string iP)
        {
            var ipToUnlock = await this.context.IPLockouts.FirstOrDefaultAsync(x => x.IP == iP);
            ipToUnlock.AccessFailedCount = 0;
            ipToUnlock.LockoutEnd = null;
            this.context.Update(ipToUnlock);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserBlockedIps(string userId)
        {
            var loginAttempts = await this.context.UserIPs.Where(x => x.UserId == userId && x.IPLockout.AccessFailedCount >= 4).Select(x => x.IP).ToListAsync();
            return loginAttempts;
        }

        public async Task<bool> IsIpLocked(string iP)
        {
            var ipLockout = await this.context.IPLockouts.FirstOrDefaultAsync(x => x.IP == iP && x.LockoutEnabled == true);
            if (ipLockout?.LockoutEnd != null)
            {
                return DateTimeOffset.Compare(DateTimeOffset.UtcNow, (DateTimeOffset)ipLockout.LockoutEnd) < 0;
            }
            return false;
        }

        public async Task ResetLoginAttempts(LoginAttemptAddModel loginAttempt)
        {
            var ipLockout = this.context.IPLockouts.FirstOrDefault(x => x.IP == loginAttempt.IP);
            if (ipLockout != null)
            {
                ipLockout.AccessFailedCount = 0;
                ipLockout.LockoutEnd = null;
                this.context.IPLockouts.Update(ipLockout);
            }
            
            loginAttempt.User.AccessFailedCount = 0;
            loginAttempt.User.LockoutEnd = null;
            this.context.Users.Update(loginAttempt.User);
            await this.context.SaveChangesAsync();
        }

        public async Task SaveFailAttemptAndSetLockoutIfNeeded(LoginAttemptAddModel loginAttempt)
        {
            var ipLockout = this.context.IPLockouts.FirstOrDefault(x => x.IP == loginAttempt.IP);
            if (ipLockout != null)
            {
                ipLockout.AccessFailedCount += 1;
                if (ipLockout.AccessFailedCount >= 2)
                {
                    ipLockout.LockoutEnabled = true;
                    ipLockout.LockoutEnd = ipLockout.AccessFailedCount == 2 ? DateTimeOffset.UtcNow.AddSeconds(5) : ipLockout.AccessFailedCount == 3 ? DateTimeOffset.UtcNow.AddSeconds(10) : ipLockout.AccessFailedCount >= 4 ? DateTimeOffset.UtcNow.AddYears(1000) : DateTimeOffset.UtcNow;
                }
                this.context.Update(ipLockout);
            }
            else
            {
                ipLockout = new IPLockout
                {
                    IP = loginAttempt.IP,
                    AccessFailedCount = 1,
                    LockoutEnabled = true,
                    LockoutEnd = null,
                };
                this.context.Add(ipLockout);
            }
            loginAttempt.User.AccessFailedCount += 1;
            if (loginAttempt.User.AccessFailedCount >= 2)
            {
                loginAttempt.User.LockoutEnabled = true;
                loginAttempt.User.LockoutEnd = loginAttempt.User.AccessFailedCount == 2 ? DateTimeOffset.UtcNow.AddSeconds(5) : loginAttempt.User.AccessFailedCount == 3 ? DateTimeOffset.UtcNow.AddSeconds(10) : loginAttempt.User.AccessFailedCount >= 4 ? DateTimeOffset.UtcNow.AddMinutes(2) : DateTimeOffset.UtcNow;
            }
            this.context.Update(loginAttempt.User);
            await this.context.SaveChangesAsync();
        }
    }
}
