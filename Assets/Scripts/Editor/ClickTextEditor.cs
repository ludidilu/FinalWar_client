/**
 * Author: 1901 (kaixuan1901@gmail.com)
 * Date: Feb 24, 2017 11:05
 */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ClickText))]
public class ClickTextEditor : UnityEditor.UI.TextEditor
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
