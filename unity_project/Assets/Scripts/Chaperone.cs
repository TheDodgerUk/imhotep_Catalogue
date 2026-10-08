using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Chaperone : MonoBehaviour
{
    [SerializeField]
    private float size = 0.05f;

    [SerializeField]
    private List<Vector4> Positions = new List<Vector4>();
    private string m_sGlobalParamName = "_ChaperoneInfluencers";

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_ChaperoneArrayLength", Positions.Count);
        Shader.SetGlobalVectorArray(m_sGlobalParamName, Positions);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < Positions.Count; i++)
        {
            Gizmos.DrawWireCube(Positions[i], Vector3.one * size);
        }
    }
}
