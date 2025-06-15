namespace RRS.Dtos;

public class ContractDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal FinalPrice { get; set; }
    public List<string> Updates { get; set; }
    public int UpdatesSupportYears { get; set; }
    public int IsSigned { get; set; }
    public SoftwareContractDto Software { get; set; }
    public VersionDto SoftwareVersion { get; set; }
}