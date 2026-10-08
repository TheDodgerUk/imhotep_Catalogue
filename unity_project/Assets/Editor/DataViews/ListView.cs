using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Views
{
    public abstract class ListView<T> : IDataView
    {
        public event Func<T, List<T>> AddItem = null;
        public event Func<T, List<T>> RemoveItem = null;

        public event Action<T, T> ItemModified = null;
        public event Action ListModified = null;

        public BackgroundStyle Style = BackgroundStyle.None;
        public bool DrawModifyControls = true;
        public bool UseScrollView = false;
        public bool RenderIndices = false;
        public bool CanModifyList = true;
        public bool AlignButtons = true;

        public T Selected => m_List[m_SelectedIndex];
        public int SelectedIndex => m_SelectedIndex;

        protected virtual bool Spacing => false;

        protected Vector2 m_ScrollPos = Vector2.zero;
        protected bool m_IsFull = true;

        private readonly string m_Name = string.Empty;
        private bool m_Foldout = true;

        private int m_SelectedIndex = -1;

        private List<T> m_Filtered;
        private List<T> m_List;

        public ListView(string name, List<T> list)
        {
            m_List = list ?? new List<T>();
            m_Name = name;

            if (string.IsNullOrEmpty(m_Name) == true)
            {
                ToggleFoldout(true);
            }
            else if (m_List.Count == 0)
            {
                ToggleFoldout(false);
            }
        }

        public void ToggleFoldout(bool state) => m_Foldout = state;

        public void SetSelectedItem(T item) => SetSelectedIndex(m_List.IndexOf(item));
        public void SetSelectedIndex(int index) => m_SelectedIndex = index;
        public void ClearSelection() => SetSelectedIndex(-1);

        public virtual void OnGUI()
        {
            EditorGUIUtility.wideMode = true;

            // title bars and add button
            if (string.IsNullOrEmpty(m_Name) == false)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    m_Foldout = EditorGUILayout.Foldout(m_Foldout, $"{m_Name} ({m_List.Count})", true);
                    GUILayout.FlexibleSpace();

                    if (DrawModifyControls == true)
                    {
                        using (new GuiEnabledScope(m_IsFull && CanModifyList))
                        {
                            if (Utils.Gui.BtnUtility("+", true) == true)
                            {
                                OnAdd();
                            }
                        }
                    }
                }
            }
            // if collapsed, don't show
            if (m_Foldout == true)
            {
                if (m_List.Count > 0)
                {
                    // scroll view?
                    if (UseScrollView == true)
                    {
                        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                    }

                    m_Filtered = FilterList(m_List);
                    bool filtered = (m_Filtered != null);
                    var list = filtered ? m_Filtered : m_List;

                    // draw the list
                    for (int i = 0; i < list.Count; ++i)
                    {
                        DrawElement(list, ref i, !filtered);

                        if (Spacing == true)
                        {
                            EditorGUILayout.Space();
                        }
                    }

                    // scroll view?
                    if (UseScrollView == true)
                    {
                        EditorGUILayout.EndScrollView();
                    }
                }
                else
                {
                    string text = $"No {m_Name ?? typeof(T).Name} added.";
                    EditorGUILayout.HelpBox(text, MessageType.Info);
                }
            }
        }

        protected List<T> GetActiveList() => m_Filtered ?? m_List;
        protected List<T> GetList() => m_List;

        protected virtual List<T> FilterList(List<T> list) => null;

        private void DrawElement(List<T> list, ref int i, bool allowShifting)
        {
            bool isSelected = (m_SelectedIndex == i);
            var style = isSelected ? BackgroundStyle.Box : Style;
            using (new HorizontalStyleScope(style))
            {
                if(IsElementValid(list[i]) == false)
                {
                    return;
                }
                // render index in list if enabled
                if (RenderIndices == true)
                {
                    using (new LabelWidthScope(36))
                    {
                        EditorGUILayout.PrefixLabel(i.ToString());
                    }
                }

                // indent in
                if (string.IsNullOrEmpty(m_Name) == false)
                {
                    EditorGUI.indentLevel++;
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    // render element and fire callbacks if changed
                    var element = DrawElement(list[i]);
                    if (list[i].Equals(element) == false)
                    {
                        ItemModified?.Invoke(list[i], element);
                        list[i] = element;
                        ListModified?.Invoke();
                    }
                }

                // indent out
                if (string.IsNullOrEmpty(m_Name) == false)
                {
                    EditorGUI.indentLevel--;
                }

                if (DrawModifyControls == true)
                {
                    // shift element up
                    using (new GuiEnabledScope(CanModifyList && allowShifting && (i > 0)))
                    {
                        if (Utils.Gui.BtnUtility("↑", AlignButtons) == true)
                        {
                            if (isSelected == true)
                            {
                                m_SelectedIndex--;
                            }
                            m_List.Insert(i - 1, m_List[i]);
                            m_List.RemoveAt(i + 1);
                        }
                    }

                    // shift element down
                    using (new GuiEnabledScope(CanModifyList && allowShifting && (i < m_List.Count - 1)))
                    {
                        if (Utils.Gui.BtnUtility("↓", AlignButtons) == true)
                        {
                            if (isSelected == true)
                            {
                                m_SelectedIndex++;
                            }
                            m_List.Insert(i + 2, m_List[i]);
                            m_List.RemoveAt(i);
                        }
                    }

                    // remove element
                    using (new GuiEnabledScope(CanModifyList))
                    {
                        if (Utils.Gui.BtnRemove(AlignButtons) == true)
                        {
                            string name = "";
                            string guid = "";
                            if (list[i] is CatalogueEntry item)
                            {
                                name = item.FullName.ToString();
                                guid = item.Guid.ToString();
                            }
                            if (EditorUtility.DisplayDialog("Delete item", $"{name}", "Delete", "Cancel") == true)
                            {
                                if (isSelected == true)
                                {
                                    ClearSelection();
                                }

                                if (string.IsNullOrEmpty(guid) == false)
                                {
                                    string folderName = $"{Application.dataPath}/Artwork/CatalogueThumbnails/{guid}";
                                    if (Directory.Exists(folderName) == true)
                                    {
                                        Directory.Delete(folderName, true);
                                    }
                                }

                                Remove(list[i]);
                                --i;
                            }
                        }
                    }
                }
            }
        }

        protected abstract void OnAdd();

        protected abstract T DrawElement(T element);

        protected virtual bool IsElementValid(T element) => true;

        protected virtual void Add(T item)
        {
            m_Foldout = true;

            if (AddItem == null)
            {
                m_List.Add(item);
            }
            else
            {
                m_List = AddItem(item);
            }
            ListModified?.Invoke();
        }

        protected virtual void Remove(T item)
        {
            if (RemoveItem == null)
            {
                m_List.Remove(item);
            }
            else
            {
                m_List = RemoveItem(item);
            }
            ListModified?.Invoke();
        }
    }

    public abstract class DefaultListView<T> : ListView<T> where T : new()
    {
        public DefaultListView(string name, List<T> list) : base(name, list) { }

        protected override void OnAdd()
        {
            Add(new T());
        }
    }
}