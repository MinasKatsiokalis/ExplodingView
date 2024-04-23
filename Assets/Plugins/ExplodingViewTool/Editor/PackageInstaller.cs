#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MK.ExplodingView.Editor
{
    [InitializeOnLoad]
    public class PackageInstaller : EditorWindow
    {
        static AddRequest Request;

        private void OnEnable()
        {
            minSize = new Vector2(100, 100);
        }

        [InitializeOnLoadMethod]
        public static void InitOnLoad()
        {
            EditorApplication.delayCall += ShowWindow;
        }

        [MenuItem("Tools/Exploding View/Packages")]
        public static void ShowWindow()
        {
            GetWindow<PackageInstaller>("Package Installer");
        }

        private void OnGUI()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel);
            textStyle.wordWrap = true;
            textStyle.richText = true;

            EditorGUILayout.TextArea("UniTask package is used in core components of Exploding View Tool.", textStyle);
            if (GUILayout.Button("Install UniTask (Mandatory)"))
            {
                InstallUniTaskPackage();
            }
            EditorGUILayout.Space();

            EditorGUILayout.TextArea("Unity glTFast is an glTF/GLB format importer, used in sample scenes.", textStyle);
            if (GUILayout.Button("Install Unity glTFast"))
            {
                InstallGlTFastPackage();
            }
            EditorGUILayout.Space();
        }

        public static void InstallUniTaskPackage()
        {
            Debug.Log("Installing UniTask package...");
            Request = Client.Add("https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
            EditorApplication.update += Progress;
        }

        public static void InstallGlTFastPackage()
        {
            Debug.Log("Installing Unity glTFast package...");
            Request = Client.Add("com.unity.cloud.gltfast");
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + Request.Result.packageId);
                    if (Request.Result.packageId.StartsWith("com.cysharp.unitask"))
                        AddDefineSymbol("UNITASK");
                }
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }

        public static void AddDefineSymbol(string symbol)
        {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            if (!defines.Contains(symbol))
            {
                defines += $";{symbol}";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
            }
        }
    }
}
#endif