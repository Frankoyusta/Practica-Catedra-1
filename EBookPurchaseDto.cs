using System.ComponentModel.DataAnnotations;

namespace ebooks_dotnet7_api;

/// <summary>
/// Represents an eBook entity.
/// </summary>
public class EBookPurchaseDto
{
    public int id {get; set;}
    public int totalCant{get;set;}
    public int totalPrice{get;set;}
}
