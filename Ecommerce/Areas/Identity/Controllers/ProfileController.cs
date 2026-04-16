using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            // Automapper, mappster

            var result = user.Adapt<ApplicationUserVM>();

            //var result = new ApplicationUserVM()
            //{
            //    Address = user.Address,
            //    Email = user.Email,
            //    FirstName = user.FName,
            //    LastName = user.LName,
            //    PhoneNumber = user.PhoneNumber,
            //    Id = user.Id
            //};

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
                return View("Index", applicationUserVM);

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            user.Email = applicationUserVM.Email;
            user.Address = applicationUserVM.Address;
            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Update Profile Successully";
            return RedirectToAction("Index");
        }

        // TODO: Change Password
    }
}
