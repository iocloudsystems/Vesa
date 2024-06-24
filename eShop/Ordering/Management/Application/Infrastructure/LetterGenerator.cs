using eShop.Ordering.Data.Enums;
using eShop.Ordering.Management.Application.Abstractions;

namespace eShop.Ordering.Management.Application.Infrastructure;

public class LetterGenerator : ILetterGenerator
{
    private readonly string _path;
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);

    public LetterGenerator(string path)
    {
        _path = path;
    }

    public string GetCorrespondenceTemplateName(CorrespondenceType correspondenceType)
    {
        return correspondenceType switch
        {
            //CorrespondenceType.ViolationLetter => "ViolationLetter.dot",
            CorrespondenceType.ViolationLetter => "ViolationLetterTemplate.txt",
            CorrespondenceType.ContraventionLetter => "ContraventionLetter.dot",
            _ => throw new ArgumentOutOfRangeException(String.Format("The given correspondence type {0} is not supported!", correspondenceType))
        };
    }

    public async Task<string> GenerateAsync(string fileName, string CorrespondenceTemplateName, IDictionary<string, string> fields, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        //TODO: do mail merge here
        //...
        var documentTemplate = @$"{_path}\ViolationLetterTemplate.txt";
        string text = await File.ReadAllTextAsync(documentTemplate, cancellationToken);

        var filePath = @$"{_path}\{fileName}";

        //List<string> list = new List<string>();
        foreach (var item in fields)
        {
            text = text.Replace($"<{item.Key}>", item.Value);
            // list.Add(item.Key + "=" + item.Value);
        }
        //string fileContents = String.Join(Environment.NewLine, list);

        await File.WriteAllTextAsync(filePath, text, cancellationToken);
        _semaphoregate.Release();
        return filePath;
    }
}
