namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a paginated list of answers.
/// </summary>
public record PaginatedAnswersDto
{
    /// <summary>
    /// Gets the list of answers in the current page.
    /// </summary>
    public List<AnswerDto> Items { get; init; } = new();

    /// <summary>
    /// Gets the total count of answers across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Gets whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
