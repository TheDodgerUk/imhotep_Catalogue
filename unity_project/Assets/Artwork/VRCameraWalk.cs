using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCameraWalk : MonoBehaviour
{
     float m_Speed = 5;
     float m_RotateSpeed = 75;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.position += Camera.main.transform.forward * m_Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += -Camera.main.transform.forward * m_Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += -Camera.main.transform.right * m_Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Camera.main.transform.right * m_Speed * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Rotate(0, -m_RotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Rotate(0, m_RotateSpeed * Time.deltaTime, 0);
        }
    }
}
