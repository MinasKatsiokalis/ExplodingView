#if UNITY_EDITOR
using UnityEngine;
using MK.ExplodingView.Core;
using MK.ExplodingView.Utils;
using UnityEditor;

namespace MK.ExplodingView.Editor
{
    [CustomEditor(typeof(ExplodingViewComponent))]
    public class ExplodingViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ExplodingViewComponent explodingViewComponent = (ExplodingViewComponent)target;

            EditorGUILayout.LabelField("Explodable Parts", EditorStyles.boldLabel);
            explodingViewComponent.AddExplodablesAutomatically = EditorGUILayout.Toggle(new GUIContent("Add Explodables Automatically", "If enabled, the exploding parts will be calculated automatically. Otherwise, they have to be added manually by adding the ExplodablePart.cs component and drag-n-drop them in the Explodables list."), explodingViewComponent.AddExplodablesAutomatically);
            if (!explodingViewComponent.AddExplodablesAutomatically)
            {
                SerializedProperty explodablesProperty = serializedObject.FindProperty("Explodables");
                EditorGUILayout.PropertyField(explodablesProperty, true);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Initialization Properties", EditorStyles.boldLabel);
            explodingViewComponent.Center = (Transform)EditorGUILayout.ObjectField(new GUIContent("Center", "Transform to count as the center of exploding view. If no Trasform is set, the center is calculated as the average of the mesh parts and has orientation based on the current game object."), explodingViewComponent.Center, typeof(Transform), true);
            explodingViewComponent.Direction = (Direction)EditorGUILayout.EnumPopup(new GUIContent("Direction", "The direction in which the parts should explode to."), explodingViewComponent.Direction);
            if(explodingViewComponent.Direction == Direction.FromPlane || explodingViewComponent.Direction == Direction.FromAxis)
                explodingViewComponent.NormalAxis = (Axis)EditorGUILayout.EnumPopup(new GUIContent("Normal Axis", "The normal that will be used in case of Plane or Axis direction. This is relative to Center transform."), explodingViewComponent.NormalAxis);
            explodingViewComponent.MoveOnLocalAxisOnly = EditorGUILayout.Toggle(new GUIContent("Move On Local Axis", "If enabled, the parts will explode only in directions based on their local transforms (X,Y,Z)"), explodingViewComponent.MoveOnLocalAxisOnly);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Exploding Speed", EditorStyles.boldLabel);
            explodingViewComponent.ExplosionSpeed = EditorGUILayout.FloatField(new GUIContent("Explosion Speed", "Speed of explosion in seconds."), explodingViewComponent.ExplosionSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Distance Factor", EditorStyles.boldLabel);
            explodingViewComponent.DistanceFactor = (DistanceFactor)EditorGUILayout.EnumPopup(new GUIContent("Distance Factor", "Explosion distance factor. Distance from projection point is applicable only when Axis or Plane has been selected as direction origin."), explodingViewComponent.DistanceFactor);
            if(explodingViewComponent.DistanceFactor == DistanceFactor.StaticDistance)
                explodingViewComponent.StaticDistanceReference = (StaticDistanceReference)EditorGUILayout.EnumPopup(new GUIContent("Static Reference", "Relative to which point should the static distance be applied."), explodingViewComponent.StaticDistanceReference);
            explodingViewComponent.DistanceFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Distance Factor Multiplier", "Multiplier for the distance factor."), explodingViewComponent.DistanceFactorMultiplier, 0f, 5f);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Additional Factors", EditorStyles.boldLabel);
            explodingViewComponent.AddScaleFactor = EditorGUILayout.Toggle(new GUIContent("Add Scale Factor", "If enabled, the mesh size of the part will add a factor to the final position."), explodingViewComponent.AddScaleFactor);
            if (explodingViewComponent.AddScaleFactor)
            {
                explodingViewComponent.ScaleFactor = (ScaleFactor)EditorGUILayout.EnumPopup(new GUIContent("Scale Factor", "Determines how the scale of the part affects the final position."), explodingViewComponent.ScaleFactor);
                explodingViewComponent.ScaleFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Scale Factor Multiplier", ""), explodingViewComponent.ScaleFactorMultiplier, 0f, 5f);
            }
            EditorGUILayout.Space();

            explodingViewComponent.AddHierarchyFactor = EditorGUILayout.Toggle(new GUIContent("Add Hierarchy Factor", "If enabled, the hierarchy index of the part will add a factor to the final position."), explodingViewComponent.AddHierarchyFactor);
            if(explodingViewComponent.AddHierarchyFactor)
                explodingViewComponent.HierarchyFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Hierarchy Factor Multiplier", ""), explodingViewComponent.HierarchyFactorMultiplier, 0f, 5f);
            EditorGUILayout.Space();

            explodingViewComponent.AddSiblingFactor = EditorGUILayout.Toggle(new GUIContent("Add Sibling Factor", "If enabled, the sibling index of the part will add a factor to the final position."), explodingViewComponent.AddSiblingFactor);
            if (explodingViewComponent.AddSiblingFactor)
                explodingViewComponent.SiblingFactorMultiplier = EditorGUILayout.Slider(new GUIContent("Sibling Factor Multiplier", ""), explodingViewComponent.SiblingFactorMultiplier, 0f, 5f);
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