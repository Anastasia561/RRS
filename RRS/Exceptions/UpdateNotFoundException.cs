namespace RRS.Exceptions;

public class UpdateNotFoundException : Exception
{
    public UpdateNotFoundException(string name)
        : base($"Update with name {name} not found.")
    {
    }
}