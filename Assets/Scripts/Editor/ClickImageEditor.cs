using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ClickImage))]
public class ClickImageEditor : UnityEditor.UI.ImageEditor
{
    private SerializedProperty styleProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        styleProp = serializedObject.FindProperty("clickKey");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(styleProp, new GUIContent("clickKey"));
        serializedObject.ApplyModifiedProperties();
    }
}
