using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstance;

public class BuildStateViewInstanceCommand : Command
{
    public BuildStateViewInstanceCommand()
         : base()
    {
    }

    public BuildStateViewInstanceCommand
    (
        string stateViewName,
        string subject,
        string triggeredBy,
        int lastSequenceNumber = 0
    )
        : base(triggeredBy, lastSequenceNumber)
    {
        StateViewName = stateViewName;
        Subject = subject;
    }

    public string StateViewName { get; init; }
    public string Subject { get; init; }
}