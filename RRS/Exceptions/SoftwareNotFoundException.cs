namespace RRS.Exceptions;

public class SoftwareNotFoundException : Exception
{
    public SoftwareNotFoundException(int id)
        : base($"Software with ID {id} not found.")
    {
    }
}