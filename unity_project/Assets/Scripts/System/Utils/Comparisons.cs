using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Comparisons
{
    public static int ByHierarchy<T>(T x, T y) where T : Component => ByHierarchy(x.transform, y.transform);
    public static int ByHierarchy(GameObject x, GameObject y) => ByHierarchy(x.transform, y.transform);
    public static int ByHierarchy(Transform x, Transform y) => HierarchyComparer.CompareHierarchy(x, y);

    public static int AlphaNumericByName<T>(T x, T y) where T : UnityEngine.Object => Alphanumeric(x.name, y.name);
    public static int NaturalByName<T>(T x, T y) where T : UnityEngine.Object => Natural(x.name, y.name);

    public static IComparer<T> GetHierarchy<T>() where T : Component => new HierarchyComparer<T>();
    public static IComparer<Transform> GetHierarchy() => new HierarchyComparer();

    public static IComparer<T> GetAlphanumeric<T>(Func<T, string> selector) => new AlphanumericComparer<T>(selector);
    public static IComparer<string> GetAlphanumeric() => new AlphanumericComparer();

    public static int Alphanumeric(string x, string y) => AlphanumericComparer.Alphanumeric(x, y);
    public static int Natural(string x, string y) => NaturalComparer.CompareNatural(x, y);
}

internal class HierarchyComparer<T> : Comparer<T> where T : Component
{
    public override int Compare(T x, T y) => HierarchyComparer.CompareHierarchy(x.transform, y.transform);
}

internal class HierarchyComparer : Comparer<Transform>
{
    public override int Compare(Transform x, Transform y) => CompareHierarchy(x, y);

    public static int CompareHierarchy(Transform x, Transform y)
    {
        if (x == y)
        {
            return 0;
        }
        if (y.IsChildOf(x))
        {
            return -1;
        }
        if (x.IsChildOf(y))
        {
            return 1;
        }

        var xParents = GetParents(x);
        var yParents = GetParents(y);

        for (int i = 0; i < xParents.Count; i++)
        {
            if (y.IsChildOf(xParents[i]))
            {
                int yIndex = yParents.IndexOf(xParents[i]) - 1;
                i--;
                return xParents[i].GetSiblingIndex() - yParents[yIndex].GetSiblingIndex();
            }
        }

        return xParents[xParents.Count - 1].GetSiblingIndex() - yParents[yParents.Count - 1].GetSiblingIndex();
    }

    private static List<Transform> GetParents(Transform t)
    {
        var parents = new List<Transform> { t };

        while (t.parent != null)
        {
            parents.Add(t.parent);
            t = t.parent;
        }
        return parents;
    }
}

internal class NaturalComparer : Comparer<string>
{
    public override int Compare(string x, string y) => CompareNatural(x, y);

    public static int CompareNatural(string strA, string strB)
    {
        return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
    }

    public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
    {
        CompareInfo cmp = culture.CompareInfo;
        int iA = 0;
        int iB = 0;
        int softResult = 0;
        int softResultWeight = 0;
        while (iA < strA.Length && iB < strB.Length)
        {
            bool isDigitA = char.IsDigit(strA[iA]);
            bool isDigitB = char.IsDigit(strB[iB]);
            if (isDigitA != isDigitB)
            {
                return cmp.Compare(strA, iA, strB, iB, options);
            }
            else if (!isDigitA && !isDigitB)
            {
                int jA = iA + 1;
                int jB = iB + 1;
                while (jA < strA.Length && !char.IsDigit(strA[jA])) jA++;
                while (jB < strB.Length && !char.IsDigit(strB[jB])) jB++;
                int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                if (cmpResult != 0)
                {
                    // Certain strings may be considered different due to "soft" differences that are
                    // ignored if more significant differences follow, e.g. a hyphen only affects the
                    // comparison if no other differences follow
                    string sectionA = strA.Substring(iA, jA - iA);
                    string sectionB = strB.Substring(iB, jB - iB);
                    if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                        cmp.Compare(sectionA + "2", sectionB + "1", options))
                    {
                        return cmp.Compare(strA, iA, strB, iB, options);
                    }
                    else if (softResultWeight < 1)
                    {
                        softResult = cmpResult;
                        softResultWeight = 1;
                    }
                }
                iA = jA;
                iB = jB;
            }
            else
            {
                char zeroA = (char)(strA[iA] - (int)char.GetNumericValue(strA[iA]));
                char zeroB = (char)(strB[iB] - (int)char.GetNumericValue(strB[iB]));
                int jA = iA;
                int jB = iB;
                while (jA < strA.Length && strA[jA] == zeroA) jA++;
                while (jB < strB.Length && strB[jB] == zeroB) jB++;
                int resultIfSameLength = 0;
                do
                {
                    isDigitA = jA < strA.Length && char.IsDigit(strA[jA]);
                    isDigitB = jB < strB.Length && char.IsDigit(strB[jB]);
                    int numA = isDigitA ? (int)char.GetNumericValue(strA[jA]) : 0;
                    int numB = isDigitB ? (int)char.GetNumericValue(strB[jB]) : 0;
                    if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                    if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                    if (isDigitA && isDigitB)
                    {
                        if (numA != numB && resultIfSameLength == 0)
                        {
                            resultIfSameLength = numA < numB ? -1 : 1;
                        }
                        jA++;
                        jB++;
                    }
                }
                while (isDigitA && isDigitB);
                if (isDigitA != isDigitB)
                {
                    // One number has more digits than the other (ignoring leading zeros) - the longer
                    // number must be larger
                    return isDigitA ? 1 : -1;
                }
                else if (resultIfSameLength != 0)
                {
                    // Both numbers are the same length (ignoring leading zeros) and at least one of
                    // the digits differed - the first difference determines the result
                    return resultIfSameLength;
                }
                int lA = jA - iA;
                int lB = jB - iB;
                if (lA != lB)
                {
                    // Both numbers are equivalent but one has more leading zeros
                    return lA > lB ? -1 : 1;
                }
                else if (zeroA != zeroB && softResultWeight < 2)
                {
                    softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                    softResultWeight = 2;
                }
                iA = jA;
                iB = jB;
            }
        }
        if (iA < strA.Length || iB < strB.Length)
        {
            return iA < strA.Length ? 1 : -1;
        }
        else if (softResult != 0)
        {
            return softResult;
        }
        return 0;
    }
}

