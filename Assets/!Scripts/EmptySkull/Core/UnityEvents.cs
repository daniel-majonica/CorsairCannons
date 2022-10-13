using UnityEngine;
using UnityEngine.Events;

namespace EmptySkull.Tools.Unity.Core
{
    public class UnityEvents
    {
        [System.Serializable] public class UnityEventBool : UnityEvent<bool> { }
        [System.Serializable] public class UnityEventInt : UnityEvent<int> { }
        [System.Serializable] public class UnityEventFloat : UnityEvent<float> { }
        [System.Serializable] public class UnityEventString : UnityEvent<string> { }
        [System.Serializable] public class UnityEventChar : UnityEvent<char> { }
        [System.Serializable] public class UnityEventByte : UnityEvent<byte> { }
        [System.Serializable] public class UnityEventGameObject : UnityEvent<GameObject> { }
    }
}