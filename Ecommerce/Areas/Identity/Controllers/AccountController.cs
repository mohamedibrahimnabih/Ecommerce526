using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace Ecommerce.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly IEmailSender _emailSender;

        // IdentityUser (ApplicationUser) => Service layer => UserManager
        // IdentityUser (ApplicationUser) => Repo layer => UserStore
        // IdentityRole (ApplicationRole) => Service layer => RoleManager
        // IdentityRole (ApplicationRole) => Repo layer => RoleStore
        // Another Services => SignInManger

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManger = signInManger;
            _emailSender = emailSender;
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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); // By Default => token valid to 24h
            var link = Url.Action("Confirm", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Conirmation Your account",
                $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

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
    }
}
