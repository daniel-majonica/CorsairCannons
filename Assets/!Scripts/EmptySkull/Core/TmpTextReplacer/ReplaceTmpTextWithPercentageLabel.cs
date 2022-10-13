using EmptySkull.Tools.Unity.Core;
using TMPro;
using UnityEngine;

namespace EmptySkull.Tools.Unity.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ReplaceTmpTextWithPercentageLabel : ReplaceTmpText
    {
        private TMP_Text _tmp;
        private string _nativeTextValue;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DisableIf("@UnityEngine.Application.isPlaying")]
#endif
        [SerializeField, Range(0, 1f)] private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;

                _tmp.text = _nativeTextValue; //Reset to recreate
                Replace();
            }
        }

        protected override void Awake()
        {
            _tmp = GetComponent<TMP_Text>();
            _nativeTextValue = _tmp.text;

            base.Awake();
        }

        protected override string ReplaceKey => "#";

        protected override string NewText => AsPercentage(Value);

        private string AsPercentage(float v)
            => ((ProgressValue)v).AsPercentLabel(true, 0);
    }
}