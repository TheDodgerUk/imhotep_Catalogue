using System.Collections.Generic;

public static class Levenshtein
{
    public static int Distance(string lhs, string rhs, bool caseSensitive = true)
    {
        if (!caseSensitive)
        {
            lhs = lhs.ToLower();
            rhs = rhs.ToLower();
        }
        char[] first = lhs.ToCharArray();
        char[] second = rhs.ToCharArray();
        return Distance(first, second);
    }

    public static int Distance<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) where T : System.IEquatable<T>
    {
        // validate parameters
        if (lhs == null) throw new System.ArgumentNullException(nameof(lhs));
        if (rhs == null) throw new System.ArgumentNullException(nameof(rhs));

        // convert the parameters into IList instances in order to obtain indexing capabilities
        var first = lhs as IList<T> ?? new List<T>(lhs);
        var second = rhs as IList<T> ?? new List<T>(rhs);

        // if either is 0, return the length of the other, since that number of insertions would be required.
        int n = first.Count, m = second.Count;
        if (n == 0) return m;
        if (m == 0) return n;

        // Rather than maintain an entire matrix (which would require O(n*m) space),
        // just store the current row and the next row, each of which has a length m+1,
        // so just O(m) space. Initialize the current row.
        int curRow = 0, nextRow = 1;

        var rows = new int[][] { new int[m + 1], new int[m + 1] };
        for (int j = 0; j <= m; ++j)
        {
            rows[curRow][j] = j;
        }

        // For each virtual row (since we only have physical storage for two)
        for (int i = 1; i <= n; ++i)
        {
            // Fill in the values in the row
            rows[nextRow][0] = i;

            for (int j = 1; j <= m; ++j)
            {
                int dist1 = rows[curRow][j] + 1;
                int dist2 = rows[nextRow][j - 1] + 1;
                int dist3 = rows[curRow][j - 1] +
                    (first[i - 1].Equals(second[j - 1]) ? 0 : 1);

                rows[nextRow][j] = System.Math.Min(dist1, System.Math.Min(dist2, dist3));
            }

            // Swap the current and next rows
            if (curRow == 0)
            {
                curRow = 1;
                nextRow = 0;
            }
            else
            {
                curRow = 0;
                nextRow = 1;
            }
        }

        // Return the computed edit distance
        return rows[curRow][m];
    }
}