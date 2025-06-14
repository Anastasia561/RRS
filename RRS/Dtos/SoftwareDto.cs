using RRS.Models;

namespace RRS.Dtos;

public class SoftwareDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal YearCost { get; set; }
    public string Category { get; set; }
    public VersionDto CurrentVersion { get; set; }
    public List<DiscountDto> Discounts { get; set; }
}