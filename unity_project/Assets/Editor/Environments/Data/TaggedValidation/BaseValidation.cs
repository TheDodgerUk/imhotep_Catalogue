#if UNITY_EDITOR
#if CATALOG_PROGRAM
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BaseValidation : MonoBehaviour
{
    protected virtual string TAG => $"[Enter]";

    [SerializeField]
    protected string m_TriggerName = "";

    public bool IsTheNameValid() => ValidateGameObjectName();

    [InspectorButton]
    private void Awake()
    {
        SetCorrectStates();
    }

    [InspectorButton]
    private void CreateGameObjectName()
    {
        CreateGameObjectNameAndAssign();
    }


    protected virtual void SetCorrectStates()
    {
    }

    protected virtual void CreateGameObjectNameAndAssign()
    {

    }

    protected virtual bool ValidateGameObjectName()
    {
        return true;
    }

    [InspectorButton]
    private void ValidateHierachy()
    {
        _ValidateHierachy();
    }
  
    protected virtual void _ValidateHierachy()
    {
        bool errorsFound = false;
        List<Renderer> rendererList = this.GetComponentsInChildren<Renderer>().ToList();
        foreach(Renderer item in rendererList)
        {
            Collider colliderRef = item.GetComponent<Collider>();
            if(colliderRef != null)
            {
                errorsFound = true;
                Debug.LogError($"renderer cannot have a collider on it as the layers conflict {item.gameObject.GetGameObjectPath() }");
            }
        }

        List<Collider> colliderList = this.GetComponentsInChildren<Collider>().ToList();
        foreach (Collider item in colliderList)
        {

        }

        if(true == errorsFound)
        {
            if(true == EditorUtility.DisplayDialog("ErrorsFound", "do you wnat to try to autoFix it ", "Yes", "No"))
            {
                foreach (Renderer item in rendererList)
                {
                    BoxCollider colliderBoxRef = item.GetComponent<BoxCollider>();

                    if (colliderBoxRef != null)
                    {
                        CreateChildAndAddCollider<BoxCollider>(colliderBoxRef);
                        GameObject child = new GameObject(nameof(Collider));
                        child.transform.SetParent(item.transform);
                        child.gameObject.ClearLocals();
                        child.AddComponentCopy<BoxCollider>(colliderBoxRef);
                        DestroyImmediate(colliderBoxRef);
                    }

                    MeshCollider colliderMeshRef = item.GetComponent<MeshCollider>();
                    if (colliderBoxRef != null)
                    {
                        GameObject child = new GameObject(nameof(Collider));
                        child.transform.SetParent(item.transform);
                        child.gameObject.ClearLocals();
                        child.AddComponentCopy<MeshCollider>(colliderMeshRef);
                        DestroyImmediate(colliderMeshRef);
                    }
                }

                colliderList = this.GetComponentsInChildren<Collider>().ToList();
                foreach (Collider item in colliderList)
                {
                }
            }
        }
    }

    private void CreateChildAndAddCollider<T>(Collider obj) where T : Component
    {

    }
}
#endif
#endif
