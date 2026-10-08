using EditorTools.Views;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public abstract class ExplorerView<T> : ListView<T> where T : class
    {
        public event Action<T> DataSelected = null;
        public event Action Repaint = null;

        protected T m_Selected;

        protected bool m_DrawDisplay = true;
        protected bool m_DrawSize = true;

        public ExplorerView(string name, List<T> list, T selected = null) : base(name, list)
        {
            ExplorerViewSize.LoadStatics();

            UseScrollView = true;
            AlignButtons = false;
            SetSelected(selected);
        }

        public bool IsSelected(T selected) => (m_Selected == selected);
        public void SetSelected(T selected) => m_Selected = selected;
        public T GetSelected() => m_Selected;

        public sealed override void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, ExplorerViewSize.GetWidth()))
            {
                ExplorerViewSize.DrawHeaderControls(drawSize: m_DrawSize, drawDisplay: m_DrawDisplay);

                OnHeaderGUI();

                base.OnGUI();

                OnExplorerGUI();
            }

            ExecuteKeyboardControls();
        }

        protected virtual void OnHeaderGUI() { }

        protected abstract void OnExplorerGUI();

        protected void OnDataSelected(T element)
        {
            DataSelected?.Invoke(element);
        }

        protected GUIStyle GetButtonStyle(bool isSelected)
        {
            var style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, fontStyle  = FontStyle.Normal};
            if (isSelected == true)
            {
                style = new GUIStyle(style) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.BoldAndItalic  };
            }
            return style;
        }

        private void ExecuteKeyboardControls()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.KeyDown)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.UpArrow:
                        ChangeSelection(-1);
                        break;

                    case KeyCode.DownArrow:
                        ChangeSelection(+1);
                        break;

                    default:
                        break;
                }
            }
        }

        private void ChangeSelection(int delta)
        {
            var list = GetActiveList();
            int index = list.IndexOf(m_Selected);
            int newIndex = index + delta;
            var item = list.Get(newIndex);

            float height = EditorGUIUtility.singleLineHeight + 5;
            m_ScrollPos.y = height * newIndex;

            OnDataSelected(item);
            Repaint?.Invoke();
        }
    }
}