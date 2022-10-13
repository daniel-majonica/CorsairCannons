using EmptySkull.Core;
using EmptySkull.Tools.Unity.Core;
using UnityEngine;

namespace EmptySkull.Management
{
    [DefaultExecutionOrder(-900)]
    public abstract class BaseManagerModule : MonoBehaviour, IInitializable
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
        public bool IsInitialized => InitializationProgress.IsCompleted;

        public ProgressValue InitializationProgress { get; protected set; }

#if !UNITY_2021_2_OR_NEWER
        bool IInitializable.IsInitialized => InitializationProgress.IsCompleted;    
#endif

        private bool _quitting;


        protected virtual void Awake()
        {
            if (!Manager.AutomateInitialization)
            {
                Debug.Log($"[Manager] Registering {GetType()} on GameObject {gameObject.name}");
                Manager.TryRegister(this);
            }
           OnAwake();
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_quitting)
                return;

            Manager.TryDeregister(this);
        }

        public virtual void OnAwake() { }

        /// <summary>
        /// [CALLED BY THE MANAGER AUTOMATICALLY]
        /// Initializes the module.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                return;

            OnInitialize();
        }

        /// <summary>
        /// [CALLED BY THE MANAGER AUTOMATICALLY]
        /// Always called when the module is used by the manager.
        /// </summary>
        public void Use()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[{nameof(ManagerModule)} on: {gameObject.name}] Access denied because module was not initialized. " +
                                 "Make sure to call Module after 'Awake' and to have an active 'Manager' in the scene. Use modules 'IsInitialized' " +
                                 "value to check if it was initialized correctly.");
                return;
            }
            OnUse();
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnUse() { }
    }
}
