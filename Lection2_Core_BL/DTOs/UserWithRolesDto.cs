using Lection2_Core_DAL.Entities;

namespace Lection2_Core_BL.DTOs
{
    public class UserWithRolesDto
    {
        public User User { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}
