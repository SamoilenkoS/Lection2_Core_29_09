using Lection2_Core_DAL.Entities;

namespace Lection2_Core_DAL
{
    public class RolesHelper
    {
        private Dictionary<string, Guid> _roles;

        public RolesHelper()
        {
            _roles = new Dictionary<string, Guid>()
            {
                { RolesList.Admin, Guid.Parse("9d25f40b-88de-4e7f-b76b-74f87f26f654") },
                { RolesList.User, Guid.Parse("a2a9a6ba-cc43-4251-bfc9-34791264a417") }
            };
        }

        public Guid this[string roleTitle] => _roles[roleTitle];
    }
}
