namespace Lection2_Core_DAL.RolesHelper
{
    public interface IRolesHelper
    {
        Guid this[string roleTitle] { get; }
    }
}