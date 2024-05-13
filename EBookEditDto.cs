using System.ComponentModel.DataAnnotations;

namespace ebooks_dotnet7_api;

/// <summary>
/// Represents an eBook entity.
/// </summary>
public class EBookEditDto
{
    public string? genre {get;set;}
    public string? author {get;set;}
    public string? formato {get;set;}
    public int? price {get;set;}
}
