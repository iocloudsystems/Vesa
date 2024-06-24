using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace vesa.Core.Abstractions;

public abstract class StateViewJson<TStateView>
    where TStateView : class, IStateView
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public DateTimeOffset StateViewDate { get; set; }

    [JsonIgnore]
    public string Json { get; set; }

    [NotMapped]
    public TStateView StateView
    {
        get => JsonConvert.DeserializeObject<TStateView>(Json);
        set
        {
            Id = value.Id;
            Subject = value.Subject;
            StateViewDate = value.StateViewDate;
            Json = JsonConvert.SerializeObject(value);
        }
    }
}
