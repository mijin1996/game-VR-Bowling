using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{

    /// <summary>
    /// This behavior causes the floating text to rotate back and forth on the z axis.
    /// </summary>
    public class WobbleBehavior : IFloatingBehavior
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
        /// Controls which direction we're rotating on the z axis
        /// </summary>
        private bool flipRotation;
        /// <summary>
        /// The current elapsed time for the rotation
        /// </summary>
        private float rotationElapsed;
        /// <summary>
        ///  How long does it take to do a full rotation
        /// </summary>
        private float rotationDuration;
        /// <summary>
        /// The current z axis rotation of the object
        /// </summary>
        private float currentZRotation;
        /// <summary>
        /// The start z axis rotation
        /// </summary>
        private float startZRotation;
        /// <summary>
        /// The end z axis rotation (where we're rotating to)
        /// </summary>
        private float endZRotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="WobbleBehavior"/> class.
        /// </summary>
        /// <param name="duration">How long the shake behavior will run</param>
        public WobbleBehavior(float duration)
        {
            this.duration = duration;
        }

        /// <summary>
        /// Initialzes the behavior
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            elapsed = 0f;
            rotationElapsed = 0f;
            rotationDuration = 0.1f;
            currentZRotation = 0f;
            startZRotation = 0f;
            endZRotation = 20f;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            elapsed += dt;
            float t = Mathf.Clamp01(elapsed / duration);

            // now do the rotation behavior
            rotationElapsed += dt;
            float rotationT = Mathf.Clamp01(rotationElapsed / rotationDuration);
            currentZRotation = Mathf.Lerp(startZRotation, endZRotation, rotationT);
            owner.transform.eulerAngles = new Vector3(0f, 0f, currentZRotation);
            if (rotationT >= 1f) {
                rotationElapsed = 0f;
                flipRotation = !flipRotation;
                startZRotation = currentZRotation;
                if (flipRotation) {
                    endZRotation *= -1f;
                }
            }

            if (t >= 1f) {
                IsFinished = true;
            }
        }
    }

}