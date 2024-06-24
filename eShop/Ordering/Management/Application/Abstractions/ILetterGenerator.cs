using eShop.Ordering.Data.Enums;

namespace eShop.Ordering.Management.Application.Abstractions;

public interface ILetterGenerator
{
    string GetCorrespondenceTemplateName(CorrespondenceType correspondenceType);
    Task<string> GenerateAsync(string fileName, string correspondenceTypeName, IDictionary<string, string> fields, CancellationToken cancellation = default);
}
