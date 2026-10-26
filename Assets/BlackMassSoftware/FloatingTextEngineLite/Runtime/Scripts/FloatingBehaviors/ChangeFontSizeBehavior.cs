using TMPro;
using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
	/// <summary>
	/// This behavior will set this floating texts specific size. This is a substitute for changing the global size through the FloatingTextEngine inspector window.
	/// </summary>
	/// <seealso cref="BlackMassSoftware.FloatingTextEngine.Behaviors.IFloatingBehavior" />
	public class ChangeFontSizeBehavior : IFloatingBehavior
    {
        /// <summary>
        /// The new font size of the floating text
        /// </summary>
        private float fontSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeFontSizeBehavior"/> class.
        /// </summary>
        /// <param name="fontSize">The new font size for this floating text.</param>
        public ChangeFontSizeBehavior(float fontSize)
        {
            this.fontSize = fontSize;
        }

        /// <summary>
        /// Initializes the behavior
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            // set the owners font size here
            if (owner.TryGetComponent(out TextMeshPro textMeshPro)) {
                textMeshPro.fontSize = this.fontSize;
            }

            // set this behavior to finished. There's nothing more that it does
            IsFinished = true;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void UpdateBehavior(float dt)
        {
            
        }
    }

}