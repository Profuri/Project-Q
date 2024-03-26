using System.Linq;
using System.Text;

public static class StringExtensitionMethod
{
    public static string ToUpperFirstChar(this string str)
    {
        if (str.Length <= 0)
        {
            return "";
        }
        
        var lower = str.ToLower();
        var firstChar = char.ToUpper(lower[0]);
        return firstChar + lower.Substring(1);
    }
    
    public static string GetCamelCase(string input)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var ch in input.Where(IsEnglishAlphabet))
        {
            sb.Append(ch);
        }
        return sb.ToString();
    }

    private static bool IsEnglishAlphabet(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }
}