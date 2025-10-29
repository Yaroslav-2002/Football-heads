#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ViewManager))]
public class ViewManagerEditor : Editor
{
    private SerializedProperty startingViewProperty;
    private SerializedProperty viewsProperty;

    private void OnEnable()
    {
        startingViewProperty = serializedObject.FindProperty("startingView");
        viewsProperty = serializedObject.FindProperty("views");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(startingViewProperty, new GUIContent("Starting View"));
        EditorGUILayout.Space();

        DrawViewsList();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawViewsList()
    {
        if (viewsProperty == null)
        {
            return;
        }

        viewsProperty.isExpanded = EditorGUILayout.Foldout(viewsProperty.isExpanded, "Registered Views", true);
        if (!viewsProperty.isExpanded)
        {
            return;
        }

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(viewsProperty.FindPropertyRelative("Array.size"));

        for (int i = 0; i < viewsProperty.arraySize; i++)
        {
            SerializedProperty element = viewsProperty.GetArrayElementAtIndex(i);
            View view = element.objectReferenceValue as View;
            string label = view != null
                ? $"{view.name} ({view.GetType().Name})"
                : $"Element {i}";

            EditorGUILayout.PropertyField(element, new GUIContent(label));
        }

        EditorGUI.indentLevel--;
    }
}
#endif
