namespace vesa.Core.Extensions;

public static class StringExtensions
{
    public static int NthIndexOf(this string input, char value, int n)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException("n", n, "n is less than zero.");

        int i = -1;
        do
        {
            i = input.IndexOf(value, i + 1);
            n--;
        }
        while (i != -1 && n > 0);

        return i;
    }
}
