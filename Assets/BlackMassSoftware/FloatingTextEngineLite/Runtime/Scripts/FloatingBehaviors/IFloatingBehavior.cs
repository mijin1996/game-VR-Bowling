using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// Interface class for all floating text behaviors.  Implement this class to define new behavior components.
    /// </summary>
    public abstract class IFloatingBehavior
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is finished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished { get; protected set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether [wait for finish].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [wait for finish]; otherwise, <c>false</c>.
        /// </value>
        public bool WaitForFinish { get; protected set; } = false;
        /// <summary>
        /// The delay for timer
        /// </summary>
        private float delayForTimer = 0f;
        /// <summary>
        /// The current delay elapsed
        /// </summary>
        private float currentDelayElapsed = 0f;

        /// <summary>
        /// The owner
        /// </summary>
        protected GameObject owner;

        /// <summary>
        /// Defines if this behavior has a "finish" condition or if it just lasts for the entirety of the behavior list.
        /// </summary>
        /// <seealso cref="BlackMassSoftware.FloatingTextEngine.Behaviors.BillboardBehavior" />
        public bool IsPersistentBehavior { get; protected set; } = false;

        /// <summary>
        /// Initializes the behavior.
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public virtual void InitializeBehavior(GameObject owner) { this.owner = owner; }
        /// <summary>
        /// Updates the behavior.
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public abstract void UpdateBehavior(float dt);
        /// <summary>
        /// Marks this behavior as a wait behavior. Any behaviors following this one will have to wait before it finishes for them to start.
        /// </summary>
        /// <returns></returns>
        public IFloatingBehavior Wait() { WaitForFinish = true; return this; }
        /// <summary>
        /// Delays the start of the behavior for a time.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <returns></returns>
        public IFloatingBehavior DelayFor(float delay) { delayForTimer = delay; return this; }

        /// <summary>
        /// Does the wait for delay.
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        /// <returns></returns>
        public bool DoWaitForDelay(float dt)
        {
            bool delayOver = false;
            currentDelayElapsed += dt;
            if (currentDelayElapsed >= delayForTimer) {
                delayOver = true;
                currentDelayElapsed = 0f;
                delayForTimer = 0f;
            }
            return delayOver;
        }
    }
}