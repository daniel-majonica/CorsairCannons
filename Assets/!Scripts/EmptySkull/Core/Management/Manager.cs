using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmptySkull.Management
{
    [DefaultExecutionOrder(-950)]
    public class Manager : MonoBehaviour, ISingleScriptCallback
    {
        [SerializeField] private bool _automateInitialization = true;
        public static bool AutomateInitialization;
        
        private static Dictionary<Type, BaseManagerModule> Modules;
        private bool _instanceWillBeDestroyed;

        public static event Action<BaseManagerModule> OnModuleRegistered;
        public static event Action<BaseManagerModule> OnModuleDeregistered;

        protected virtual void Awake()
        {
            if(_instanceWillBeDestroyed)
                return;

            AutomateInitialization = _automateInitialization;

            Modules = new Dictionary<Type, BaseManagerModule>();
            foreach (BaseManagerModule module in FindObjectsOfType<BaseManagerModule>())
            {
                Type t = module.GetType();
                if (!TryRegister(module))
                {
                    Debug.LogWarning($"[{nameof(Manager)}] Module of type '{t.Name}' already listed. " +
                                     $"Module on game-object '{module.gameObject.name}' will be ignored!");
                    continue;
                }
            }
        }

        public static T Use<T>() where T : BaseManagerModule
        {
            T m = (T) Modules[typeof(T)];
            m.Use();
            return m;
        }

        public static bool TryRegister(BaseManagerModule module)
        {
            Type moduleType = module.GetType();
            if (Modules.ContainsKey(moduleType))
                return false;

            if(AutomateInitialization)
                module.Initialize();
            
            Modules.Add(moduleType, module);

            OnModuleRegistered?.Invoke(module);

            return true;
        }

        public static bool TryDeregister(BaseManagerModule module)
        {
            Type moduleType = module.GetType();
            if (!Modules.TryGetValue(moduleType, out BaseManagerModule registeredModule) || registeredModule != module)
                return false;

            Modules.Remove(moduleType);

            OnModuleDeregistered?.Invoke(module);

            return true;
        }

        public void OnDestroyedByOther()
        {
            _instanceWillBeDestroyed = true;
        }
    }
}

