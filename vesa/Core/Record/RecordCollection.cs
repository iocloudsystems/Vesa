using System.Collections.ObjectModel;

namespace vesa.Core.Record;

public class RecordCollection<T> : ReadOnlyCollection<T>
{
    public RecordCollection(params T[] values) : base(values == null
        ? new List<T>()
        : new List<T>(values))
    {
    }

    public RecordCollection(IList<T> list) : base(list)
    {
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is RecordCollection<T> collection &&
               collection.Items.SequenceEqual(Items);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (T item in Items)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }
}