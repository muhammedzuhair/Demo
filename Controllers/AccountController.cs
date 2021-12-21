using Demo.BL.Helper;
using Demo.BL.Models;
using Demo.DAL.Extend;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {


        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        #region Registration (Sign up)


        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    var user = new ApplicationUser() { 
                    
                        UserName = model.Email,
                        Email = model.Email ,
                        IsAgree = model.IsAgree
                    };
                    user.PhotoName = FileUploader.UploadFile("/wwwroot/Files/Imgs",model.Photo);
                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }


                }

                return View(model);

            }
            catch (Exception)
            {
                return View(model);
            }



            return View();
        }

        #endregion


        #region Login (Sign In)

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    var user = await userManager.FindByEmailAsync(model.Email);

                    var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid UserName or Password");
                    }

                }

                return View(model);

            }
            catch (Exception)
            {
                return View(model);
            }



            return View();
        }

        #endregion


        #region LogOff (Sign Out)

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
             

        #endregion


        #region Forget Password

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    // Get User By Email
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {

                        // Generate Token
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);

                        // Generate Reset Link
                        var passwordResetLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, Request.Scheme);

                        MailSender.SendMail(new MailVM() { Mail = model.Email , Title = "Reset Password - Route Academy"  , Message = passwordResetLink });

                        return RedirectToAction("ConfirmForgetPassword");
                    }

                }

                return View(model);

            }
            catch (Exception)
            {
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult ConfirmForgetPassword()
        {
            return View();
        }



        #endregion


        #region Reset Password


        [HttpGet]
        public IActionResult ResetPassword(string Email,string Token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("ConfirmResetPassword");
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(model);
                    }

                    return RedirectToAction("ConfirmResetPassword");

                }

                return View(model);

            }
            catch (Exception)
            {
                return View(model);
            }



            return View();
        }

        [HttpGet]
        public IActionResult ConfirmResetPassword()
        {
            return View();
        }






        #endregion
















    }
}
