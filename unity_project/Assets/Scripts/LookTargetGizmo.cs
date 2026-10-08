using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "ABHead.png", true);
    }
}
