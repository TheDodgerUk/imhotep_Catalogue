using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class MuseumRoot : MonoBehaviour
{
    [SerializeField]
    private PlinthsManager m_PlinthsManager;
    private GameObject m_PlinthPrefab;
    public static List<PlinthInformation> m_PlinthInformation = new List<PlinthInformation>();


    void Awake()
    {
        Transform t = transform.Search("PlinthManager");
        m_PlinthsManager = t.gameObject.ForceComponent<PlinthsManager>();
        m_PlinthsManager.Initilise(m_PlinthPrefab);
    }
}


public class PlinthsManager : MonoBehaviour
{
    Vector3[] positionArray = new[]
    {
        new Vector3(3f, 0f, 3f),
        new Vector3(3f, 0f, -3f),
        new Vector3(-3f, 0f, 3f),
        new Vector3(-3f, 0f, -3f),
    };

    

    public void Initilise(GameObject prefab)
    {

    }
}


public class PlinthInformation : MonoBehaviour
{
    public GameObject m_DisplayPoint;
    private PlayableDirector m_PlayableDirector;
    private RenderTexture m_RenderTexture;
    private Camera m_Camera;
    private TextMeshPro m_TextMeshPro;

    public void Initilise(int index)
    {
        m_PlayableDirector = GetComponent<PlayableDirector>();
        m_DisplayPoint = this.transform.Search("DisplayPoint").gameObject;
        InitiliseRenderTexture(index);
    }

    private void OnTriggerEnter(Collider other)
    {
        m_PlayableDirector.Play();
    }

    private void InitiliseRenderTexture(int index)
    {
        m_RenderTexture = new RenderTexture(300, 300, 300); //TODO these numbers just made up 
        m_Camera = GetComponentInChildren<Camera>(true);
        m_Camera.targetTexture = m_RenderTexture;
        m_TextMeshPro = GetComponentInChildren<TextMeshPro>(true);
        m_TextMeshPro.text = $"test {index}";
        m_Camera.transform.position = new Vector3(0, (index + 1) * -100, 0); // -100 so there is no chance of collision


        GameObject gameObjectRef = this.transform.Search("TextRendererTarget").gameObject;
        Renderer rendererRef = gameObjectRef.GetComponent<Renderer>();
        rendererRef.material.mainTexture = m_RenderTexture;
    }

    public void SetScrollingText(string text)
    {
        m_Camera.gameObject.SetActive(true);
        m_TextMeshPro.text = text;
        this.WaitForFrames(2, () => // setting 2 frames for safety 
        {
            m_Camera.gameObject.SetActive(false);
        });
    }
}



