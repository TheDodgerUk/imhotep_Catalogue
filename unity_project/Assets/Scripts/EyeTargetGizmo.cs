using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTargetGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "EyeIcon.png", true);
    }
}
