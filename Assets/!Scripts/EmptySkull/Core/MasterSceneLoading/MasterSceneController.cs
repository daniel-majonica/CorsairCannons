using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using EmptySkull.Tools.Unity.Core;
using static EmptySkull.Tools.Unity.Core.UnityEvents;

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    [DefaultExecutionOrder(-975)]
    public class MasterSceneController : MonoBehaviour
    {
        private static HashSet<MasterSceneInitializer> _masterSceneInitializer;
        private HashSet<MasterSceneInitializer> _currentlyRunningInitializer = new HashSet<MasterSceneInitializer>();

        public static event Action OnProgressUpdated;

        [SerializeField] private bool _executeOnStart = true;
        [SerializeField] private UnityEventFloat OnProgressChanged;

        public ProgressValue TotalProgress => _masterSceneInitializer.Average(t => t.InitializationProgress);

        private Action _progressUpdatedAction;


        protected virtual void Start()
        {
            if (_executeOnStart)
                Execute();
        }

        public void RegisterInitializer(MasterSceneInitializer initializer)
        {
            if (_masterSceneInitializer == null)
                _masterSceneInitializer = new HashSet<MasterSceneInitializer>();

            if (_progressUpdatedAction == null)
                _progressUpdatedAction = () =>
                {
                    OnProgressUpdated?.Invoke();
                    OnProgressChanged?.Invoke(TotalProgress);
                };

            _masterSceneInitializer.Add(initializer);
            initializer.OnProgressChanged += _progressUpdatedAction;
        }

        public void UnregisterInitializer(MasterSceneInitializer initializer)
        {
            _masterSceneInitializer.Remove(initializer);
            initializer.OnProgressChanged -= _progressUpdatedAction;
        }

        public void Execute()
        {
            foreach (MasterSceneInitializer masterSceneInitializer in _masterSceneInitializer)
            {
                _currentlyRunningInitializer.Add(masterSceneInitializer);

                MasterSceneInitializer temp = masterSceneInitializer;
                masterSceneInitializer.OnInitializerReady += () => HandelInitializerReady(temp);
            }
            //First add ALL initializer to the current hashset, then execute them: Prevent simultanious executions from skipping the registration
            foreach (MasterSceneInitializer masterSceneInitializer in _masterSceneInitializer)
            {
                masterSceneInitializer.Initialize();
            }

            void HandelInitializerReady(MasterSceneInitializer initializer)
            {
                _currentlyRunningInitializer.Remove(initializer);

                if (_currentlyRunningInitializer.Count <= 0)
                {
                    foreach (MasterSceneInitializer masterSceneInitializer in _masterSceneInitializer)
                        masterSceneInitializer.HandelMasterSceneReady();
                }
            }
        }
    }
}