using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_ROLE)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var users = _userManager.Users.AsQueryable();

            // Filter
            if (query is not null)
            {
                users = users.Where(e => e.NormalizedUserName!.Contains(query.Trim().ToUpper()));
                ViewBag.Query = query;
                //ViewData["Query"] = query;
            }

            // Pagination
            double totalPages = Math.Ceiling(users.Count() / 3.0);
            var userList = users.Skip((page - 1) * 3).Take(3).ToList();

            // Mapping
            Dictionary<ApplicationUser, string> userRoles = new();
            foreach (var item in userList)
            {
                userRoles.Add(item, (await _userManager.GetRolesAsync(item)).FirstOrDefault()!);
            }

            return View(new ApplicationUserWithFilterVM()
            {
                UserRoles = userRoles.ToDictionary(),
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }

        public async Task<IActionResult> UpdateRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE)) return NotFound();

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;

            return View(new UserWithRoleVM()
            {
                ApplicationUser = user,
                RoleName = userRole,
                IdentityRoles = _roleManager.Roles.AsEnumerable()
            });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRole(UserWithRoleVM userWithRoleVM)
        {
            var user = await _userManager.FindByIdAsync(userWithRoleVM.Id);
            if (user is null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE)) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            await _userManager.AddToRoleAsync(user, userWithRoleVM.RoleName);

            TempData["success-notification"] = $"Update Role To user: {user.UserName} Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;

            if (!user.LockoutEnabled)
            {
                user.LockoutEnd = DateTime.Now.AddDays(14);
                TempData["warning-notification"] = $"Lock user: {user.UserName} Successfully";
            }
            else
            {
                user.LockoutEnd = null;
                TempData["warning-notification"] = $"Un Lock user: {user.UserName} Successfully";
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
