using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EmptySkull.Utilities
{
    public class RunLastBuildEditorExtension : MonoBehaviour
    {
        private static string LastBuildEditorPrefsKey =>
            $"{Application.companyName}:{Application.productName}:LastBuildPath";

        public static string LastBuildPath
        {
            get => EditorPrefs.GetString(LastBuildEditorPrefsKey, string.Empty);
            private set => EditorPrefs.SetString(LastBuildEditorPrefsKey, value);
        }

        [MenuItem("File/Run last build")]
        public static void RunLastBuild() //Called by Unity
            => new Process {StartInfo = {FileName = LastBuildPath}}.Start();

        [MenuItem("File/Run last build", true)]
        public static bool RunLastBuildValidation() //Called by Unity
            => !string.IsNullOrEmpty(LastBuildPath) && File.Exists(LastBuildPath);


        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget _, string pathToBuiltProject) //Called by Unity
            => LastBuildPath = pathToBuiltProject;
    }
}