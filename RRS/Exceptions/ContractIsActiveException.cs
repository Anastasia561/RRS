namespace RRS.Exceptions;

public class ContractIsActiveException : Exception
{
    public ContractIsActiveException(int id)
        : base($"Client with id {id} already has contract active for provided version of software ")
    {
    }
}