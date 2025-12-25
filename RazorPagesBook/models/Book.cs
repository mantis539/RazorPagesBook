using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RazorPagesBook.Models;

public class Book
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Author { get; set; } = string.Empty;

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Genre { get; set; } = string.Empty;

    [Range(1, 100)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [RegularExpression(@"^([1-4]([.,]\d)?|5([.,]0)?)$")]
    [Required]
    public double Rating { get; set; } 

    public string? CoverPath { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
