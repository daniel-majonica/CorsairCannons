#if UNITY_EDITOR

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    [InitializeOnLoad]
    public static class MasterSceneLoader
    {
        private static MasterSceneLoadingSettings _masterSceneSettingsValue;
        private static MasterSceneLoadingSettings MasterSceneSettings
        {
            get
            {
                if (_masterSceneSettingsValue == null)
                    _masterSceneSettingsValue = MasterSceneLoadingSettings.GetOrCreateSettings();
                return _masterSceneSettingsValue;
            }
        }

        private static string SceneTableEditorPrefsKey => $"{Application.companyName}:{Application.productName}:LastEditorStartedSceneBuildIndex";
        private static SceneLookupTable LastEditorScenesTable
        {
            get => JsonConvert.DeserializeObject<SceneLookupTable>(EditorPrefs.GetString(SceneTableEditorPrefsKey, string.Empty));
            set => EditorPrefs.SetString(SceneTableEditorPrefsKey, JsonConvert.SerializeObject(value));
        }

        public static string MasterScenePath => EditorBuildSettings.scenes[0].path;


        static MasterSceneLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        /// <summary>
        /// Provides a list of all scene-paths that were loaded when the editor was exited.
        /// </summary>
        /// <param name="activeSceneIndex">The index (from the provided list) of the scene that was active.</param>
        /// <returns>An array of scene-asset-paths.</returns>
        public static string[] GetEditorStartedScenes(out int activeSceneIndex)
        {
            SceneLookupTable table = LastEditorScenesTable;

            activeSceneIndex = table.ActiveSceneIndex;
            return table.LoadedScenePaths;
        }


        private static void RecreateLastEditorSceneSetup()
        {
            bool discardScenesOnNextLoad = true;

            //TODO Keep master scene in editor when checked in the settings
            bool masterSceneLoaded = false;
            if (MasterSceneSettings.KeepMasterInEditor)
            {
                EditorSceneManager.OpenScene(MasterScenePath, OpenSceneMode.Single);
                discardScenesOnNextLoad = false;
                masterSceneLoaded = true;
            }

            for (int i = 0; i < LastEditorScenesTable.LoadedScenePaths.Length; i++)
            {
                string scenePath = LastEditorScenesTable.LoadedScenePaths[i];

                if (scenePath == MasterScenePath && masterSceneLoaded)
                    continue; //Master already loaded.

                Scene s = EditorSceneManager.OpenScene(scenePath, discardScenesOnNextLoad ? OpenSceneMode.Single : OpenSceneMode.Additive);

                if (LastEditorScenesTable.ActiveSceneIndex == i)
                    SceneManager.SetActiveScene(s);

                discardScenesOnNextLoad = false;
            }

            if (SceneManager.GetActiveScene().path != LastEditorScenesTable.ActiveScenePath)
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(LastEditorScenesTable.ActiveScenePath));
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (!MasterSceneSettings.IsActivated)
                return;

            switch (change)
            {
                case PlayModeStateChange.ExitingEditMode:
                    //Store current editor scene setup for later recreation
                    LastEditorScenesTable = GetCurrentEditorScenesAsLookup(out string[] currentEditorScenes);

                    //Handel unsafed scenes
                    HandelSceneSave(currentEditorScenes, out bool stopExecution);
                    if (stopExecution)
                    {
                        Debug.LogWarning($"[{nameof(MasterSceneLoader)}] Scene save processes canceled. Master scene loader will not execute.");
                        return;
                    }

                    //Single-Open the master-scene to run before everything else
                    EditorSceneManager.OpenScene(MasterScenePath, OpenSceneMode.Single);
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    //Restore the scene setup that was stored when last exiting edit mode
                    RecreateLastEditorSceneSetup();
                    break;
            }

            SceneLookupTable GetCurrentEditorScenesAsLookup(out string[] currentEditorScenes)
            {
                int sceneCount = SceneManager.sceneCount;
                string activeScenePath = SceneManager.GetActiveScene().path;

                SceneLookupTable tabel = new SceneLookupTable();
                tabel.LoadedScenePaths = new string[sceneCount];

                currentEditorScenes = new string[sceneCount];
                for (int i = 0; i < sceneCount; i++)
                {
                    Scene s = SceneManager.GetSceneAt(i);
                    currentEditorScenes[i] = s.path;
                    tabel.LoadedScenePaths[i] = s.path;

                    if (s.path == activeScenePath)
                        tabel.ActiveSceneIndex = i;
                }

                return tabel;
            }
        }


        private static void HandelSceneSave(string[] currentEditorScenePaths, out bool stopExecution)
        {
            stopExecution = false;
            switch (MasterSceneSettings.SaveBehaviour)
            {
                case MasterSceneLoadingSettings.SceneSaveBehaviour.DontSave:
                    return;
                case MasterSceneLoadingSettings.SceneSaveBehaviour.PromptSave:
                    if (TryGetAllDirtyScenes(out Scene[] scenesToPrompt))
                    {
                        if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenesToPrompt))
                            stopExecution = true;
                    }
                    break;
                case MasterSceneLoadingSettings.SceneSaveBehaviour.AlwaysSave:
                    if (TryGetAllDirtyScenes(out Scene[] scenesToSave))
                        EditorSceneManager.SaveScenes(scenesToSave);
                    break;
            }

            bool TryGetAllDirtyScenes(out Scene[] modifiedScenes)
            {
                modifiedScenes = currentEditorScenePaths.Select(t => SceneManager.GetSceneByPath(t)).Where(t => t.isDirty).ToArray();
                return modifiedScenes.Any();
            }
        }

        [System.Serializable]
        private struct SceneLookupTable
        {
            public string[] LoadedScenePaths;
            public string ActiveScenePath => LoadedScenePaths[ActiveSceneIndex];
            public int ActiveSceneIndex;

            public SceneLookupTable(string[] loadedScenePaths, int activeSceneIndex)
            {
                LoadedScenePaths = loadedScenePaths;
                ActiveSceneIndex = activeSceneIndex;
            }

            public SceneLookupTable(IEnumerable<string> loadedScenePaths, int activeSceneIndex)
            {
                LoadedScenePaths = loadedScenePaths.ToArray();
                ActiveSceneIndex = activeSceneIndex;
            }
        }
    }
}

#endif