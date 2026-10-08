using UnityEditor;
using UnityEngine;

public class PlaceObject : EditorWindow
{
    [MenuItem("Window/Utility/Place Objects")]
    private static void ShowWindow() => GetWindow<PlaceObject>(true, "Place Object");

    [SerializeField]
    private GameObject m_ObjectToPlace;
    private string m_ObjectName;
    private GameObject m_RootObject;

    private bool m_bPlaceAtChildLocations;
    private bool m_bAppend;
    private bool m_bIgnoreScale = true;
    private GameObject m_ExistingObject;

    private void OnGUI()
    {
        m_bPlaceAtChildLocations = EditorGUILayout.Toggle("Place at child locations", m_bPlaceAtChildLocations);
        m_bAppend = EditorGUILayout.Toggle("Append to existing", m_bAppend);
        m_bIgnoreScale = EditorGUILayout.Toggle("Dont modify scale", m_bIgnoreScale);
        m_ObjectToPlace = EditorHelper.ObjectField("Object to Place", m_ObjectToPlace);

        if (true == m_bAppend)
        {
            m_ExistingObject = EditorHelper.ObjectField("Append Root", m_ExistingObject);
        }
        else
        {
            m_RootObject = EditorHelper.ObjectField("Root object", m_RootObject);
            m_ObjectName = EditorGUILayout.TextField("Parent Name", m_ObjectName);
        }

        if (m_ObjectToPlace != null)
        {
            var transforms = Selection.transforms;
            EditorGUILayout.LabelField($"Ready to place on {transforms.Length} objects");

            if (GUILayout.Button("Place Object on Selected Objects") == true)
            {
                GameObject attachParent = null;
                if (false == m_bAppend)
                {
                    attachParent = CreateParent(m_RootObject.transform, m_ObjectName);
                }
                else
                {
                    //Appending to existing object
                    attachParent = m_ExistingObject;
                }

                if (false == m_bPlaceAtChildLocations)
                {
                    //Place at selection only
                    ProcessSelection(transforms, attachParent.transform);
                }
                else
                {
                    //Place at each child location
                    foreach(Transform parent in transforms)
                    {
                        attachParent = CreateParent(parent.transform, parent.gameObject.name);
                        Transform[] children = parent.GetDirectChildren().ToArray();
                        ProcessSelection(children, attachParent.transform);
                    }
                }
            }
        }
    }

    private GameObject CreateParent(Transform sample, string parentName)
    {
        //Creating a new root object
        GameObject attachParent = new GameObject(parentName);
        attachParent.transform.position = sample.position;
        attachParent.transform.rotation = sample.rotation;

        return attachParent;
    }

    private void ProcessSelection(Transform[] selection, Transform attachParent)
    {
        for (int i = 0; i < selection.Length; i++)
        {
            var clone = Utils.Unity.Clone(m_ObjectToPlace, attachParent);
            Undo.RegisterCreatedObjectUndo(clone, $"Placed {clone.name}");
            clone.transform.localRotation = selection[i].rotation;
            clone.transform.localPosition = selection[i].position;
            if (false == m_bIgnoreScale)
            {
                clone.transform.localScale = selection[i].localScale;
            }
        }
    }

    private void OnSelectionChange()
    {
        Repaint();
    }
}
