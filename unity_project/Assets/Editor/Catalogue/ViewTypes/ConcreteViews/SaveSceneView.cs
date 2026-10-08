using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class SaveSceneView
    {
        private readonly Catalogue m_Catalogue;
        private readonly SaveScene m_SaveScene;

        public SaveSceneView(Catalogue catalogue, SaveScene saveScene)
        {
            m_Catalogue = catalogue;
            m_SaveScene = saveScene;
        }

        public void OnGUI()
        {
            foreach (var obj in m_SaveScene.Objects)
            {
                EditorGUILayout.TextField("Runtime ID", obj.RuntimeId);
                EditorGUILayout.Vector3Field("Position", obj.Position);
                EditorHelper.QuaternionField("Rotation", obj.Rotation);

                EditorGUILayout.Space();

                if (string.IsNullOrEmpty(obj.CatalogueId) == false)
                {
                    EditorGUILayout.TextField("Catalogue ID", obj.CatalogueId);
                    var entry = m_Catalogue.Entries.Find(e => e.Guid == obj.CatalogueId);
                    if (entry != null)
                    {
                        EditorGUILayout.LabelField("Full Name", entry.FullName.Text);
                        EditorGUILayout.LabelField("Short Name", entry.ShortName.Text);
                    }
                }

                if (obj.Modules != null)
                {
                    EditorGUILayout.LabelField("Modules");
                    foreach (var module in obj.Modules)
                    {
                        EditorGUILayout.TextField(module);
                    }
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Contents");
                for (int i = 0; i < obj.Children.Count; ++i)
                {
                    var formatting = Newtonsoft.Json.Formatting.Indented;
                    string json = obj.Children[i].ToString(formatting);
                    json = EditorGUILayout.TextArea(json);

                    if (GUILayout.Button("Edit") == true)
                    {
                        string path = $"{FileUtil.GetUniqueTempPathInProject()}.json";
                        Utils.IO.WriteTextToFile(path, json);

                        Utils.Code.RunCommand("code", $"-w -n {path}");

                        json = Utils.IO.ReadTextFromFile(path);
                        try
                        {
                            obj.Children[i] = JObject.Parse(json);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"Failed to deserialise modified text into JSON\n{ex}\n\n\"{json}\"");
                        }
                    }
                }
            }
        }
    }
}