using Microsoft.AspNetCore.Identity;

namespace Ecommerce.ViewModels
{
    public class UserWithRoleVM
    {
        public string Id { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public string RoleName { get; set; } = string.Empty;
        public IEnumerable<IdentityRole> IdentityRoles { get; set; } = [];
    }
}
