using System.Collections.Generic;

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 a, T2 b)
    {
        return new Tuple<T1, T2>(a, b);
    }

    public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 a, T2 b, T3 c)
    {
        return new Tuple<T1, T2, T3>(a, b, c);
    }

    public static Tuple<TKey, TValue> AsTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
    {
        return New(kvp.Key, kvp.Value);
    }
}

[System.Serializable]
public class Tuple<T1, T2>
{
    public T1 A;
    public T2 B;

    public Tuple(T1 a, T2 b)
    {
        A = a;
        B = b;
    }
}

[System.Serializable]
public class Tuple<T1, T2, T3> : Tuple<T1, T2>
{
    public T3 C;

    public Tuple(T1 a, T2 b, T3 c) : base(a, b)
    {
        C = c;
    }
}