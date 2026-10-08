using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorTools.Views
{
    [Serializable]
    public class View3D
    {
        private const HideFlags ObjectHideFlags = HideFlags.DontSave;

        [SerializeField]
        private List<GameObject> m_Objects;
        [SerializeField]
        private GuiCamera m_Camera;

        public View3D()
        {
            m_Objects = new List<GameObject>();
            m_Camera = new GuiCamera();
        }

        public void Create()
        {
            m_Camera.Create();
            ControlGameObject(m_Camera.GameObject);
        }

        public void Release()
        {
            m_Camera.Release();

            foreach (var go in m_Objects)
            {
                go.DestroyObjectImmediate();
            }
        }

        public void AddGameObject(GameObject gameObject)
        {
            ControlGameObject(gameObject);
            m_Objects.Add(gameObject);
        }

        public void FocusCamera()
        {
            m_Camera.Focus(m_Objects);
        }

        public void OnGUI(Action onRender)
        {
            OnGUI(GUILayoutUtility.GetRect(64, 2048, 64, 2048), onRender);
        }

        public void OnGUI(Rect position, Action onRender)
        {
            if (m_Camera.IsValid == false)
            {
                m_Objects.Clear();
                Create();
            }

            m_Camera.OnGUI(position, m_Objects, onRender);
        }

        private void ControlGameObject(GameObject gameObject)
        {
            gameObject.SetLayerRecursively(GuiCamera.Layer);
            gameObject.hideFlags = ObjectHideFlags;
        }
    }
}
