using ARViewer;
using EditorTools.Views;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ArFeaturesDataView : MetaDataView<ArFeaturesMetaData>
    {
        private int m_iXrayObjectsCount = 0;

        public ArFeaturesDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {

        }

        protected override void OnMetaDataGUI()
        {
            if (GUILayout.Button("Auto Generate!") == true)
            {
                // Check if the info button will return anything.
                ProductInfoManager.GetProductInfo(m_Entry.ReferenceNumber, (productInfo) =>
                {
                    m_MetaData.m_bHasInfo = (null != productInfo);
                });

                // Check if it supports explosion.
                // For now this just looks for an animation component, this might need to end up more complicated.
                RenderMetaData renderMetaData = m_Entry.GetMetaData<RenderMetaData>();
                GameObject prefabObject = GameObject.Find(renderMetaData.Main.Prefab);
                m_MetaData.m_bSupportsExplosion = (null != prefabObject.GetComponentInChildren<Animator>());

                // Check if it supports xray.
                m_MetaData.m_bSupportsXRay = ((null != m_MetaData.m_XrayObjectNames) && (m_MetaData.m_XrayObjectNames.Count > 0));

                // Check if this supports AR Guide.
                m_MetaData.m_bSupportsArGuide = false;
                ArGuideData arGuideData = prefabObject.GetComponentInChildren<ArGuideData>();
                if (null != arGuideData)
                {
                    if ((null != arGuideData.m_XmlFile) && (null != arGuideData.m_DataFile))
                    {
                        m_MetaData.m_bSupportsArGuide = true;

                        // TODO: Name doesn't include extension, might not be able to do this here.
                        //if ((true == arGuideData.m_XmlFile.name.EndsWith(".xml")) && (true == arGuideData.m_DataFile.name.EndsWith(".bytes")))
                        //{
                        //    m_MetaData.m_bSupportsArGuide = true;
                        //}
                        //else
                        //{
                        //    Debug.LogError("AR Guide not setup correctly: " 
                        //        + ((false == arGuideData.m_XmlFile.name.EndsWith(".xml")) ? ("XML: Wrong file type assigned. ") : ("")) 
                        //        + ((false == arGuideData.m_DataFile.name.EndsWith(".bytes")) ? ("Data: Wrong file type assigned.") : ("")));
                        //}
                    }
                    else
                    {
                        Debug.LogError("AR Guide not setup correctly: " 
                            + ((null == arGuideData.m_XmlFile) ? ("XML File is not assigned. ") : ("")) 
                            + ((null == arGuideData.m_DataFile) ? ("Data File is not assigned.") : ("")));
                    }
                }

                // Check for interaction.
                ContentClickMetaData clickMetaData = m_Entry.GetMetaData<ContentClickMetaData>();
                ContentDialMetaData dialMetaData = m_Entry.GetMetaData<ContentDialMetaData>();
                ContentButtonMetaData buttonMetaData = m_Entry.GetMetaData<ContentButtonMetaData>();
                ContentSliderMetaData sliderMetaData = m_Entry.GetMetaData<ContentSliderMetaData>();

                m_MetaData.m_bSupportsInteraction = ((null != dialMetaData) || (null != buttonMetaData) || (null != sliderMetaData));

                // Check for patches. // TODO: Might want sliders in this at some point.
                if (null != dialMetaData)
                {
                    m_MetaData.m_bSupportsPatches = (dialMetaData.m_PatchData.Count > 0);
                }
                else
                {
                    m_MetaData.m_bSupportsPatches = false;
                }

                // Check for instruction.
                bool bHasInstruction = false;

                if (null != clickMetaData)
                {
                    foreach (InstructionData item in clickMetaData.m_ClickData)
                    {
                        if ((false == string.IsNullOrEmpty(item.m_sTitle)) && (false == string.IsNullOrEmpty(item.m_sDescription)))
                        {
                            bHasInstruction = true;
                            break;
                        }
                    }
                }

                if ((false == bHasInstruction) && (null != dialMetaData))
                {
                    foreach (InstructionData item in dialMetaData.m_DialData)
                    {
                        if ((false == string.IsNullOrEmpty(item.m_sTitle)) && (false == string.IsNullOrEmpty(item.m_sDescription)))
                        {
                            bHasInstruction = true;
                            break;
                        }
                    }
                }

                if ((false == bHasInstruction) && (null != buttonMetaData))
                {
                    foreach (InstructionData item in buttonMetaData.m_ItemData)
                    {
                        if ((false == string.IsNullOrEmpty(item.m_sTitle)) && (false == string.IsNullOrEmpty(item.m_sDescription)))
                        {
                            bHasInstruction = true;
                            break;
                        }
                    }
                }

                if ((false == bHasInstruction) && (null != sliderMetaData))
                {
                    foreach (InstructionData item in sliderMetaData.m_ItemData)
                    {
                        if ((false == string.IsNullOrEmpty(item.m_sTitle)) && (false == string.IsNullOrEmpty(item.m_sDescription)))
                        {
                            bHasInstruction = true;
                            break;
                        }
                    }
                }

                m_MetaData.m_bSupportsInstruction = bHasInstruction;
            }

            EditorGUILayout.Space();

            m_MetaData.m_bSupportsExplosion = EditorGUILayout.Toggle("Supports Explosion:", m_MetaData.m_bSupportsExplosion);
            m_MetaData.m_bSupportsXRay = EditorGUILayout.Toggle("Supports X-Ray:", m_MetaData.m_bSupportsXRay);
            m_MetaData.m_bSupportsInteraction = EditorGUILayout.Toggle("Supports Interaction:", m_MetaData.m_bSupportsInteraction);
            m_MetaData.m_bSupportsInstruction = EditorGUILayout.Toggle("Supports Instruction:", m_MetaData.m_bSupportsInstruction);
            m_MetaData.m_bSupportsArGuide = EditorGUILayout.Toggle("Supports AR Guide:", m_MetaData.m_bSupportsArGuide);
            m_MetaData.m_bSupportsPatches = EditorGUILayout.Toggle("Supports Patches:", m_MetaData.m_bSupportsPatches);
            m_MetaData.m_bHasInfo = EditorGUILayout.Toggle("Has Info Available:", m_MetaData.m_bHasInfo);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Assign X-Ray Objects:");

            m_iXrayObjectsCount = EditorGUILayout.IntField("Length:", m_MetaData.m_XrayObjectNames.Count);

            while (m_iXrayObjectsCount > m_MetaData.m_XrayObjectNames.Count)
            {
                m_MetaData.m_XrayObjectNames.Add("");
                m_MetaData.m_XrayGameObjects.Add(null);
            }

            while (m_iXrayObjectsCount < m_MetaData.m_XrayObjectNames.Count)
            {
                m_MetaData.m_XrayObjectNames.RemoveAt(m_MetaData.m_XrayObjectNames.Count - 1);
                m_MetaData.m_XrayGameObjects.RemoveAt(m_MetaData.m_XrayGameObjects.Count - 1);
            }

            GameObject xRayObject = null;
            string xRayName = "";
            for (int i = 0; i < m_iXrayObjectsCount; i++)
            {
                xRayObject = m_MetaData.m_XrayGameObjects[i];
                xRayName = m_MetaData.m_XrayObjectNames[i];
                xRayObject = StringToGameObject(xRayName, m_RootGameObject);
                GameObjectToString("Pivot Object", ref xRayObject, ref xRayName);
                m_MetaData.m_XrayGameObjects[i] = xRayObject;

                // If there's no assigned object this time, use the name that's already saved.
                if (null == xRayObject)
                {
                    xRayName = m_MetaData.m_XrayObjectNames[i];
                }

                EditorGUILayout.LabelField("X-Ray Object path", xRayName);
                m_MetaData.m_XrayObjectNames[i] = xRayName;
            }
        }
    }
}

