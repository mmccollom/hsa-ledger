namespace HsaLedger.Client.Common;

public static class ListExtensions
{
    public static bool HasChangesComparedTo(this List<string>? current, List<string>? other)
    {
        if (current == null && other == null)
            return false;

        if (current == null || other == null)
            return true;

        if (current.Count != other.Count)
            return true;

        // Compare content and order
        for (int i = 0; i < current.Count; i++)
        {
            if (!string.Equals(current[i], other[i], StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
