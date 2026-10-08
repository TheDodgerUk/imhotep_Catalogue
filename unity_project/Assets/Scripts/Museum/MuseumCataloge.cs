using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Museum
{
    public class MuseumCataloge : MonoBehaviour
    {
        private string m_CurrentEnviromentName = null;
        public List<string> m_CatalogueNameList = new List<string>();

        public void Initilise()
        {
            string environmentName = PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME, "NONE");
            if ((true == string.IsNullOrEmpty(environmentName)) || (m_CurrentEnviromentName != environmentName))
            {

                for (int i = 0; i < Catalogue.Instance.Entries.Count; i++)
                {
                    if (true == Catalogue.Instance.Entries[i].EnvironmentsAvailable.Contains(environmentName))
                    {
                        m_CatalogueNameList.Add(Catalogue.Instance.Entries[i].ShortName.Text);
                    }
                }

                Catalogue.Instance.OnPostLoad();
            }
            m_CurrentEnviromentName = environmentName;
        }

        public GameObject LoadPrefab(CatalogueEntry CatalogueEntryRef)
        {
            var renderMetaData = CatalogueEntryRef.GetMetaData<RenderMetaData>();
            if (null != renderMetaData && null != renderMetaData.Main)
            {
                var prefab = LoadPrefab(renderMetaData.Main);
                var instance = GameObject.Instantiate(prefab) as GameObject;
                return instance;
            }
            return null;
        }


        private GameObject LoadPrefab(RenderItemData renderItem)
        {
            if (string.IsNullOrEmpty(renderItem.Guid) == false)
            {
#if UNITY_EDITOR
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(renderItem.Guid);
                if (string.IsNullOrEmpty(path) == false)
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
#endif
            }
            return null;
        }

    }
}
