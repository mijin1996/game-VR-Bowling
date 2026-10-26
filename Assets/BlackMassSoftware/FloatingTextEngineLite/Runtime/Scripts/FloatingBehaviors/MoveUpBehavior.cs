using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This behavior moves up in the Y axis.
    /// </summary>
    /// <seealso cref="BlackMassSoftware.FloatingTextEngine.Behaviors.IFloatingBehavior" />
    public class MoveUpBehavior : IFloatingBehavior
    {
        /// <summary>
        /// The maximum y distance
        /// </summary>
        private float maxYDistance = 3f;
        /// <summary>
        /// The start position
        /// </summary>
        private Vector3 startPosition;
        /// <summary>
        /// The end position
        /// </summary>
        private Vector3 endPosition;
        /// <summary>
        /// The elapsed
        /// </summary>
        private float elapsed;
        /// <summary>
        /// The duration
        /// </summary>
        private float duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUpBehavior"/> class.
        /// </summary>
        /// <param name="yOffset">The y offset that we are going to move to.</param>
        /// <param name="moveDuration">The total duration of the behavior.</param>
        public MoveUpBehavior(float yOffset, float moveDuration)
        {
            maxYDistance = yOffset;
            elapsed = 0f;
            duration = moveDuration;
        }

        /// <summary>
        /// Initializes the behavior.
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            startPosition = owner.transform.position;
            endPosition = new Vector3(owner.transform.position.x, owner.transform.position.y + maxYDistance, owner.transform.position.z);
        }

        /// <summary>
        /// Updates the behavior.
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            if (owner != null && !IsFinished) {
                elapsed += dt;
                float t = Mathf.Clamp01(elapsed / duration);
                owner.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                if (t >= 1f) {
                    IsFinished = true;
                }
            }
        }
    }
}