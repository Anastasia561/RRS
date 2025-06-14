using System.ComponentModel.DataAnnotations;

namespace RRS.Dtos;

public class CompanyUpdateDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(30, ErrorMessage = "Name must be at most 30 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(50, ErrorMessage = "Address must be at most 50 characters.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(40, ErrorMessage = "Email cannot be longer than 40 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Phone must contain 7 to 15 digits.")]
    public string Phone { get; set; }
}