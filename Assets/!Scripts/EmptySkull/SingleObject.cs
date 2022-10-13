using System;
using System.Linq;
using UnityEngine;

namespace EmptySkull
{
    public class SingleObject : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private string _uniqueKey;
#pragma warning restore 649
        public string UniqueKey => _uniqueKey;

        protected virtual void Awake()
        {
            if (string.IsNullOrWhiteSpace(_uniqueKey))
                throw new ArgumentException(
                    $"[{nameof(SingleObject)} on: {gameObject.name}] {nameof(UniqueKey)} invalid. Empty keys are not allowed.");

            if (!FindObjectsOfType<SingleObject>().Where(sObj => sObj != this).Any(sObj =>
                string.Equals(sObj.UniqueKey, UniqueKey, StringComparison.CurrentCultureIgnoreCase)))
                return;

            foreach (ISingleScriptCallback singleCallback in GetComponentsInChildren<ISingleScriptCallback>())
            {
                singleCallback.OnDestroyedByOther();
            }

            Destroy(gameObject);
        }
    }

    public interface ISingleScriptCallback
    {
        void OnDestroyedByOther();
    }
}