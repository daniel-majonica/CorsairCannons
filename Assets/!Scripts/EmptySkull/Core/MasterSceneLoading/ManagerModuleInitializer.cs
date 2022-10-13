using EmptySkull.Core;
using EmptySkull.Management;
using EmptySkull.Tools.Unity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_2021_2_OR_NEWER
using Sirenix.Utilities; //Used Odins ToHashSet-Extension before Linq implemented one (.NET 4.7.2)
#endif

namespace EmptySkull.Tools.Unity.MasterSceneLoading
{
    [DefaultExecutionOrder(-950)]
    public class ManagerModuleInitializer : MasterSceneInitializer
    {
        private HashSet<IInitializable> _initializables = new HashSet<IInitializable>();

        private Action<BaseManagerModule> registeredAction;
        private Action<BaseManagerModule> unregisteredAction;


        private HashSet<IInitializable> _initializations;

        protected override void Awake()
        {
            base.Awake();

            registeredAction = module => _initializables.Add(module);
            unregisteredAction = module => _initializables.Remove(module);

            Manager.OnModuleRegistered += registeredAction;
            Manager.OnModuleDeregistered += unregisteredAction;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Manager.OnModuleRegistered -= registeredAction;
            Manager.OnModuleDeregistered -= unregisteredAction;
        }

        public override void Initialize()
        {
            StartModuleInitialization(out _initializations);
            ReportInitializationProgress(_initializations, new Progress<ProgressValue>(t => InitializationProgressInternal = t));
        }

        private void StartModuleInitialization(out HashSet<IInitializable> initializations)
        {
            foreach (IInitializable initializable in _initializables)
            {
                initializable.Initialize();
            }

            initializations = _initializables.Where(t => !t.IsInitialized).ToHashSet();
        }

        private async void ReportInitializationProgress(HashSet<IInitializable> initializations, IProgress<ProgressValue> progress)
        {

            if (!initializations.Any())
            {
                progress.Report(1f);
                InvokeReadyEvent();
                return;
            }

            int waitMs = (int)TimeSpan.FromSeconds(_progressUpdateRate).TotalMilliseconds;

            float p;
            do
            {
                float sum = 0;
                foreach (IInitializable i in initializations)
                {
                    sum += i.InitializationProgress;
                }

                p = sum / initializations.Count;

                progress.Report(p);

                await Task.Delay(waitMs);
            } while (p < 1f);

            InvokeReadyEvent();
        }
    }
}