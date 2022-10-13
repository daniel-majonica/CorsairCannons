using UnityEngine;
using EmptySkull.Core;
using EmptySkull.Tools.Unity.Core;

namespace EmptySkull.Management
{
    public class ManagerModule : BaseManagerModule 
    {
        protected override void OnInitialize()
        {
            InitializationProgress = 1f;
        }
    }
}