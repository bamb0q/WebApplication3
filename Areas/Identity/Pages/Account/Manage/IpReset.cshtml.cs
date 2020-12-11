using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication3.Data.Models;
using WebApplication3.Services;

namespace WebApplication3.Areas.Identity.Pages.Account.Manage
{
    public class IpResetModel : PageModel
    {
        private readonly ILoginAttemptsService _loginAttemptsService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IpResetModel(ILoginAttemptsService loginAttemptsService, UserManager<ApplicationUser> userManager)
        {
            _loginAttemptsService = loginAttemptsService;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<SelectListItem> IPs { get; set; }

        [TempData]
        public string IpResetStatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "IP")]
            public string IP { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var ips = await _loginAttemptsService.GetUserBlockedIps(userId);
            IPs = ips.Select(x => new SelectListItem 
            { 
                Text = x,
                Value = x
            });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _loginAttemptsService.UnlockIP(Input.IP);
            }
            var ips = new List<SelectListItem>();
            ips.AddRange(ips.Where(x => x.Value != Input.IP));
            IPs = ips;
            IpResetStatusMessage = "Your IP has been unblocked.";
            return Page();
        }
    }
}
