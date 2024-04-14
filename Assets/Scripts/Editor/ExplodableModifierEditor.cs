#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MK.ExplodingView.Core;
using MK.ExplodingView.Utils;

namespace MK.ExplodingView.Editor
{
    [CustomEditor(typeof(ExplodableModifier))]
    public class ExplodableModifierEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ExplodableModifier explodableModifier = (ExplodableModifier)target;

            EditorGUILayout.LabelField("Exploding Properties", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            explodableModifier.Axis = (ModifierAxis)EditorGUILayout.EnumPopup(new GUIContent("Axis", "Set this value if you want to make this part move in a specific direction based on its transform axes."), explodableModifier.Axis);
            int newOrder = EditorGUILayout.IntField(new GUIContent("Order", "Set the order in which this part is gonna move. Parts are moving from lower to higher order. By default all parts are in order 0."), (int)explodableModifier.Order);
            explodableModifier.Order = (uint)Mathf.Clamp(newOrder, 0, 10);
            explodableModifier.UseSelfDistance = EditorGUILayout.Toggle(new GUIContent("Use Custom Distance", "Set this value if you want to make this part move a specific distance, ignoring all the factors."), explodableModifier.UseSelfDistance);
            if (explodableModifier.UseSelfDistance)
                explodableModifier.Distance = EditorGUILayout.Slider("Custom Distance", explodableModifier.Distance, 0, 5);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif