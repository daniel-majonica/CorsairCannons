using EmptySkull.Core;
using EmptySkull.Tools.Unity.Core;
using System;
using UnityEngine;

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    [RequireComponent(typeof(MasterSceneController))]
    public abstract class MasterSceneInitializer : MonoBehaviour, IInitializable
    {
        private MasterSceneController _masterSceneControllerValue;
        private MasterSceneController MasterSceneController
        {
            get
            {
                if (_masterSceneControllerValue == null)
                    _masterSceneControllerValue = GetComponent<MasterSceneController>();
                return _masterSceneControllerValue;
            }
        }

        private ProgressValue _initializationProgressInternalValue;
        protected ProgressValue InitializationProgressInternal
        {
            get => _initializationProgressInternalValue;
            set
            {
                if (_initializationProgressInternalValue == value)
                    return;

                _initializationProgressInternalValue = value;
                OnProgressChanged?.Invoke();
            }
        }
        public ProgressValue InitializationProgress => InitializationProgressInternal;

#if !UNITY_2021_2_OR_NEWER
        bool IInitializable.IsInitialized => InitializationProgress.IsCompleted;
#endif

        public event Action OnInitializerReady;
        public event Action OnProgressChanged;

        [Header("Updating")]
        [SerializeField] protected float _progressUpdateRate = .1f;

        public abstract void Initialize();

        protected virtual void Awake()
        {
            MasterSceneController.RegisterInitializer(this);
        }

        protected virtual void OnDestroy()
        {
            MasterSceneController.UnregisterInitializer(this);
        }

        protected void InvokeReadyEvent()
        {
            OnInitializerReady?.Invoke();
        }

        public virtual void HandelMasterSceneReady() { }
    }
}