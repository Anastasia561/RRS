using System.ComponentModel.DataAnnotations;

namespace RRS.Dtos;

public class PaymentDto
{
    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }
}