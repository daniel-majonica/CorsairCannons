using UnityEngine;

namespace EmptySkull.Tools.Unity.UI
{
    public class ReplaceTmpTextWithVersion : ReplaceTmpText
    {
        protected override string ReplaceKey => "##VERSION##";
        protected override string NewText => Application.version;
    }
}