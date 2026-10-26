using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This behavior scales up quickly and then slowly scales down over time.
    /// </summary>
    public class PopBehavior : IFloatingBehavior
    {
        /// <summary>
        /// Bool to flag if we've finished all the movements (pop to and scale down)
        /// </summary>
        private bool finishedAllMovements;
        /// <summary>
        /// The value we're goin to pop (scale) to initially
        /// </summary>
        private float popToValue;
        /// <summary>
        /// How fast/slow we're going to scale down after the pop behavior
        /// </summary>
        private float scaleDownDuration;
        /// <summary>
        /// The elapsed time for our current movement (either pop or scale down)
        /// </summary>
        private float currentMovementElapsed;
        /// <summary>
        /// Did the pop movement finish
        /// </summary>
        private bool popFinished;
        /// <summary>
        /// How long it takes to do the initial pop scaling
        /// </summary>
        private float popToDuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="PopBehavior"/> class.
		/// </summary>
		/// <param name="popTo">The scale we're doing the initial pop to.</param>
		/// <param name="popToDuration">The time it takes to do the initial pop movement.</param>
		/// <param name="scaleDownDuration">The time it takes to scale back down to normal after the pop.</param>
		public PopBehavior(float popTo, float popToDuration, float scaleDownDuration)
        {
            this.popToValue = popTo;
            this.scaleDownDuration = scaleDownDuration;
            this.popToDuration = popToDuration;
        }

        /// <summary>
        /// Initializes the behavior.
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            currentMovementElapsed = 0f;
            popFinished = false;
        }

        /// <summary>
        /// Updates the behavior.
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            if (!popFinished) {
                // do pop movement
                currentMovementElapsed += dt;
                float popT = Mathf.Clamp01(currentMovementElapsed / popToDuration);
                float scaleX = Mathf.Lerp(1f, popToValue, popT);
                float scaleY = Mathf.Lerp(1f, popToValue, popT);
                owner.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                if (popT >= 1f) {
                    currentMovementElapsed = 0f;
                    popFinished = true;
                }
            }
            else {
                currentMovementElapsed += dt;
                float scaleDownT = Mathf.Clamp01(currentMovementElapsed / scaleDownDuration);
                // scale down slowly
                float currentScaleX = owner.transform.localScale.x;
                float currentScaleY = owner.transform.localScale.y;
                float scaleX = Mathf.Lerp(currentScaleX, 1f, scaleDownT);
                float scaleY = Mathf.Lerp(currentScaleY, 1f, scaleDownT);
                owner.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                if (scaleDownT >= 1f) {
                    currentMovementElapsed = 0f;
                    finishedAllMovements = true;
                }
            }

            if (finishedAllMovements) {
                IsFinished = true;
            }
        }

    }
}