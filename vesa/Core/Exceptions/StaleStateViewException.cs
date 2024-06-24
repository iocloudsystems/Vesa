namespace vesa.Core.Exceptions;

public class StaleStateViewException : Exception
{
    public StaleStateViewException(string commandId)
    {
        CommandId = commandId;
    }

    public string CommandId { get; init; }
}
