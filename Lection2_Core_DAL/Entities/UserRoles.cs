using System.ComponentModel.DataAnnotations.Schema;

namespace Lection2_Core_DAL.Entities
{
    public class UserRoles
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
