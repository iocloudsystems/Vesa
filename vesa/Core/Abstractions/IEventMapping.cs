namespace vesa.Core.Abstractions
{
    public interface IEventMapping
    {
        string SourceType { get; set; }
        string TargetType { get; set; }
    }
}
