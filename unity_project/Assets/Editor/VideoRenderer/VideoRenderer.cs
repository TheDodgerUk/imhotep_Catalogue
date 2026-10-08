using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VideoRenderer
{
    [MenuItem("Video Production/Run Catalogue Videos")]
    public static void RenderCatalogueVideos()
    {
        Debug.Log("Starting video Renders");

        //Load scene
        EditorSceneManager.OpenScene("Assets/Artwork/Scenes/Environments/VideoRenderer/VideoRenderer.unity", OpenSceneMode.Single);

        //Play scene
        EditorApplication.isPlaying = true;
    }
}