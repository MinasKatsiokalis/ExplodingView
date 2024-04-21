#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MK.ExplodingView.Core;
using MK.ExplodingView.Utils;

namespace MK.ExplodingView.Editor
{
    [CustomEditor(typeof(ExplodableModifier))]
    [CanEditMultipleObjects]
    public class ExplodableModifierEditor : UnityEditor.Editor
    {
        SerializedProperty orderProp;
        SerializedProperty modifyProp;
        SerializedProperty axisProp;
        SerializedProperty localPositionProp;
        SerializedProperty affectChildrenProp;

        void OnEnable()
        {
            orderProp = serializedObject.FindProperty("Order");
            modifyProp = serializedObject.FindProperty("ModifierProperty");
            axisProp = serializedObject.FindProperty("Axis");
            localPositionProp = serializedObject.FindProperty("LocalPosition");
            affectChildrenProp= serializedObject.FindProperty("AffectChildren");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string key = "ExplodableModifierData_" + target.GetInstanceID();

            EditorGUILayout.LabelField("Exploding Properties", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(orderProp, new GUIContent("Order", "Set the order in which this part is gonna move. Parts are moving from higher to lower order in explosion, and reverse in shrinkage. By default all parts are in order 0."));
            EditorGUILayout.PropertyField(modifyProp, new GUIContent("Modifier Property", "Select the property to modify."));

            switch ((ModifierProperty)modifyProp.enumValueIndex)
            {
                case ModifierProperty.Axis:
                    EditorGUILayout.PropertyField(axisProp, new GUIContent("Axis", "Set the axis for the explosion."));
                    EditorGUILayout.PropertyField(affectChildrenProp, new GUIContent("Affect Children", "Modifies the explosion axis of all children."));
                    break;
                case ModifierProperty.LocalPosition:
                    EditorGUILayout.PropertyField(localPositionProp, new GUIContent("Local Position", "Set the local position for the explosion."));
                    break;
                default:
                    break;
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Save Changes", "Save the current state of the ExplodableModifier to EditorPrefs.")))
            {
                string data = EditorJsonUtility.ToJson(target);
                EditorPrefs.SetString(key, data);
            }

            if (GUILayout.Button(new GUIContent("Load Changes", "Load the saved state from EditorPrefs and apply it to the ExplodableModifier.")))
            {
                string data = EditorPrefs.GetString(key, "");
                if (!string.IsNullOrEmpty(data))
                {
                    EditorJsonUtility.FromJsonOverwrite(data, target);
                }
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif