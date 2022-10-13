using EmptySkull.Tools.Unity.Core;
using TMPro;
using UnityEngine;

namespace EmptySkull.Tools.Unity.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public abstract class ReplaceTmpText : MonoBehaviourAutostart
    {
        private TMP_Text _text;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        protected abstract string ReplaceKey { get; }

        protected abstract string NewText { get; }

        protected override void Awake() => _text = GetComponent<TMP_Text>();

        public void Replace()
        {
            _text.text = _text.text.Replace(ReplaceKey, NewText);
        }

        protected sealed override void AutostartRun()
        {
            Replace();
        }
    }
}
