namespace RRS.Exceptions;

public class VersionNotFoundException : Exception
{
    public VersionNotFoundException(int id)
        : base($"Software version with id {id} not found.")
    {
    }
}