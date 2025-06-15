using System.ComponentModel.DataAnnotations;

namespace RRS.Dtos;

public class ContractCreateDto
{
    [Required(ErrorMessage = "Start date is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime EndDate { get; set; }

    [Range(1, 3, ErrorMessage = "Update support years must be between 1 and 3.")]
    public int UpdatesSupportYears { get; set; }

    [Required(ErrorMessage = "At least one update is required.")]
    [MinLength(1, ErrorMessage = "At least one update must be provided.")]
    public List<string> Updates { get; set; }

    [Required(ErrorMessage = "Software ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Software ID must be a positive number.")]
    public int SoftwareId { get; set; }

    [Required(ErrorMessage = "Software version ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Software version ID must be a positive number.")]
    public int SoftwareVersionId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duration = (EndDate - StartDate).TotalDays;

        if (StartDate >= EndDate)
        {
            yield return new ValidationResult("Start date must be earlier than end date.",
                new[] { nameof(StartDate), nameof(EndDate) });
        }

        if (duration < 3 || duration > 30)
        {
            yield return new ValidationResult("Contract duration must be between 3 and 30 days.",
                new[] { nameof(StartDate), nameof(EndDate) });
        }
    }
}