using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Utilities
{
    /// <summary>
    /// Utilities class
    /// </summary>
    public static class FloatingTextUtils
    {
        /// <summary> Converts RGB values to Unity Color </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static Color ConvertRGBToColor(float r, float g, float b)
        {
            float newR = r / 255.0f;
            float newG = g / 255.0f;
            float newB = b / 255.0f;
            return new Color(newR, newG, newB);
        }
    }
}