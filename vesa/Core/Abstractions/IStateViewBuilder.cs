namespace vesa.Core.Abstractions;

public interface IStateViewBuilder
{
    Task BuildAllStateViewsAsync();
}


public interface IStateViewBuilder<TStateView> 
    where TStateView : class, IStateView
{
    Task BuildStateViewsAsync(Type stateViewType);
    Task BuildStateViewAsync(string subject);
}