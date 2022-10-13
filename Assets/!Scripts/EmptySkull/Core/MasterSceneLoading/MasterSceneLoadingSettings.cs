#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    public class MasterSceneLoadingSettings : ScriptableObject
    {
        public enum SceneSaveBehaviour
        {
            DontSave,
            PromptSave,
            AlwaysSave
        }

        public static string SettingsPath => "Assets/Editor/MasterSceneLoading/MasterSceneLoadingSettings.asset";

        [SerializeField] private bool _isActivated;
        [SerializeField] private bool _keepMasterInEditor;
        [SerializeField] private SceneSaveBehaviour _sceneSaveBehaviour = SceneSaveBehaviour.PromptSave;

        public bool IsActivated => _isActivated;
        public bool KeepMasterInEditor => _keepMasterInEditor;
        public SceneSaveBehaviour SaveBehaviour => _sceneSaveBehaviour;

        internal static MasterSceneLoadingSettings GetOrCreateSettings()
        {
            MasterSceneLoadingSettings settings = AssetDatabase.LoadAssetAtPath<MasterSceneLoadingSettings>(SettingsPath);
            if (settings == null)
            {
                TryCreateSettingsAsset(out settings);
            }
            return settings;
        }

        internal static bool TryCreateSettingsAsset()
            => TryCreateSettingsAsset(out _);

        internal static bool TryCreateSettingsAsset(out MasterSceneLoadingSettings settings)
        {
            try
            {
                settings = CreateInstance<MasterSceneLoadingSettings>();

                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[{nameof(MasterSceneLoadingSettings)}] Exception caught: \n{e.Message}");
                settings = null;
                return false;
            }
            return true;
        }

        internal static SerializedObject GetSerializedSettings()
            => new SerializedObject(GetOrCreateSettings());
    }

    class MasterSceneLoadingSettingsProvider : SettingsProvider
    {
        private SerializedObject _settings;

        static string SettingsPath => MasterSceneLoadingSettings.SettingsPath;

        public MasterSceneLoadingSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
            => File.Exists(SettingsPath);


        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = MasterSceneLoadingSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            SerializedProperty activeProp = _settings.FindProperty("_isActivated");
            SerializedProperty keepInEditorProp = _settings.FindProperty("_keepMasterInEditor");
            SerializedProperty sceneSaveBehaviourProp = _settings.FindProperty("_sceneSaveBehaviour");

            activeProp.serializedObject.Update();

            bool canChangeActivation = true;
            if (!CanBeActivated(out string masterSceneName))
            {
                EditorGUILayout.HelpBox("You need to have a valid scene at build index '0' to be used as a master scene!", MessageType.Warning);
                if (activeProp.boolValue)
                    activeProp.boolValue = false;
                canChangeActivation = false;
            }

            bool isActivated = activeProp.boolValue;

            Color cOld = GUI.color;
            GUI.color = isActivated ? Color.green : Color.red;
            {
                bool enabeldStateOld = GUI.enabled;
                GUI.enabled = canChangeActivation;
                {
                    if (GUILayout.Button(!isActivated ? "Activate" : "Deactivate"))
                        activeProp.boolValue = !isActivated;
                }
                GUI.enabled = enabeldStateOld;
            }
            GUI.color = cOld;


            string masterSceneNameDisplay = masterSceneName + (isActivated ? string.Empty : " (inactive)");
            string masterSceneDisplay = string.IsNullOrWhiteSpace(masterSceneName) ? "---" : masterSceneNameDisplay;
            EditorGUILayout.LabelField($"Master Scene: {masterSceneDisplay}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(keepInEditorProp);
            EditorGUILayout.PropertyField(sceneSaveBehaviourProp);

            activeProp.serializedObject.ApplyModifiedProperties();

            bool CanBeActivated(out string masterSceneName)
            {
                EditorBuildSettingsScene zeroScene = EditorBuildSettings.scenes == null || EditorBuildSettings.scenes.Length <= 0 ? null : EditorBuildSettings.scenes[0];
                if (zeroScene == null)
                {
                    masterSceneName = string.Empty;
                    return false;
                }

                masterSceneName = Path.GetFileNameWithoutExtension(zeroScene.path);
                return true;
            }
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMasterSceneLoaderSettingsProvider()
        {
            if (!IsSettingsAvailable())
                MasterSceneLoadingSettings.TryCreateSettingsAsset();

            if (IsSettingsAvailable())
            {
                MasterSceneLoadingSettingsProvider provider = new MasterSceneLoadingSettingsProvider("Project/EmptySkull/Master Scene Loading", SettingsScope.Project);

                // Automatically extract all keywords from the Styles.
                // provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>(); //TODO Reimplement keywords
                return provider;
            }

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
    }
}

#endif