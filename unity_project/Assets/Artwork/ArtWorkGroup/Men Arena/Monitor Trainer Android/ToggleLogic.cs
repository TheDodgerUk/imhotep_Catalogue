using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ToggleLogic : MonoBehaviour
{
    public void SetBackground(bool bIsToggleOn)
    {
        this.gameObject.SetActive(!bIsToggleOn);
    }
}