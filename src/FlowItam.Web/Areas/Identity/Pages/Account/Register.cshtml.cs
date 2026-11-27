using System.ComponentModel.DataAnnotations;
using FlowItam.Infrastructure.Identity;
using FlowItam.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowItam.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class RegisterModel(
    OrganizationRegistrationService registrationService,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; private set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ReturnUrl = returnUrl;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email
        };

        var result = await registrationService.CreateOrganizationAccountAsync(
            user,
            Input.Password,
            Input.OrganizationName,
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        var redirectUrl = Url.IsLocalUrl(returnUrl)
            ? returnUrl!
            : Url.Content("~/");

        return LocalRedirect(redirectUrl);
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Organization name")]
        public string OrganizationName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 12)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}