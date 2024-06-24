using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure
{
    public class EventMapping : IEventMapping
    {
        public string SourceType { get; set; }
        public string TargetType { get; set; }
    }
}
