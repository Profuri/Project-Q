using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class StringUtil
{
    public static string GetCamelCase(string input)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (IsEnglishAlphabet(ch))
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
    
    public static bool IsEnglishAlphabet(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
}
