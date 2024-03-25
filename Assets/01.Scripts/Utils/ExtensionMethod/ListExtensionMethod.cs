using System.Collections.Generic;

public static class ListExtensionMethod
{
    public static void Swap<T>(this List<T> list, int from, int to)
    {
        (list[from], list[to]) = (list[to], list[from]);
    }
}