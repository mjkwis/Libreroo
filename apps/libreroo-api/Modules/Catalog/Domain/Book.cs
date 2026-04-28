using Libreroo.Api.Shared.Application.Errors;

namespace Libreroo.Api.Modules.Catalog.Domain;

public class Book
{
    private Book()
    {
    }

    public Book(string title, int authorId, int availableCopies)
    {
        Title = title;
        AuthorId = authorId;
        AvailableCopies = availableCopies;
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public int AuthorId { get; private set; }

    public int AvailableCopies { get; private set; }

    public Author? Author { get; private set; }

    public void DecreaseAvailableCopies()
    {
        if (AvailableCopies <= 0)
        {
            throw new DomainRuleViolationException("No available copies.");
        }

        AvailableCopies--;
    }

    public void IncreaseAvailableCopies()
    {
        AvailableCopies++;
    }
}
