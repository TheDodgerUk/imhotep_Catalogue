using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Views
{
    [Serializable]
    public class GuiCamera
    {
        public static int Layer => LayerMask.NameToLayer("EditorView3D");

        public bool IsValid => m_Camera != null && m_Texture != null;
        public GameObject GameObject => m_Camera.gameObject;

        [SerializeField]
        private GuiCameraController m_Controller;
        [SerializeField]
        private RenderTexture m_Texture;
        [SerializeField]
        private Camera m_Camera;

        public void Create()
        {
            Utils.Editor.ToggleVisibleLayer(Layer, false);
            CreateRenderTarget(128, 128);

            var bgColour = new Color(0.760784f, 0.760784f, 0.760784f, 1);
            m_Camera = Utils.Unity.Create<Camera>();
            m_Camera.stereoTargetEye = StereoTargetEyeMask.None;
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.backgroundColor = bgColour.linear;
            m_Camera.cullingMask = (1 << Layer);
            m_Camera.targetTexture = m_Texture;
            m_Camera.nearClipPlane = 0.01f;
            m_Camera.AddComponent<GLRender>();
            m_Camera.enabled = false;

            m_Controller = new GuiCameraController();
            m_Controller.Create(m_Camera);

            var light = m_Camera.transform.CreateChild<Light>();
            light.type = LightType.Directional;
            light.cullingMask = (1 << Layer);
            light.intensity = 1;
            light.transform.Reset();
        }

        public void Focus(List<GameObject> objects)
        {
            m_Controller.FocusCamera(objects);
        }

        public void Release()
        {
            m_Camera?.gameObject.DestroyObjectImmediate();
            m_Texture?.Release();
        }

        public void OnGUI(Rect position, List<GameObject> objects, Action onRender)
        {
            // draw last frame's output
            GUI.DrawTexture(position, m_Camera.activeTexture, ScaleMode.ScaleToFit);

            if (Event.current.type == EventType.Repaint)
            {
                if (m_Texture.width != position.width || m_Texture.height != position.height)
                {
                    CreateRenderTarget(position.width, position.height);
                    m_Camera.targetTexture = m_Texture;
                }
            }

            m_Controller.OnGUI(position, objects);

            //if (Event.current.type == EventType.Repaint)
            {
                onRender();
                m_Camera.Render();
            }
        }

        private void CreateRenderTarget(float w, float h)
        {
            if (m_Texture != null)
            {
                m_Texture.Release();
            }
            const RenderTextureReadWrite Linear = RenderTextureReadWrite.Linear;
            const RenderTextureFormat Format = RenderTextureFormat.ARGB32;
            m_Texture = new RenderTexture((int)w, (int)h, 24, Format, Linear)
            {
                name = DateTime.Now.ToFileTime().ToString()
            };
            m_Texture.Create();
        }
    }
}
