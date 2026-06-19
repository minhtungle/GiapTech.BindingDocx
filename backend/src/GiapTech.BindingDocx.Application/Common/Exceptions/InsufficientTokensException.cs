namespace GiapTech.BindingDocx.Application.Common.Exceptions;

public class InsufficientTokensException : Exception
{
    public int Required { get; }
    public int Available { get; }

    public InsufficientTokensException(int required, int available)
        : base($"Insufficient tokens. Required: {required}, Available: {available}.")
    {
        Required = required;
        Available = available;
    }
}
