using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Data.Models;
using WebApplication3.Data.ViewModels;
using WebApplication3.Services;

namespace WebApplication3.Areas.Identity.Pages.Store
{
    [Authorize]
    public class KeyModel : PageModel
    {
        private readonly IKeysService keysService;
        private readonly UserManager<ApplicationUser> userManager;
        public KeyModel(IKeysService keysService, UserManager<ApplicationUser> userManager)
        {
            this.keysService = keysService;
            this.userManager = userManager;
        }

        public PageKeyViewModel PageKey { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            var user = this.userManager.GetUserName(User);
            ApplicationUser loggedUser = await this.userManager.FindByNameAsync(user);
            var pageKey = await this.keysService.GetUserPageKeyAsync(loggedUser.Id, Guid.Parse(id));
            PageKey = new PageKeyViewModel
            {
                Id = pageKey.Id,
                Name = pageKey.Name,
                Password = pageKey.EncryptedPassword,
                UserId = pageKey.UserId
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            var user = this.userManager.GetUserName(User);
            ApplicationUser loggedUser = await this.userManager.FindByNameAsync(user);
            var pageKeyWithDecryptedPassword = await this.keysService.GetDecryptedPageKeyPassword(loggedUser.Id, Guid.Parse(id));
            if (pageKeyWithDecryptedPassword != null)
            {
                PageKey = new PageKeyViewModel
                {
                    Id = pageKeyWithDecryptedPassword.Id,
                    Name = pageKeyWithDecryptedPassword.Name,
                    UserId = pageKeyWithDecryptedPassword.UserId,
                    Password = pageKeyWithDecryptedPassword.EncryptedPassword
                };
            }

            return Page();
        }
    }
}
