#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MK.ExplodingView.Core;
using MK.ExplodingView.Utils;

namespace MK.ExplodingView.Editor
{
    [CustomEditor(typeof(ExplodingViewComponent))]
    public class ExplodingViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ExplodingViewComponent explodingViewComponent = (ExplodingViewComponent)target;

            EditorGUILayout.LabelField("Initialization Properties", EditorStyles.boldLabel);
            explodingViewComponent.Center = (Transform)EditorGUILayout.ObjectField(new GUIContent("Center", "Transform to count as the center of exploding view. If no Trasform is set, the center is calculated as the average of the mesh parts and has orientation based on the current game object."), explodingViewComponent.Center, typeof(Transform), true);
            explodingViewComponent.DirectionAxis = (Axis)EditorGUILayout.EnumPopup(new GUIContent("Direction Axis", "Direction of the line that the parts will explode around. This takes as reference the Center axes"), explodingViewComponent.DirectionAxis);
            explodingViewComponent.AddExplodablesAutomatically = EditorGUILayout.Toggle(new GUIContent("Add Explodables Automatically", "If enabled, the exploding parts will be calculated automatically. Otherwise, they have to be added manually by adding the ExplodablePart.cs component and drag-n-drop them in the Explodables list."), explodingViewComponent.AddExplodablesAutomatically);
            if(!explodingViewComponent.AddExplodablesAutomatically)
            {
                SerializedProperty explodablesProperty = serializedObject.FindProperty("Explodables");
                EditorGUILayout.PropertyField(explodablesProperty, true);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Exploding Properties", EditorStyles.boldLabel);
            explodingViewComponent.ExplosionSpeed = EditorGUILayout.FloatField(new GUIContent("Explosion Speed", "Speed of explosion in seconds."), explodingViewComponent.ExplosionSpeed);
            explodingViewComponent.ExplosionDistance = EditorGUILayout.Slider(new GUIContent("Explosion Distance", "Explosion distance factor."), explodingViewComponent.ExplosionDistance, 0f, 5f);
            explodingViewComponent.DistanceFactor = (DistanceFactor)EditorGUILayout.EnumPopup(new GUIContent("Distance Factor", "If enabled, the parts' exploded position will be relative to their distance from 'Center' point or from 'Axis' that selected."), explodingViewComponent.DistanceFactor);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Additional Factors", EditorStyles.boldLabel);
            explodingViewComponent.AddScaleFactor = EditorGUILayout.Toggle(new GUIContent("Add Scale Factor", "If enabled, the mesh size of the part will add a factor to the final position."), explodingViewComponent.AddScaleFactor);
            if(explodingViewComponent.AddScaleFactor)
                explodingViewComponent.ScaleFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Scale Factor Multiplier", ""), explodingViewComponent.ScaleFactorMultiplier, 0f, 5f);
            explodingViewComponent.AddHierarchyFactor = EditorGUILayout.Toggle(new GUIContent("Add Hierarchy Factor", "If enabled, the hierarchy index of the part will add a factor to the final position."), explodingViewComponent.AddHierarchyFactor);
            if(explodingViewComponent.AddHierarchyFactor)
                explodingViewComponent.HierarchyFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Hierarchy Factor Multiplier", ""), explodingViewComponent.HierarchyFactorMultiplier, 0f, 5f);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            SerializedProperty drawDirectionAxisProperty = serializedObject.FindProperty("DrawDirectionAxis");
            EditorGUILayout.PropertyField(drawDirectionAxisProperty, new GUIContent("Draw Direction Axis.", ""));
            EditorGUILayout.Space();

            if (GUILayout.Button("Exploding View"))
                explodingViewComponent.ExplodingView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Recalculate"))
                explodingViewComponent.CalculateExplodingParameters();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif