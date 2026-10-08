using System.Collections.Generic;
using UnityEngine;

namespace EditorTools.Views
{
    public class EnumListView<T> : UniqueListView<T>
    {
        public EnumListView(string name, List<T> list) : base(name, list, Utils.Code.GetEnumValues<T>()) { }

        protected override T DrawElement(T e)
        {
            return EditorHelper.EnumPopup(GUIContent.none, e, Unique.GetAvailableFor(e));
        }
    }
}