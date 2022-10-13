using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using EmptySkull.Tools.Unity.Core;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    public partial class SceneInitializer : MasterSceneInitializer
    {
        private const float SceneReadyProgressThreshold = .9f;

        [Header("Scene Management")]
        [Tooltip("Scenes to be loaded once the Master scene is exited. The first element in the array will be the active scene.")]
        [SerializeField] private int[] _scenesToLoad;
        [Tooltip("When true, the Master scene will not be unloaded when exited.")]
        [SerializeField] private bool _keepMasterScene;

        private HashSet<AsyncOperation> _sceneLoadingOperations;

        private int _currentSceneLoadCompletions;

        private Coroutine _switchActiveRoutine;

        public override void Initialize()
        {
            _currentSceneLoadCompletions = 0;

            StartLoadingNextScenes(out _sceneLoadingOperations);
            ReportSceneLoadingProcess(_sceneLoadingOperations, new Progress<ProgressValue>(t => InitializationProgressInternal = t));
        }

        private void StartLoadingNextScenes(out HashSet<AsyncOperation> sceneLoadingOperations)
        {
#if UNITY_EDITOR
            sceneLoadingOperations = LoadScenesEditorMode();
#else
            //When started in a build: Load the scenes assigned in the inspector of this script
            sceneLoadingOperations = LoadScenesNonEditorMode();
#endif
        }

#if UNITY_EDITOR
        private HashSet<AsyncOperation> LoadScenesEditorMode()
        {
            //When started from editor: Load the scenes that were loaded in the editor once exiting the master-scene (using editor loading)
            string[] editorLoadedScenes = MasterSceneLoader.GetEditorStartedScenes(out int activeSceneIndex);
            int masterSceneIndex = Array.IndexOf(editorLoadedScenes, MasterSceneLoader.MasterScenePath);
            string[] scenesToLoad = new string[masterSceneIndex < 0 ? editorLoadedScenes.Length : editorLoadedScenes.Length - 1];
            for (int i = 0; i < editorLoadedScenes.Length; i++)
            {
                if (i == masterSceneIndex)
                    continue;

                scenesToLoad[masterSceneIndex < 0 || i < masterSceneIndex ? i : i - 1] = editorLoadedScenes[i];
            }

            HashSet<AsyncOperation> sceneLoadingOperations = new HashSet<AsyncOperation>();

            bool anySceneLoaded = false;
            for (int i = 0; i < scenesToLoad.Length; i++) //Try and load editor-loaded scenes
            {
                if (scenesToLoad[i] == MasterSceneLoader.MasterScenePath)
                    continue;

                string path = scenesToLoad[i];

                AsyncOperation load = EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Additive));
                load.allowSceneActivation = false;

                load.completed += _ => ReportSceneLoadCompletion(scenesToLoad.Length, editorLoadedScenes[activeSceneIndex]);

                anySceneLoaded = true;
                sceneLoadingOperations.Add(load);
            }

            if (!anySceneLoaded) //Scene loading failed, because only the master-scene was editor-started: Load like in build mode
            {
                Debug.LogWarning($"[{nameof(SceneInitializer)} on: {gameObject.name}] Could not load the editor-loaded scenes " +
                    $"(most likely because the master scene was started in the editor). Fallback: Load the build-settings scenes instead.", this);
                sceneLoadingOperations = LoadScenesNonEditorMode();
            }

            return sceneLoadingOperations;
        }
#endif

        private HashSet<AsyncOperation> LoadScenesNonEditorMode()
        {
            if (_scenesToLoad.Length <= 0)
                throw new UnityException("No scenes declared to be loaded after the master scene!");

            HashSet<AsyncOperation> loadOperations = new HashSet<AsyncOperation>();

            bool discardOldScenes = !_keepMasterScene;
            for (int i = 0; i < _scenesToLoad.Length; i++)
            {
                //TODO Hold process for loading-bar...
                AsyncOperation load = SceneManager.LoadSceneAsync(_scenesToLoad[i], LoadSceneMode.Additive);
                load.allowSceneActivation = false;

                loadOperations.Add(load);

                //Make the first scene in the list active once loaded
                load.completed += _ => ReportSceneLoadCompletion(_scenesToLoad.Length, SceneManager.GetSceneByBuildIndex(_scenesToLoad[0]).path);

                discardOldScenes = false;
            }

            return loadOperations;
        }

        private async void ReportSceneLoadingProcess(HashSet<AsyncOperation> loadOperations, IProgress<ProgressValue> progress)
        {
            float p = 0;

            int waitMs = (int)TimeSpan.FromSeconds(_progressUpdateRate).TotalMilliseconds;

            do
            {
                float sum = 0;
                foreach (AsyncOperation o in loadOperations)
                {
                    sum += Mathf.InverseLerp(0, SceneReadyProgressThreshold, o.progress);
                }

                p = sum / loadOperations.Count;

                progress.Report(p);

                await Task.Delay(waitMs);
            } while (p < 1f);

            if (!_keepMasterScene)
            {
                loadOperations.First().completed += _ =>
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(0));
                };
            }

            InvokeReadyEvent();
        }

        public override void HandelMasterSceneReady()
        {
            //Debug.Log($"[Max] {nameof(HandelMasterSceneReady)} executed (Loading were null? {_sceneLoadingOperations == null})");
            foreach (AsyncOperation o in _sceneLoadingOperations)
            {
                o.allowSceneActivation = true;
            }
        }

        private void ReportSceneLoadCompletion(int expectedCompletions, string activeScenePath)
        {
            _currentSceneLoadCompletions++;
            if (_currentSceneLoadCompletions >= expectedCompletions)
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(activeScenePath));

        }
    }
}