namespace RRS.Services;

public class ContractException : Exception
{
    public ContractException(string message)
        : base(message)
    {
    }
}