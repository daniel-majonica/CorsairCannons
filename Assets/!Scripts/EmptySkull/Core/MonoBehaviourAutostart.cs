using UnityEngine;

namespace EmptySkull.Tools.Unity.Core
{
    public abstract class MonoBehaviourAutostart : MonoBehaviour
    {
        public enum StartMethod
        {
            None,
            Awake,
            Start,
        }

        [SerializeField] private StartMethod _autostartMode = StartMethod.Awake;

        public StartMethod AutostartMode => _autostartMode;

        protected virtual void Awake()
        {
            if(_autostartMode == StartMethod.Awake)
                AutostartRun();
        }

        protected virtual void Start()
        {
            if(_autostartMode == StartMethod.Start)
                AutostartRun();
        }

        protected abstract void AutostartRun();
    }
}