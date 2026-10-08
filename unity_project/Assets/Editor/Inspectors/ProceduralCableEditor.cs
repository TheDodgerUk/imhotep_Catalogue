using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralCable))]
public class ProceduralCableEditor : InspectorMethodEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cable = target as ProceduralCable;

        if (GUILayout.Button("SaveAsFBX()") == true)
        {
            var fbx = ProceduralTubeEditor.SaveCableAsFbx(cable.WireMesh);

            if (cable.BakedMesh == null)
            {
                cable.BakedMesh = PrefabUtility.InstantiatePrefab(fbx) as GameObject;
                cable.BakedMesh.GetComponent<Renderer>().sharedMaterial = cable.WireMaterial;
                cable.BakedMesh.transform.SetParent(cable.transform, true);
            }
            cable.BakedMesh.transform.OrientTo(cable.WireMesh.transform);
            cable.WireMesh.SetActive(false);
            cable.IsDirtyBake = false;
        }
        using (new GuiEnabledScope(cable.BakedMesh != null))
        {
            if (GUILayout.Button("UpdateFBX()") == true)
            {
                ProceduralTubeEditor.OverwriteFbx(cable.WireMesh, cable.BakedMesh);
                cable.BakedMesh.transform.OrientTo(cable.WireMesh.transform);
                cable.BakedMesh.SetActive(true);
                cable.WireMesh.SetActive(false);
            }
        }
    }
}
