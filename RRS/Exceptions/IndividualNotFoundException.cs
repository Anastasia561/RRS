namespace RRS.Exceptions;

public class IndividualNotFoundException : Exception
{
    public IndividualNotFoundException(int id)
        : base($"Individual with ID {id} not found or has been removed.")
    {
    }
}