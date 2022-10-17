using System.Collections;

namespace Lection2_Core_DAL.Entities;

public class RolesList : IEnumerable<string>
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);

    public IEnumerator<string> GetEnumerator()
    {
        yield return Admin;
        yield return User;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
