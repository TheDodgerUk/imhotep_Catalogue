using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{

    public abstract class MetaDataView<T> : IMetaDataView where T : class
    {
        protected readonly CatalogueEntry m_Entry;
        protected readonly Catalogue m_Catalogue;
        protected readonly T m_MetaData;

        public RenderMetaData m_RenderMetaData;
        public GameObject m_RootGameObject;

        private readonly string m_Name;
        private bool m_Foldout;

        public MetaDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews)
        {
            var oldView = oldViews.Find(v => v is MetaDataView<T>) as MetaDataView<T>;
            if (oldView != null)
            {
                m_Foldout = oldView.m_Foldout;
            }

            m_MetaData = entry.GetMetaData<T>();
            m_Name = typeof(T).Name;
            m_Catalogue = catalogue;
            m_Entry = entry;

            m_RenderMetaData = m_Entry.GetMetaData<RenderMetaData>();
            m_RootGameObject = GameObject.Find(m_RenderMetaData.Main.Prefab);
        }

        private System.Action<bool> m_OnFoldOut;
        public void OnFoldout(System.Action<bool> callback) => m_OnFoldOut = callback;
        public void ToggleFoldout(bool state) => m_Foldout = state;


        public void OnGUI()
        {
            using (new GuiSkinFontScope(EditorStyles.foldout, EditorStyles.boldFont))
            {
                bool oldFoldout = m_Foldout;
                m_Foldout = EditorGUILayout.Foldout(m_Foldout, m_Name, true);
                if(oldFoldout != m_Foldout)
                {
                    m_OnFoldOut?.Invoke(m_Foldout);
                }
            }

            OnMetaDataUpdate();
            if (m_Foldout == true)
            {
                using (new GuiIndentScope())
                {
                    OnMetaDataGUI();
                }
            }
        }

        protected virtual void OnMetaDataUpdate() { }

        protected abstract void OnMetaDataGUI();

        public static void GameObjectToString(string label, GameObjectData data)
        {
            GameObjectToString(label, ref data.GameObjectRef, ref data.GameObjectName);
            EditorGUILayout.LabelField($"{label} path", data.GameObjectName);
            EditorGUILayout.Space();
        }

        public static bool GameObjectToString(string ObjectFieldText, ref UnityEngine.GameObject obj, ref string objStringContainer, bool force = false)
        {
            var temp = EditorHelper.ObjectField(ObjectFieldText, obj);
            if ((temp != null && obj != temp) || (temp != null && force == true) || (temp != null && string.IsNullOrEmpty(objStringContainer) == true)) 
            {
                obj = temp;

                string extentionName = obj.GetGameObjectPath().SanitiseSlashes();

                int index = extentionName.IndexOf($@"/");
                extentionName = extentionName.Remove(0, index + 1); // the plus 1 is to compensate and remove the  $@"/"
                index = extentionName.IndexOf($@"/");
                extentionName = extentionName.Remove(0, index + 1); // the plus 1 is to compensate and remove the  $@"/"
                objStringContainer = extentionName;
                return true;
            }
            return false;
        }

        public static bool ComponentToString<T>(string ObjectFieldText, ref T obj, ref string objStringContainer, bool force = false) where T : UnityEngine.Component
        {
            var temp = EditorHelper.ObjectField(ObjectFieldText, obj);
            if ((temp != null && obj != temp) || (temp != null && force == true))
            {
                obj = temp;

                string extentionName = obj.gameObject.GetGameObjectPath().SanitiseSlashes();

                int index = extentionName.IndexOf($@"/");
                extentionName = extentionName.Remove(0, index + 1); // the plus 1 is to compensate and remove the  $@"/"
                index = extentionName.IndexOf($@"/");
                extentionName = extentionName.Remove(0, index + 1); // the plus 1 is to compensate and remove the  $@"/"
                objStringContainer = extentionName;
                return true;
            }
            return false;
        }


        public static UnityEngine.GameObject StringToGameObject(string item, UnityEngine.GameObject root)
        {
            if(root == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(item) == false)
            {
                UnityEngine.Transform trans = root.transform.Search(item);
                if (trans != null)
                {
                    return trans.gameObject;
                }
            }
            return null;
        }

        public static T StringToComponent<T>(string item, UnityEngine.GameObject root) where T : UnityEngine.Component
        {
            if (root == null)
            {
                UnityEngine.Debug.LogError($"PrefabManager.LoadPrefab came back as null");
                return null;
            }
            if (string.IsNullOrEmpty(item) == false)
            {
                UnityEngine.Transform trans = root.transform.Search(item);
                if (trans != null)
                {
                    return trans.gameObject.GetComponent<T>();
                }
            }
            return null;
        }

    }

    public interface IMetaDataView
    {
        void ToggleFoldout(bool state);
        void OnGUI();
    }
}