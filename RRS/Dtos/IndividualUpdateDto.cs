﻿using System.ComponentModel.DataAnnotations;

namespace RRS.Dtos;

public class IndividualUpdateDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(50, ErrorMessage = "Address cannot be longer than 50 characters.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(40, ErrorMessage = "Email cannot be longer than 40 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Phone number must contain 7 to 15 digits.")]
    public string Phone { get; set; }
}