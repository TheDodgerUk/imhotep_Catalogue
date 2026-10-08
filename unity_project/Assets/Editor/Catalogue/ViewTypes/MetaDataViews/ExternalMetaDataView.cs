using EditorTools.Views;
using FilePathHelper;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class ExternalMetaDataView : JsonView
    {
        private readonly ExternalMetaData m_Data;

        public ExternalMetaDataView(ExternalMetaData externalData) : base(externalData.Type.ToString(), externalData.JsonObject, false)
        {
            m_Data = externalData;
        }

        protected override void OnJsonMissing()
        {
            base.OnJsonMissing();

            // if json object is null, failed to find or load the file
            string path = FilePath.FromAbsolute(m_Data.ExternalPath).Relative;
            EditorGUILayout.HelpBox($"Could not find External Meta Data:\n\"{path}\"", MessageType.Error);

            if (GUILayout.Button("Retry", GUILayout.Width(80)) == true)
            {
                m_Data.LoadJson();
                SetJsonObject(m_Data.JsonObject);
            }
        }
    }
}