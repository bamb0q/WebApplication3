using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Data.Models;
using WebApplication3.Services;

namespace WebApplication3.Areas.Identity.Pages.Store
{
    [Authorize]
    public class CreateKeyModel : PageModel
    {
        private readonly IKeysService keysService;
        private readonly UserManager<ApplicationUser> userManager;
        public CreateKeyModel(IKeysService keysService, UserManager<ApplicationUser> userManager)
        {
            this.keysService = keysService;
            this.userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = this.userManager.GetUserName(User);
                ApplicationUser loggedUser = await this.userManager.FindByNameAsync(user);
                await this.keysService.AddUserPageKeyAsync(new Data.DTO.PageKeyAddModel()
                {
                    Name = Input.Name,
                    Password = Input.Password,
                    UserId = loggedUser.Id
                });


                return RedirectToPage("KeyList");
            }
            return Page();
        }
    }
}
