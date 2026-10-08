using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Parse
    {
        public static Dictionary<string, T> ToDictionary<T>(List<string> variables, int stride, Func<List<string>, int, T> parser, Action<T> single = null)
        {
            // parse pins to selection
            var dict = new Dictionary<string, T>();
            if (variables.Count >= stride)
            {
                single?.Invoke(parser(variables, 1));
                for (int i = 0; i < variables.Count - (stride - 1); i += stride)
                {
                    var item = parser(variables, i + 1);
                    var id = variables[i].Strip("\"");
                    dict.Add(id, item);
                }
            }
            else
            {
                single?.Invoke(parser(variables, 0));
            }
            return dict;
        }

        public static List<Vector3> ToVector3List(List<string> values)
        {
            if ((values.Count % 2) == 0)
            {
                var list = new List<Vector3>();
                for (int i = 0; i < values.Count - 1; i += 2)
                {
                    list.Add(ToVector3(values, i));
                }
                return list;
            }
            return null;
        }

        public static Vector3 ToVector3(List<string> values, int indexOffset = 0)
        {
            if (values.Count >= 2)
            {
                return new Vector3(0, ToFloat(values[1 + indexOffset]), ToFloat(values[0 + indexOffset]));
            }
            return Vector3.zero;
        }

        public static float ToFloat(List<string> value)
        {
            return float.Parse(value[0]);
        }

        public static float ToFloat(string value)
        {
            return float.Parse(value);
        }
    }
}
