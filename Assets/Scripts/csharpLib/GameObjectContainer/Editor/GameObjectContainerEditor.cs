using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameObjectContainer))]
public class GameObjectContainerEditor : Editor
{
    GameObjectContainer ObjectContainer { get { return (GameObjectContainer)target; } }

    ReorderableList m_layerList;

    void OnEnable()
    {

    }

    private void _LayerList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        float savedLabelWidth = EditorGUIUtility.labelWidth;
        var element = m_layerList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float elemWidth = 180;
        float elemX = rect.x;

        EditorGUI.PropertyField(
            new Rect(elemX, rect.y, elemWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("name"), GUIContent.none);

        elemX += elemWidth;
        elemWidth = 180;

        EditorGUI.PropertyField(
            new Rect(elemX, rect.y, elemWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("value"), GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        if (m_layerList == null)
        {
            m_layerList = new ReorderableList(serializedObject, serializedObject.FindProperty("Entitys"), true, true, true, true);
            m_layerList.drawElementCallback += _LayerList_DrawElementCallback;
            m_layerList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Entitys");
            };

            m_layerList.onChangedCallback = (ReorderableList list) =>
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            };

            m_layerList.onAddCallback = (ReorderableList list) =>
            {
                ObjectContainer.Add();
            };
        }

        m_layerList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        Repaint();
    }

}
