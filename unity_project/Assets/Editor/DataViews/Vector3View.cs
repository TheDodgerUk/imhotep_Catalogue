using EditorTools.CatalogueViewer;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Views
{
    public class Vector3View : DefaultListView<Vector3>
    {
        public Vector3View(string name, List<Vector3> list) : base(name, list) { }

        protected override Vector3 DrawElement(Vector3 element)
        {
            return EditorGUILayout.Vector3Field(GUIContent.none, element);
        }
    }
}
