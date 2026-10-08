using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentScaleDataView : MetaDataView<ContentScaleMetaData>
    {
        private Vector3 m_ObjScale = Vector3.one;
        public Vector3 GetObjectScale() { return m_ObjScale; }
        public void SetScale(Vector3 scale) { m_ObjScale = scale; }


        public ContentScaleDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) { }

        protected override void OnMetaDataGUI() { }

    }
}

