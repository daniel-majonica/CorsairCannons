using EmptySkull.Tools.Unity.Core;
using System;

namespace EmptySkull.Management
{
    public abstract class ManagerModuleAsync : BaseManagerModule
    {
        public new ProgressValue InitializationProgress
        {
            get => base.InitializationProgress;
            private set => base.InitializationProgress = value;
        }

        protected sealed override void OnInitialize()
        {
            OnInitializeAsync(new Progress<ProgressValue>(t => InitializationProgress = t));
        }

        protected virtual void OnInitializeAsync(IProgress<ProgressValue> progress)
        {
            progress.Report(1f);
        }
    }
}