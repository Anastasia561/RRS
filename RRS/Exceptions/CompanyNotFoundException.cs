namespace RRS.Exceptions;

public class CompanyNotFoundException : Exception
{
    public CompanyNotFoundException(int id)
        : base($"Company with ID {id} not found.")
    {
    }
}