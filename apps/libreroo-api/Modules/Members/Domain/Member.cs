namespace Libreroo.Api.Modules.Members.Domain;

public class Member
{
    private Member()
    {
    }

    public Member(string fullName)
    {
        FullName = fullName;
    }

    public int Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;
}
