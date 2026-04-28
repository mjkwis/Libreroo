namespace Libreroo.Api.Modules.Catalog.Domain;

public class Author
{
    private Author()
    {
    }

    public Author(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public ICollection<Book> Books { get; private set; } = new List<Book>();
}
