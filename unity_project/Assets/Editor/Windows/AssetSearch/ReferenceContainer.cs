using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ReferenceContainer : ListContainer<AssetReference>
{
    private List<AssetReferenceGroup> m_Groups;
    private List<Object> m_MissingTargets;

    private Vector2 m_MissingScrollPos;
    private bool m_MissingFoldout;
    private bool m_GroupsFoldout;

    public void BuildGroups(List<Object> targets)
    {
        m_Groups = new List<AssetReferenceGroup>();
        var distinctList = List.GroupBy(r => r.Asset).Select(g => g.First());
        foreach (var reference in distinctList)
        {
            m_Groups.Add(new AssetReferenceGroup(reference.Asset, List));
        }
        m_Groups.Sort((x, y) => Comparisons.AlphaNumericByName(x.Asset, y.Asset));

        m_MissingTargets = targets.FindAll(t => !m_Groups.Exists(g => g.Asset == t));
    }

    public override void Draw(string title)
    {
        base.Draw(title);

        if (m_Groups != null)
        {
            DrawGroups();

            if (m_MissingTargets != null)
            {
                base.Draw("Missing");
                DrawMissingTargets();
            }
        }
    }

    private bool DrawFoldout(ref bool foldout)
    {
        foldout = EditorGUILayout.Foldout(foldout, foldout ? "Hide" : "Show", true);
        return foldout;
    }

    private void DrawGroups()
    {
        if (DrawFoldout(ref m_GroupsFoldout) == true)
        {
            if (GUILayout.Button("Select All") == true)
            {
                Selection.objects = m_Groups.Extract(g => g.Asset).ToArray();
            }

            ScrollPos = GUILayout.BeginScrollView(ScrollPos);
            {
                using (new LabelWidthScope(100))
                {
                    using (new GuiIndentScope())
                    {
                        var btnWidth = GUILayout.Width(100);
                        foreach (var group in m_Groups)
                        {
                            DrawAsset(btnWidth, group);
                            if (group.Foldout == true)
                            {
                                using (new GuiIndentScope())
                                {
                                    foreach (var reference in group.References)
                                    {
                                        DrawPath(btnWidth, reference.Path);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }
    }

    private void DrawMissingTargets()
    {
        if (DrawFoldout(ref m_MissingFoldout) == true)
        {
            if (GUILayout.Button("Select All") == true)
            {
                Selection.objects = m_MissingTargets.ToArray();
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(m_MissingScrollPos))
            {
                using (new LabelWidthScope(100))
                {
                    var btnWidth = GUILayout.Width(100);
                    using (new GuiIndentScope())
                    {
                        foreach (var target in m_MissingTargets)
                        {
                            if (m_Groups.Exists(g => g.Asset == target) == false)
                            {
                                DrawAsset(btnWidth, target, false);
                            }
                        }
                    }
                }
                m_MissingScrollPos = scroll.scrollPosition;
            }
        }
    }

    private void DrawAsset(GUILayoutOption btnWidth, AssetReferenceGroup group)
    {
        group.Foldout = DrawAsset(btnWidth, group.Asset, group.Foldout);
    }

    private bool DrawAsset(GUILayoutOption btnWidth, Object obj, bool foldout)
    {
        GUILayout.BeginHorizontal();
        {
            using (new LabelWidthScope(0))
            {
                string name = obj.GetType().Name;
                foldout = EditorGUILayout.Foldout(foldout, name, true);
                EditorGUILayout.ObjectField(obj, obj.GetType(), false);
            }

            if (GUILayout.Button("Ping Asset", btnWidth) == true)
            {
                EditorGUIUtility.PingObject(obj);
            }
        }
        GUILayout.EndHorizontal();

        return foldout;
    }

    private void DrawPath(GUILayoutOption btnWidth, string path)
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Path", path);

            var found = GameObject.Find(path);
            GUI.enabled = (found != null);

            if (GUILayout.Button("Ping Object", btnWidth) == true)
            {
                EditorGUIUtility.PingObject(found);
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
    }
}
