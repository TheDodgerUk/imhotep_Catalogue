using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentHelpers
{
    public static class Helpers
    {
        public static bool IsNamedTagged(GameObject objectRef)
        {
            string namedTag = "";
            return GetNamedTagged(objectRef, ref namedTag);
        }

        public static bool GetNamedTagged(GameObject objectRef, ref string taggedName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(objectRef.name, @"\[(\w+)\]");
            taggedName = match.Value;
            return match.Success;
        }

        public static string GetObjectName(GameObject gameObjectRef)
        {
            var split = gameObjectRef.name.Split('_');
            if (split.Length > 1)
            {
                return split[1];
            }
            return null;
        }

        public static List<string> GetObjectNameExtentionArguments(GameObject gameObjectRef)
        {
            var split = gameObjectRef.name.Split('_');
            List<string> argumnets = new List<string>(split);
            argumnets.RemoveAt(0);
            argumnets.RemoveAt(0);
            return argumnets;
        }


        public static string GetObjectNameExtentionArguments(GameObject gameObjectRef, int argumentIndex)
        {
            List<string> arguments = GetObjectNameExtentionArguments(gameObjectRef);
            if (arguments.Count > argumentIndex)
            {
                return arguments[argumentIndex];
            }
            return null;
        }
    }


}
