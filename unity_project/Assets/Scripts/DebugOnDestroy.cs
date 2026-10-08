using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnDestroy : MonoBehaviour
{

    private void OnDisable()
    {
        Debug.LogError("OnDisable");
    }

    private void OnDestroy()
    {
        Debug.LogError("OnDestroy");
    }
}
