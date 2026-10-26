using TMPro;
using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This behavior causes the text to turn on and off simulating a blink.  It takes a blink speed and a total duration.
    /// </summary>
    /// <seealso cref="BlackMassSoftware.FloatingTextEngine.Behaviors.IFloatingBehavior" />
    public class BlinkBehavior : IFloatingBehavior
    {
        /// <summary>
        /// The speed at which the text turns off and on (in seconds, 1.0f = 1 second)
        /// </summary>
        private float blinkSpeed;
        /// <summary>
        /// The elapsed time for our current blink state
        /// </summary>
        private float blinkElapsed;
        /// <summary>
        /// The total duration for this behavior
        /// </summary>
        private float duration;
        /// <summary>
        /// The elapsed time for the entire behavior
        /// </summary>
        private float elapsed;
        /// <summary>
        /// Are we turning on or off
        /// </summary>
        private bool blinkOn = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkBehavior"/> class.
        /// </summary>
        /// <param name="blinkSpeedSeconds">The speed at which the text turns off and on (in seconds, 1.0f = 1 second).  This speed is the total time of a full cycle on and then off.</param>
        /// <param name="duration">The total time the behavior lasts for.</param>
        public BlinkBehavior(float blinkSpeedSeconds, float duration)
        {
            this.blinkSpeed = blinkSpeedSeconds;
            this.duration = duration;
            this.blinkOn = true;
            this.elapsed = 0f;
            this.blinkElapsed = 0f;
        }

        /// <summary>
        /// Initializes the behavior
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            // we need to update the blink speed to each On/Off rotation is equal to the full blinkSpeed duration
            blinkSpeed *= 0.5f;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            elapsed += dt;
            float t = Mathf.Clamp01(elapsed / duration);

            // do the blinking here
            blinkElapsed += dt;
            float blinkT = Mathf.Clamp01(blinkElapsed / blinkSpeed);

            if (blinkOn) {
                owner.GetComponent<MeshRenderer>().enabled = true;
            }
            else {
                owner.GetComponent<MeshRenderer>().enabled = false;
            }
            if (blinkT >= 1f) {
                blinkElapsed = 0f;
                blinkOn = !blinkOn;
            }

            if (t >= 1f) {
                IsFinished = true;
            }
        }
    }

}