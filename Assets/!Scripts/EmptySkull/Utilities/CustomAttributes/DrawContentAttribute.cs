using System;
using UnityEngine;

namespace EmptySkull.Utilities.Attributes
{
    /// <summary>
    /// Used to draw the current content of scriptable object beneath its reference-field directly inside
    /// the unity inspector. Only works for scriptable object reference fields. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawContentAttribute : PropertyAttribute
    {
        /// <summary>
        /// When set to 'true' a existing custom inspector of the scriptable object type will be used.
        /// Note that this overwrites the 'ShowScriptField'-option. Whether the script-field is drawn
        /// or not is dependent on the used custom inspector (if no one exists, the script field will
        /// be shown by default and cannot be hidden).
        /// </summary>
        public bool AllowCustomInspector;

        /// <summary>
        /// Used to decide whether the (not assignable) 'Script'-field found on top of default inspectors
        /// will be shown or not (default: false).
        /// This setting will have no effect, when 'AllowCustomInspector' is set to true.
        /// </summary>
        public bool ShowScriptField;

        /// <summary>
        /// When set to true, the default foldout representation of the inspector will be replaced by
        /// a simpler inspector without the ability to hide the content.
        /// </summary>
        public bool NoFoldout;
    }
}