#if UNITY_EDITOR
#if CATALOG_PROGRAM
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RaycastTriggerValidation : BaseValidation
{
    protected override string TAG => $"[RaycastTrigger]";

    public float m_MaxDistance = 1f;
    public bool m_UseMaxDistance = false;
   

    [InspectorButton]
    protected override void SetCorrectStates()
    {
        string taggedName = "";
        EnvironmentHelpers.Helpers.GetNamedTagged(this.gameObject, ref taggedName);
        m_TriggerName = EnvironmentHelpers.Helpers.GetObjectName(this.gameObject);
        float.TryParse(EnvironmentHelpers.Helpers.GetObjectNameExtentionArguments(this.gameObject, 0), out m_MaxDistance);
        bool.TryParse(EnvironmentHelpers.Helpers.GetObjectNameExtentionArguments(this.gameObject, 1), out m_UseMaxDistance);
    }

    [InspectorButton]
    protected override void CreateGameObjectNameAndAssign()
    {
        if (true == ValidateGameObjectName())
        {
            gameObject.name = $"{TAG}_{m_TriggerName}_{m_MaxDistance.ToString()}_{m_UseMaxDistance.ToString()}";
        }
    }

    protected override bool ValidateGameObjectName()
    {
        if (true == m_TriggerName.Contains("_"))
        {
            EditorUtility.DisplayDialog("m_TriggerName", "it contains  _", "Ok");
            return false;
        }

        return true;
    }

}
#endif
#endif
