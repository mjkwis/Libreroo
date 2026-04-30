using Libreroo.Api.Modules.Access.Domain;

namespace Libreroo.Domain.Tests.Access;

public class AccessUserTests
{
    [Fact]
    public void HasRole_WhenAssignmentMissing_ReturnsFalse()
    {
        var user = AccessUser.Create("sub-123", "member@demo.local", "Member User");

        Assert.False(user.HasRole(AccessRole.Member));
    }
}
