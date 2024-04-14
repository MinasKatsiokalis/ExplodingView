#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MK.ExplodingView.Core;

namespace MK.ExplodingView.Editor
{   
    /// <summary>
    /// Editor window for fast integration of Exploding View package.
    /// </summary>
    public class ExplodingViewWindow : EditorWindow
    {   
        /// <summary>
        /// Array of transforms to be exploded.
        /// </summary>
        public Transform[] Transforms;

        private ExplodingViewComponent[] explodingViewComponents;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Exploding View")]
        public static void ShowWindow()
        {
            GetWindow<ExplodingViewWindow>("Exploding View");
        }

        private void OnEnable()
        {
            minSize = new Vector2(100, 100);
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space();

            GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel);
            textStyle.wordWrap = true;
            textStyle.richText = true;
            GUILayout.TextArea("Drag & drop any transform you would like to become an explodable object." +
                "Set their parameters from the Exploding View Component editor on each transform you selected.", textStyle);
            EditorGUILayout.Space();

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("Transforms");
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Exploding View Component"))
            {
                foreach (Transform transform in Transforms)
                    if (transform != null && transform.GetComponent<ExplodingViewComponent>() == null)
                        transform.gameObject.AddComponent<ExplodingViewComponent>();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Remove All Components"))
            {
                if (Transforms == null || Transforms.Length == 0)
                {
                    explodingViewComponents = FindObjectsOfType<ExplodingViewComponent>();
                    foreach (var item in explodingViewComponents)
                        DestroyImmediate(item);
                }
                else
                {
                    foreach (Transform transform in Transforms)
                        if (transform != null && transform.GetComponent<ExplodingViewComponent>() != null)
                            DestroyImmediate(transform.gameObject.GetComponent<ExplodingViewComponent>());
                }
            }
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
        }
    }
}
#endif