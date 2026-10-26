using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This behavior causes the floating text to shake along the x axis.
    /// </summary>
    public class ShakeX3DBehavior : IFloatingBehavior
    {
        /// <summary>
        /// The current elapsed time for the behavior
        /// </summary>
        private float elapsed;
        /// <summary>
        /// The duration this behavior lasts for
        /// </summary>
        private float duration;
        /// <summary>
        /// The current elapsed time for this shake movement
        /// </summary>
        private float shakeElapsed;
        /// <summary>
        /// The duration the shake movement takes to complete
        /// </summary>
        private float shakePointDuration;
        /// <summary>
        /// The point to the left of the start transform position x value
        /// </summary>
        private float minXPosition;
        /// <summary>
        /// The point to the right of the start transform position x value
        /// </summary>
        private float maxXPosition;
        /// <summary>
        /// Boolean that controls if we've made it to the shake point and when to go back to start position
        /// </summary>
        private bool lerpToLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShakeX3DBehavior"/> class.
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        public ShakeX3DBehavior(float duration)
        {
            this.duration = duration;
        }

        /// <summary>
        /// Initializes the behavior
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            // set the two x min/max positions from the start transform
            minXPosition = owner.transform.position.x - 0.1f;
            maxXPosition = owner.transform.position.x + 0.1f;
            elapsed = 0f;
            shakeElapsed = 0f;
            shakePointDuration = 0.075f;
            lerpToLeft = true;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            elapsed += dt;
            float mainT = Mathf.Clamp01(elapsed / duration);

            // now do the shake behavior by quickly lerping to our shakePoint and back then get a new position
            shakeElapsed += dt;
            float shakeT = Mathf.Clamp01(shakeElapsed / shakePointDuration);
            if (lerpToLeft) {
                float lerpX = Mathf.Lerp(maxXPosition, minXPosition, shakeT);
                owner.transform.position = new Vector3(lerpX, owner.transform.position.y, owner.transform.position.z);
            }
            else {
                float lerpX = Mathf.Lerp(minXPosition, maxXPosition, shakeT);
                owner.transform.position = new Vector3(lerpX, owner.transform.position.y, owner.transform.position.z);
            }
            // the time controller for our actual shaking.  If we did the initial shake and moved back then we need a new point.
            if (shakeT >= 1f) {
                shakeElapsed = 0f;
                lerpToLeft = !lerpToLeft;
            }

            if (mainT >= 1f) {
                IsFinished = true;
            }
        }
    }

}