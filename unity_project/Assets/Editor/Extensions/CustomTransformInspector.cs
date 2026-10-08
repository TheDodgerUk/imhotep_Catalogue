#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class CustomTransformComponent : Editor
{
    private Transform _transform;

    public override void OnInspectorGUI()
    {
        //We need this for all OnInspectorGUI sub methods
        _transform = (Transform)target;

        StandardTransformInspector();
    }

    private void StandardTransformInspector()
    {
        bool didPositionChange = false;
        bool didRotationChange = false;
        bool didScaleChange = false;

        bool didPositionReset = false;
        bool didRotationReset = false;
        bool didScaleReset = false;

        // Watch for changes.
        //  1)  Float values are imprecise, so floating point error may cause changes
        //      when you've not actually made a change.
        //  2)  This allows us to also record an undo point properly since we're only
        //      recording when something has changed.

        // Store current values for checking later
        Vector3 initialLocalPosition = _transform.localPosition;
        Vector3 initialLocalEuler = _transform.localEulerAngles;
        Vector3 initialLocalScale = _transform.localScale;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Position", GUILayout.MaxWidth(70));
        if (true == GUILayout.Button("X", GUILayout.MaxWidth(20)))
        {
            didPositionReset = true;
        }
        Vector3 localPosition = EditorGUILayout.Vector3Field("", _transform.localPosition);
        if (EditorGUI.EndChangeCheck())
            didPositionChange = true;
        EditorGUILayout.EndHorizontal();




        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotation", GUILayout.MaxWidth(70));
        if (true == GUILayout.Button("X", GUILayout.MaxWidth(20)))
        {
            didRotationReset = true;
        }
        Vector3 localEulerAngles = EditorGUILayout.Vector3Field("", _transform.localEulerAngles);
        if (EditorGUI.EndChangeCheck())
            didRotationChange = true;
        EditorGUILayout.EndHorizontal();



        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(70));
        if (true == GUILayout.Button("X", GUILayout.MaxWidth(20)))
        {
            didScaleReset = true;
        }
        Vector3 localScale = EditorGUILayout.Vector3Field("", _transform.localScale);
        if (EditorGUI.EndChangeCheck())
            didScaleChange = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (true == GUILayout.Button("Reset all Local"))
        {
            didPositionReset = true;
            didRotationReset = true;
            didScaleReset = true;
        }
        EditorGUILayout.EndHorizontal();

        // Apply changes with record undo
        if (didPositionChange || didRotationChange || didScaleChange)
        {
            Undo.RecordObject(_transform, _transform.name);

            if (didPositionChange)
                _transform.localPosition = localPosition;

            if (didRotationChange)
                _transform.localEulerAngles = localEulerAngles;

            if (didScaleChange)
                _transform.localScale = localScale;

        }

        if (didPositionReset || didRotationReset || didScaleReset)
        {
            Undo.RecordObject(_transform, _transform.name);

            if (didPositionReset)
                _transform.localPosition = Vector3.zero;

            if (didRotationReset)
                _transform.localEulerAngles = Vector3.zero;

            if (didScaleReset)
                _transform.localScale = Vector3.one;

        }

        // Since BeginChangeCheck only works on the selected object
        // we need to manually apply transform changes to all selected objects.
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length > 1)
        {
            foreach (var item in selectedTransforms)
            {
                if (didPositionChange || didRotationChange || didScaleChange)
                    Undo.RecordObject(item, item.name);

                if (didPositionChange)
                {
                    item.localPosition = ApplyChangesOnly(
                        item.localPosition, initialLocalPosition, _transform.localPosition);
                }

                if (didRotationChange)
                {
                    item.localEulerAngles = ApplyChangesOnly(
                        item.localEulerAngles, initialLocalEuler, _transform.localEulerAngles);
                }

                if (didScaleChange)
                {
                    item.localScale = ApplyChangesOnly(
                        item.localScale, initialLocalScale, _transform.localScale);
                }

            }
        }
    }

    private Vector3 ApplyChangesOnly(Vector3 toApply, Vector3 initial, Vector3 changed)
    {
        if (!Mathf.Approximately(initial.x, changed.x))
            toApply.x = _transform.localPosition.x;

        if (!Mathf.Approximately(initial.y, changed.y))
            toApply.y = _transform.localPosition.y;

        if (!Mathf.Approximately(initial.z, changed.z))
            toApply.z = _transform.localPosition.z;

        return toApply;
    }
}
#endif
