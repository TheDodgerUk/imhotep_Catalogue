using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class UnlitTransparentTextureColourEditor : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // setup display to render as default
        materialEditor.SetDefaultGUIWidths();

        // grab custom properties
        var srcBlend = GetProperty(properties, "_SrcBlend");
        var dstBlend = GetProperty(properties, "_DstBlend");
        var zWrite = GetProperty(properties, "_ZWrite");
        var zTest = GetProperty(properties, "_ZTest");
        var cull = GetProperty(properties, "_Cull");

        // draw default properties
        var controlledProperties = new[] { srcBlend, dstBlend, zWrite, zTest, cull }.ToList();
        foreach (var property in properties)
        {
            if (controlledProperties.Contains(property) == false)
            {
                materialEditor.ShaderProperty(property, property.displayName);
            }
        }

        using (new LabelWidthScope(180f))
        {
            EnumPopup<BlendMode>(srcBlend);
            EnumPopup<BlendMode>(dstBlend);
            Toggle(zWrite);
            EnumPopup<CompareFunction>(zTest);
            EnumPopup<CullMode>(cull);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        materialEditor.RenderQueueField();
        materialEditor.EnableInstancingField();
        materialEditor.DoubleSidedGIField();
    }

    private static void Toggle(MaterialProperty property)
    {
        bool boolValue = property.floatValue == 1;
        boolValue = EditorGUILayout.Toggle(property.displayName, boolValue);
        property.floatValue = boolValue ? 1 : 0;
    }

    private static void EnumPopup<T>(MaterialProperty property)
    {
        int floatValue = (int)property.floatValue;
        var enumValue = (T)(object)floatValue;

        enumValue = EditorHelper.EnumPopup(property.displayName, enumValue);
        property.floatValue = (int)(object)enumValue;
    }

    private static int GetInt(Material material, string propertyName, int defaultValue)
    {
        if (material.HasProperty(propertyName) == true)
        {
            return material.GetInt(propertyName);
        }
        return defaultValue;
    }

    private static MaterialProperty GetProperty(MaterialProperty[] properties, string propertyName)
    {
        foreach (var property in properties)
        {
            if (property.name == propertyName)
            {
                return property;
            }
        }
        return null;
    }
}
