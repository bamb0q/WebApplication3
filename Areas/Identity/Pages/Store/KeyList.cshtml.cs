using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class KeyListModel : PageModel
    {
        private readonly IKeysService keysService;
        private readonly UserManager<ApplicationUser> userManager;
        public KeyListModel(IKeysService keysService, UserManager<ApplicationUser> userManager)
        {
            this.keysService = keysService;
            this.userManager = userManager;
        }

        public IList<PageKeyViewModel> PageKeys { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            var user = this.userManager.GetUserName(User);
            ApplicationUser loggedUser = await this.userManager.FindByNameAsync(user);
            var pageKeys =  await this.keysService.GetUserPageKeysAsync(loggedUser.Id);
            PageKeys = pageKeys.Select(x => new PageKeyViewModel
            {
                Id = x.Id,
                Name = x.Name,
                UserId = x.UserId
            }).ToList();
            return Page();
        }
    }
}
