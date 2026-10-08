using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Recticle : MonoBehaviour

{

    public Transform cursorObject;
    public float distance = 1.5f;

    void Start()
    {
        //Cursor.visible = false; 
    }

    void Update()
    {
        //ObjectFollowCursor();
    }

    void ObjectFollowCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point = ray.origin + (ray.direction * distance);

        cursorObject.position = point;
    }
}