internal class AlphanumericComparer<T> : Comparer<T>
{
    private AlphanumericComparer m_Comparer;
    private readonly Func<T, string> m_Selector;

    public AlphanumericComparer(Func<T, string> selector)
    {
        m_Comparer = new AlphanumericComparer();
        m_Selector = selector;
    }

    public override int Compare(T x, T y) => m_Comparer.Compare(m_Selector(x), m_Selector(y));
}

internal class AlphanumericComparer : Comparer<string>
{
    public override int Compare(string x, string y) => Alphanumeric(x, y);

    public static int Alphanumeric(string s1, string s2)
    {
        //get rid of special cases
        if ((s1 == null) && (s2 == null)) return 0;
        else if (s1 == null) return -1;
        else if (s2 == null) return 1;

        if ((s1.Equals(string.Empty) && (s2.Equals(string.Empty)))) return 0;
        else if (s1.Equals(string.Empty)) return -1;
        else if (s2.Equals(string.Empty)) return -1;

        //WE style, special case
        bool sp1 = char.IsLetterOrDigit(s1, 0);
        bool sp2 = char.IsLetterOrDigit(s2, 0);
        if (sp1 && !sp2) return 1;
        if (!sp1 && sp2) return -1;

        int i1 = 0, i2 = 0; //current index
        int r = 0; // temp result
        while (true)
        {
            bool c1 = char.IsDigit(s1, i1);
            bool c2 = char.IsDigit(s2, i2);

            if (!c1 && !c2)
            {
                bool letter1 = char.IsLetter(s1, i1);
                bool letter2 = char.IsLetter(s2, i2);
                if ((letter1 && letter2) || (!letter1 && !letter2))
                {
                    if (letter1 && letter2)
                    {
                        r = char.ToLower(s1[i1]).CompareTo(char.ToLower(s2[i2]));
                    }
                    else
                    {
                        r = s1[i1].CompareTo(s2[i2]);
                    }
                    if (r != 0) return r;
                }
                else if (!letter1 && letter2) return -1;
                else if (letter1 && !letter2) return 1;
            }
            else if (c1 && c2)
            {
                r = CompareNum(s1, ref i1, s2, ref i2);
                if (r != 0) return r;
            }
            else if (c1)
            {
                return -1;
            }
            else if (c2)
            {
                return 1;
            }

            i1++;
            i2++;

            if ((i1 >= s1.Length) && (i2 >= s2.Length))
            {
                return 0;
            }
            else if (i1 >= s1.Length)
            {
                return -1;
            }
            else if (i2 >= s2.Length)
            {
                return -1;
            }
        }
    }

    private static int CompareNum(string s1, ref int i1, string s2, ref int i2)
    {
        int nzStart1 = i1, nzStart2 = i2; // nz = non zero
        int end1 = i1, end2 = i2;

        ScanNumEnd(s1, i1, ref end1, ref nzStart1);
        ScanNumEnd(s2, i2, ref end2, ref nzStart2);
        int start1 = i1; i1 = end1 - 1;
        int start2 = i2; i2 = end2 - 1;

        int nzLength1 = end1 - nzStart1;
        int nzLength2 = end2 - nzStart2;

        if (nzLength1 < nzLength2) return -1;
        else if (nzLength1 > nzLength2) return 1;

        for (int j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
        {
            int r = s1[j1].CompareTo(s2[j2]);
            if (r != 0) return r;
        }
        // the nz parts are equal
        int length1 = end1 - start1;
        int length2 = end2 - start2;
        if (length1 == length2) return 0;
        if (length1 > length2) return -1;
        return 1;
    }

    private static void ScanNumEnd(string s, int start, ref int end, ref int nzStart)
    {
        nzStart = start;
        end = start;
        bool countZeros = true;
        while (char.IsDigit(s, end))
        {
            if (countZeros && s[end].Equals('0'))
            {
                nzStart++;
            }
            else countZeros = false;
            end++;
            if (end >= s.Length) break;
        }
    }
}