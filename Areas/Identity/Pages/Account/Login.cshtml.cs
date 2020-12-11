using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication3.Data.Models;
using WebApplication3.Services;
using WebApplication3.Data.DTO;

namespace WebApplication3.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ILoginAttemptsService _loginAttemptsService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, 
            ILoginAttemptsService loginAttemptsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _loginAttemptsService = loginAttemptsService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {
                var iP = HttpContext.Connection.RemoteIpAddress;
                if (await _loginAttemptsService.IsIpLocked(iP.ToString()))
                {
                    ModelState.AddModelError(string.Empty, "IP locked out.");
                    return RedirectToPage("./Lockout");
                }
                else 
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var user = await _userManager.FindByNameAsync(Input.Email);
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        var loginAttempt = new LoginAttemptAddModel
                        {
                            IP = iP.ToString(),
                            LoginResult = true,
                            User = user
                        };
                        await _loginAttemptsService.ResetLoginAttempts(loginAttempt);
                        return LocalRedirect(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        ModelState.AddModelError(string.Empty, "User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    if (user == null) 
                    {
                        ModelState.AddModelError(string.Empty, "No user.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        var loginAttempt = new LoginAttemptAddModel
                        {
                            IP = iP.ToString(),
                            LoginResult = false,
                            User = user
                        };
                        await _loginAttemptsService.SaveFailAttemptAndSetLockoutIfNeeded(loginAttempt);

                        return Page();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
