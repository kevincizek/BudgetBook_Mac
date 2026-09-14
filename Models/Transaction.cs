using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class Transaction
{
    public int Id { get; set; }

    [Range(0.01, 999999.99)]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime BookingDate { get; set; }

    public TransactionType Type { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    public string UserId { get; set; }
    public IdentityUser? User { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
