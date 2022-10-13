using EmptySkull.Tools.Unity.Core;

namespace EmptySkull.Utilities
{
    public class DontDestroyOnLoad : MonoBehaviourAutostart
    {
        protected override void AutostartRun()
        {
            Execute();
        }

        public void Execute()
        {
            DontDestroyOnLoad(transform.root);
        }
    }
}
