using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;
        private readonly IEmailSender _emailSender;

        // IdentityUser (ApplicationUser) => Service layer => UserManager
        // IdentityUser (ApplicationUser) => Repo layer => UserStore
        // IdentityRole (ApplicationRole) => Service layer => RoleManager
        // IdentityRole (ApplicationRole) => Repo layer => RoleStore
        // Another Services => SignInManger

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManger, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManger = signInManger;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManger.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegiterVM regiterVM)
        {
            if (!ModelState.IsValid)
                return View(regiterVM);

            var user = new ApplicationUser()
            {
                FirstName = regiterVM.FirstName,
                LastName = regiterVM.LastName,
                Email = regiterVM.Email,
                UserName = regiterVM.UserName,
                Address = regiterVM.Address
            };

            var result = await _userManager.CreateAsync(user, regiterVM.Password);
            // Why generate Errors?
            // Password (Uppercase-lowercase-digits-special char-min lenght = 6) < = by default
            // dublicate username < = by default
            // dublicate email, phone number

            if(!result.Succeeded)
            {
                // Print Errors
                foreach (var item in result.Errors)
                    ModelState.AddModelError(string.Empty, item.Description);

                return View(regiterVM);
            }

            // Send Email Confirmation
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); // By Default => token valid to 24h
            //var link = Url.Action("Confirm", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);
            //await _emailSender.SendEmailAsync(user.Email, "Conirmation Your account",
            //    $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");
            await SendConfirmationMailAsync(user);

            TempData["success-notification"] = "Create Account Successfully, Please Check Your email to verfiy";

            //await _signInManger.SignInAsync(user, false); // Automatic login

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Confirm(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                // Print Errors
                //foreach (var item in result.Errors)
                    //ModelState.AddModelError(string.Empty, item.Description);

                TempData["error-notification"] = String.Join(", ", result.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(Login));
            }

            TempData["success-notification"] = "Active Account Successfully";

            //await _signInManger.SignInAsync(user, false);
            //return RedirectToAction("Index", "Home", new { area = "Customer" });
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);

            if(user is null)
            {
                ModelState.AddModelError("UserNameOrEmail", "Email Or UserName Incorrect");
                ModelState.AddModelError("Password", "Password Incorrect");

                return View(loginVM);
            }

            var result = await _signInManger.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("UserNameOrEmail", "Email Or UserName Incorrect");
                ModelState.AddModelError("Password", "Password Incorrect");

                if(result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too many attempts, please try again later");
                }

                return View(loginVM);
            }

            TempData["success-notification"] = $"Welcome Back {user.FirstName} {user.LastName}";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);

            if(user is not null)
            {
                await SendConfirmationMailAsync(user);
            }

            TempData["success-notification"] = "Send Confirmation Mail, Check You Mail";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);

            if (user is not null)
            {
                await SendOTPToMailAsync(user);
            }

            TempData["success-notification"] = "Send OTP Number To Your Mail, Check Your Mail";
            TempData["userId"] = user.Id;
            return RedirectToAction("ValidateOTP");
        }

        [HttpGet]
        public IActionResult ValidateOTP()
        {
            if (TempData.Peek("userId") is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
                return View(validateOTPVM);

            var userId = TempData.Peek("userId");

            var otpInDB = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == userId.ToString() && !e.IsUsed))
                .OrderByDescending(e=>e.CreateAt).FirstOrDefault();

            if(otpInDB.OTP != validateOTPVM.OTP)
            {
                TempData["error-notification"] = "In Valid Or Expired OTP";
                return View();
            }

            return RedirectToAction("ChangePassword");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (TempData.Peek("userId") is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
                return View(changePasswordVM);

            var user = await _userManager.FindByIdAsync(TempData["userId"].ToString());

            if (user is null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, changePasswordVM.Password);

            if(!result.Succeeded)
            {
                TempData["error-notification"] = String.Join(", ", result.Errors.Select(e => e.Description));
                TempData["userId"] = user.Id;
                return View();
            }

            TempData["success-notification"] = "Change Password Successfully";
            return RedirectToAction(nameof(Login));
        }

        private async Task<bool> SendConfirmationMailAsync(ApplicationUser user)
        {
            try
            {
                // Send Email Confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); // By Default => token valid to 24h
                var link = Url.Action("Confirm", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Conirmation Your account",
                    $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendOTPToMailAsync(ApplicationUser user)
        {
            try
            {
                // Send Email Confirmation
                var otp = new Random().Next(100000, 999999);
                //new Guid().ToString().Substring(0, 5);

                await _emailSender.SendEmailAsync(user.Email, $"Reset Password Your Account - {DateTime.Now}",
                    $"<h1>OTP: <b>{otp}</b>. Don't share it.<h1>");

                await _applicationUserOTPRepository.CreateAsync(new ApplicationUserOTP()
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id
                });
                await _applicationUserOTPRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
