using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionSaveLoad : MonoBehaviour
{
    public Dictionary<string, Tuple<Vector3, quaternion>> m_Positions = new Dictionary<string, Tuple<Vector3, quaternion>>();

    public void SavePositions()
    {
        m_Positions.Clear();

        foreach (GameObject rootGameobject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (Transform child in rootGameobject.GetComponentsInChildren<Transform>(true))
            {
                if (true == child.name.StartsWith("Position_"))
                {
                    if (false == m_Positions.ContainsKey(child.name))
                    {
                        m_Positions.Add(child.name, new Tuple<Vector3, quaternion>(child.position, child.rotation));
                    }
                    else
                    {
                        Debug.LogError($"There is already a gameobject with the name {child.name}");
                    }
                }
            }
        }

        // Write these to a JSON file.
        Json.JsonNet.WriteToFile(m_Positions, Application.persistentDataPath + "/position", true);

        Debug.Log("File saved to: " + Application.persistentDataPath);
    }


    public void CreateObjectsFromData()
    {
        if (false == File.Exists(Application.persistentDataPath + "/position.json"))
        {
            Debug.LogError("Cannot find data file at : " + Application.persistentDataPath + "/position.json");
            return;
        }

        m_Positions = Json.JsonNet.ReadFromFile(Application.persistentDataPath + "/position", typeof(Dictionary<string, Tuple<Vector3, quaternion>>)) as Dictionary<string, Tuple<Vector3, quaternion>>;

        GameObject parentObject = new GameObject("Positions");
        GameObject childObject;

        foreach (var item in m_Positions)
        {
            childObject = new GameObject(item.Key);
            childObject.transform.position = item.Value.A;
            childObject.transform.rotation = item.Value.B;
            childObject.transform.SetParent(parentObject.transform, true);
        }

        Debug.Log($"Created {m_Positions.Count} items from data.");
    }
}
