using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class IndexWriter
{
    public static void Write<T>(List<T> list, string path, Func<T, string> getId, Func<T, string> getComment)
    {
        var index = list.Extract(getId);

        string jsonText = Json.Library.WriteToText(index, true);
        //var matches = Regex.Matches(jsonText, "\"(?'id'[^\"]+)\"");

        //int offset = 0;
        //var comments = list.Extract(getId, getComment);
        //foreach (Match match in matches)
        //{
        //    string id = match.Groups["id"].Value;
        //    string comment = $" // {comments[id]}";
        //    int end = match.Index + match.Length + offset;
        //    if (jsonText[end] != ',')
        //    {
        //        comment = " " + comment;
        //    }
        //    else
        //    {
        //        end++;
        //    }
        //    jsonText = jsonText.Insert(end, comment);
        //    offset += comment.Length;
        //}
        Utils.IO.WriteTextToFile($"{path}.json", jsonText);
    }
}
