using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Views
{
    public class JsonView : IDataView
    {
        private readonly string m_Name;

        private JObject m_JsonObject;
        private string m_JsonText;
        private bool m_TooLong;
        private bool m_Foldout;

        public JsonView(string name, JObject jsonObject, bool foldout)
        {
            SetJsonObject(jsonObject);
            m_Name = name;
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var foldoutContent = new GUIContent(m_Name);
                if (m_JsonObject == null)
                {
                    foldoutContent.image = EditorHelper.LoadBuiltInIcon("console.erroricon.sml");
                }
                else if (m_TooLong == true)
                {
                    foldoutContent.image = EditorHelper.LoadBuiltInIcon("console.warnicon.sml");
                }
                m_Foldout = EditorGUILayout.Foldout(m_Foldout, foldoutContent, true);

                using (new GuiEnabledScope(m_JsonObject != null))
                {
                    if (GUILayout.Button("Edit", GUILayout.Width(80)) == true)
                    {
                        // open the json text in an editor using a temp file
                        string path = $"{FileUtil.GetUniqueTempPathInProject()}.json";
                        Utils.IO.WriteTextToFile(path, m_JsonText);
                        Utils.Code.RunCommand("code", $"-w -n {path}");

                        ParseJson(Utils.IO.ReadTextFromFile(path));
                    }
                }
            }

            if (m_Foldout == true)
            {
                using (new GuiIndentScope())
                {
                    if (m_JsonObject == null)
                    {
                        OnJsonMissing();
                    }
                    else if (m_TooLong == true)
                    {
                        // if the file is too long we cannot display it
                        EditorGUILayout.HelpBox("Cannot display External Meta Data. File is too large.", MessageType.Warning);
                    }
                    else
                    {
                        // show the json file as text
                        using (new GuiSkinFontScope(GUI.skin.label, Resources.Load<Font>("Consolas")))
                        {
                            using (new GuiSkinFontSizeScope(GUI.skin.label, 12))
                            {
                                EditorGUILayout.TextArea(m_JsonText, GUI.skin.label);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnJsonMissing() { }

        protected void SetJsonObject(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                m_JsonText = GetJsonString(jsonObject);
                m_TooLong = (m_JsonText.Length > 11056);
            }
            m_JsonObject = jsonObject;
        }

        private void ParseJson(string jsonText)
        {
            try
            {
                m_JsonObject = JObject.Parse(jsonText);
                m_JsonText = GetJsonString(m_JsonObject);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to deserialise modified text into JSON\n{ex}\n\n\"{jsonText}\"");
            }
        }

        private static string GetJsonString(JObject json)
        {
            return json.ToString(Newtonsoft.Json.Formatting.Indented);
        }
    }
}