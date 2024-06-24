using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class FileSequenceNumberGenerator : ISequenceNumberGenerator
{
    private readonly string _sequenceNumberPath;
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);

    public FileSequenceNumberGenerator(string sequenceNumberPath)
    {
        _sequenceNumberPath = sequenceNumberPath;
    }

    public async Task<long> GetNextSequenceNumberAsync(string sequenceName = "Global")
    {
        await _semaphoregate.WaitAsync();
        long sequenceNumber = 0;
        var sequenceNumberFileName = GetSequenceNumberFileName(sequenceName);
        if (File.Exists(sequenceNumberFileName))
        {
            sequenceNumber = long.Parse(await File.ReadAllTextAsync(sequenceNumberFileName));
        }
        await File.WriteAllTextAsync(sequenceNumberFileName, (++sequenceNumber).ToString());
        _semaphoregate.Release();
        return sequenceNumber;
    }

    private string GetSequenceNumberFileName(string sequenceName) => @$"{_sequenceNumberPath}\{sequenceName}.txt";
}
