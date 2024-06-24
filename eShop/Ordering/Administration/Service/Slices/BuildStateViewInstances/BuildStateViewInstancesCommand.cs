using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstances;

public class BuildStateViewInstancesCommand : Command
{
    public BuildStateViewInstancesCommand()
        : base()
    {
    }

    public BuildStateViewInstancesCommand
    (
        string stateViewName,
        string triggeredBy,
        int lastSequenceNumber = 0
    )
        : base(triggeredBy, lastSequenceNumber)
    {
        StateViewName = stateViewName;
    }

    public string StateViewName { get; init; }
}