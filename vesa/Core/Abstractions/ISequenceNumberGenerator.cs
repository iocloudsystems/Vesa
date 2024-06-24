namespace vesa.Core.Abstractions;

public interface ISequenceNumberGenerator
{
    Task<long> GetNextSequenceNumberAsync(string subject);
}
