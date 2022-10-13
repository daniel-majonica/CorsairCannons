using EmptySkull.Tools.Unity.Core;

namespace EmptySkull.Core
{
    public interface IInitializable
    {
        ProgressValue InitializationProgress { get; }

#if UNITY_2021_2_OR_NEWER
        bool IsInitialized => InitializationProgress.IsCompleted;
#else
        bool IsInitialized { get; }
#endif
        void Initialize();
    }
}
