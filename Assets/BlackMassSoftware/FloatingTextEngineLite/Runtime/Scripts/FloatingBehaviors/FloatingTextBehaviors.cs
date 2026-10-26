using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This wrapper class hides the "new" calls for each behavior.  This needs to be updated when a new behavior is implemented.
    /// Pass these methods into the .With() function from the FloatingBehaviorEngine class. 
    /// </summary>
    public class FloatingTextBehaviors
    {
        /// <summary>
        /// Fades out the floating text within specified duration.
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior FadeOut(float duration)
        {
            return new FadeOutBehavior(duration);
        }

        /// <summary>
        /// Move the up/down in the y axis over duration
        /// </summary>
        /// <param name="yOffset">The y offset that we are going to move to.</param>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior MoveUp(float yOffset, float duration)
        {
            return new MoveUpBehavior(yOffset, duration);
        }

        /// <summary>
        /// Rotates the object on the z axis back and forth for the duration
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior Wobble(float duration)
        {
            return new WobbleBehavior(duration);
        }

        /// <summary>
        /// Shakes the floating text object left to right
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior ShakeX2D(float duration)
        {
            return new ShakeX2DBehavior(duration);
        }

        /// <summary>
        /// Shakes the floating text object left to right
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior ShakeX3D(float duration)
        {
            return new ShakeX3DBehavior(duration);
        }

        /// <summary>
        /// Scales up quickly and then slowly scales down
        /// </summary>
        /// <param name="popTo">The scale we're doing the initial pop to.</param>
        /// <param name="popToDuration">The time it takes to do the initial pop movement.</param>
        /// <param name="scaleDownDuration">The time it takes to scale back down to normal after the pop.</param>
        /// <returns></returns>
        public static IFloatingBehavior Pop(float popTo, float popToDuration, float scaleDownDuration)
        {
            return new PopBehavior(popTo, popToDuration, scaleDownDuration);
        }

        /// <summary>
        /// Starts a blinking effect
        /// </summary>
        /// <param name="blinkSpeed">The speed at which the text turns off and on (in seconds, 1.0f = 1 second).  This speed is the total time of a full cycle on and then off.</param>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        public static IFloatingBehavior Blink(float blinkSpeed, float duration)
        {
            return new BlinkBehavior(blinkSpeed, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontSize">The new font size of the floating text.</param>
        /// <returns></returns>
        public static IFloatingBehavior ChangeFontSize(float fontSize)
        {
            return new ChangeFontSizeBehavior(fontSize);
        }

        /// <summary>
        /// Billboards the text object using the mode provided. Full - Complete rotation towards eye. YAxisOnly - Rotates but locks Y axis.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eye"></param>
        /// <returns></returns>
        public static IFloatingBehavior BillboardTextObject(EBillboardType type, Transform eye)
        {
            return new BillboardBehavior(type, eye);
        }


        #region Pro Behavior Methods

        /// <summary>
        /// Moves up arc and in an arc over duration.
        /// </summary>
        /// <param name="arcHeight">Height of the arc.</param>
        /// <param name="arcSpread">The arc spread.</param>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        /// <remarks>MoveUpArc2D - Available in the Full Version!</remarks>
        public static IFloatingBehavior MoveUpArc2D(float arcHeight, float arcSpread, float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> MoveUpArc2D is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Moves up arc and in an arc over duration.
        /// </summary>
        /// <param name="arcHeight">Height of the arc.</param>
        /// <param name="arcSpread">The arc spread.</param>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        /// <remarks>MoveUpArc3D - Available in the Full Version!</remarks>
        public static IFloatingBehavior MoveUpArc3D(float arcHeight, float arcSpread, float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> MoveUpArc3D is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Moves up in the Y axis and expands when it gets there.
        /// </summary>
        /// <param name="moveTo">The y offset that we are going to move to.</param>
        /// <param name="moveDuration">The time it takes to do the move.</param>
        /// <param name="expandTo">The value we're going to scale to.</param>
        /// <param name="expandDuration">The time it takes to do the scale.</param>
        /// <returns></returns>
        /// <remarks>MoveUpExpand - Available in the Full Version!</remarks>
        public static IFloatingBehavior MoveUpExpand(float moveTo, float moveDuration, float expandTo, float expandDuration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> MoveUpExpand is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Scales the floating text to the specified value.
        /// </summary>
        /// <param name="scaleTo">The value we're going to scale to.</param>
        /// <param name="duration">The time it takes to do the scaling.</param>
        /// <returns></returns>
        /// <remarks>ScaleTo - Available in the Full Version!</remarks>
        public static IFloatingBehavior ScaleTo(float scaleTo, float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> ScaleTo is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Expands then shrinks for the specified amount of time.
        /// 1 time = full cycle of expand/shrink
        /// </summary>
        /// <param name="scaleTo">The value we're going to expand to.</param>
        /// <param name="duration">The duration to do each expand and shrink movement.</param>
        /// <param name="howManyTimes">The number of times to do the entire cycle of expand/shrink.</param>
        /// <returns></returns>
        /// <remarks>ExpandShrink - Available in the Full Version!</remarks>
        public static IFloatingBehavior ExpandShrink(float scaleTo, float duration, int howManyTimes)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> ExpandShrink is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Shakes the floating text over the specified duration.
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        /// <remarks>Shake2D - Available in the Full Version!</remarks>
        public static IFloatingBehavior Shake2D(float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Shake2D is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Shakes the floating text over the specified duration.
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        /// <remarks>Shake3D - Available in the Full Version!</remarks>
        public static IFloatingBehavior Shake3D(float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> Shake3D is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// Lerps from one color to the other
        /// </summary>
        /// <param name="from">The start color</param>
        /// <param name="to">The end color</param>
        /// <param name="duration">How long it takes to lerp between the colors</param>
        /// <returns></returns>
        /// <remarks>ColorGradient - Available in the Full Version!</remarks>
        public static IFloatingBehavior ColorGradient(Color from, Color to, float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> ColorGradient is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fadeInOutSpeedSeconds">The speed at which the text fades in/out (in seconds, 1.0f = 1 second).  This speed is the total time of a full cycle of fade in/fade out.</param>
        /// <param name="duration">The total duration of the behavior.</param>
        /// <returns></returns>
        /// <remarks>FadeInOut - Available in the Full Version!</remarks>
        public static IFloatingBehavior FadeInOut(float fadeInOutSpeedSeconds, float duration)
        {
#if UNITY_EDITOR
            Debug.LogError("<b>FloatingTextEngine Lite:</b> FadeInOut is available in the <color=orange>Full Version</color> of FloatingTextEngine!");
#endif
            return null;
        }

        #endregion
    }
}