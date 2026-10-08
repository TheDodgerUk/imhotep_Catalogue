using UnityEditor;
using UnityEngine;

namespace EditorTools.Views
{
    public class UpdateWindow : EditorWindow, ISerializationCallbackReceiver
    {
        protected virtual void Awake()
        {
            EditorApplication.update += OnUpdate;
        }

        protected virtual void OnDestroy()
        {
            EditorApplication.update -= OnUpdate;
        }

        public void OnBeforeSerialize()
        {
            EditorApplication.update -= OnUpdate;
        }

        public void OnAfterDeserialize()
        {
            EditorApplication.update += OnUpdate;
        }

        protected virtual void OnUpdate()
        {
            Repaint();
        }
    }
}
