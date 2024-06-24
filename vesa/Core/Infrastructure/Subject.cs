namespace vesa.Core.Infrastructure;

public class Subject
{
    private readonly string _subject;

    public Subject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentOutOfRangeException(nameof(subject), "Subject cannot empty.");
        }
        _subject = subject;
    }

    public string Name => _subject.Contains('_') ? _subject.Substring(0, _subject.IndexOf('_')) : _subject;

    public string Value => (_subject.Contains('_') && _subject.IndexOf('_') != _subject.Length - 1) ? _subject.Substring(_subject.IndexOf('_') + 1) : null;

    public static implicit operator string(Subject d) => d._subject;
    public static explicit operator Subject(string subject) => new Subject(subject);

    public override string ToString() => $"{_subject}";
}
