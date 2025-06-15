using JobHunter.DTOs;
using JobHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    public class AccountController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                // Update user properties
                currentUser.FirstName = model.FirstName;
                currentUser.LastName = model.LastName;
                currentUser.PhoneNumber = model.PhoneNumber;

                // Handle email change (requires special handling in Identity)
                if (currentUser.Email != model.Email)
                {
                    var emailResult = await _userManager.SetEmailAsync(currentUser, model.Email);
                    if (!emailResult.Succeeded)
                    {
                        foreach (var error in emailResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        TempData["Error"] = "Failed to update email address.";
                        return View("Index", currentUser);
                    }

                    // Update username if you're using email as username
                    await _userManager.SetUserNameAsync(currentUser, model.Email);

                    // Replace the invalid SetEmailConfirmedAsync call
                    // Generate an email confirmation token and confirm the email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
                    var emailConfirmResult = await _userManager.ConfirmEmailAsync(currentUser, token);
                    if (!emailConfirmResult.Succeeded)
                    {
                        TempData["Error"] = "Email updated but confirmation failed.";
                        return RedirectToAction("Index");
                    }
                }

                var result = await _userManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Profile updated successfully!";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["Error"] = "Failed to update profile.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating your profile.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                var result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    // Sign the user in again to refresh their authentication cookie
                    await _signInManager.RefreshSignInAsync(currentUser);

                    TempData["Success"] = "Password changed successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while changing your password.");
            }

            return View(model);
        }
    }
}
