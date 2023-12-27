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
}