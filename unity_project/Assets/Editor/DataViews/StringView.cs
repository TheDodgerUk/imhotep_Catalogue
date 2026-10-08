using System.Collections.Generic;
using UnityEditor;

namespace EditorTools.Views
{
    public class StringView : ListView<string>
    {
        public StringView(string name, List<string> list) : base(name, list) { }

        protected override string DrawElement(string element)
        {
            return EditorGUILayout.TextField(element);
        }

        protected override void OnAdd()
        {
            Add(string.Empty);
        }
    }
}
