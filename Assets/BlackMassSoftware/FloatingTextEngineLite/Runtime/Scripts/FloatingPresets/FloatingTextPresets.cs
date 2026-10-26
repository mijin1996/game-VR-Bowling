using System.Collections.Generic;
using UnityEngine;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;
using BlackMassSoftware.FloatingTextEngine.Lite.Utilities;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Presets
{
    using FloatingTextPreset = List<IFloatingBehavior>;

    /// <summary>
    /// This optional class has predefined behaviors so the user can call them directly for common behaviors.
    /// This prevents the user from having to build their own based on the intended behavior.
    /// </summary>
    public class FloatingTextPresets
    {
        /// <summary>
        /// Builds a default hit floating text behavior.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Default Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset Default()
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Default Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Builds a critical hit floating text behavior.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Critical Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset Critical()
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Critical Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Builds a miss hit floating text behavior.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Miss Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset Miss()
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Miss Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Builds a block floating text behavior.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Block Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset Block()
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Block Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Builds a Healing floating text behavior.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Heal Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset Heal()
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Heal Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Creates a preset behavior based off the provided damage type
        /// </summary>
        /// <param name="damageType">The damage type to create a behavior for.</param>
        /// <returns></returns>
        /// <remarks>MagicDamage Preset - Available in the Full Version!</remarks>
        public static FloatingTextPreset MagicDamage(EFloatingDamageType damageType)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> MagicDamage Preset is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }
    }
}