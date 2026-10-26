using TMPro;
using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{
    /// <summary>
    /// This behavior will fade out the text over the specified duration.
    /// </summary>
    /// <seealso cref="BlackMassSoftware.FloatingTextEngine.Behaviors.IFloatingBehavior" />
    public class FadeOutBehavior : IFloatingBehavior
    {
        /// <summary>
        /// The elapsed time
        /// </summary>
        private float elapsed;
        /// <summary>
        /// The fade duration
        /// </summary>
        private float fadeDuration;
        /// <summary>
        /// The text mesh pro component
        /// </summary>
        private TextMeshPro textMeshPro;

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeOutBehavior"/> class.
        /// </summary>
        /// <param name="duration">The total duration of the behavior.</param>
        public FadeOutBehavior(float duration)
        {
            elapsed = 0f;
            fadeDuration = duration;
        }

        /// <summary>
        /// Initializes the behavior.
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
            if (owner.TryGetComponent(out TextMeshPro textMesh)) {
                textMeshPro = textMesh;
            }
        }

        /// <summary>
        /// Updates the behavior.
        /// </summary>
        /// <param name="dt">The deltatime.</param>
        public override void UpdateBehavior(float dt)
        {
            if (textMeshPro != null) {
                elapsed += dt;
                float t = Mathf.Clamp01(elapsed / fadeDuration);

                textMeshPro.alpha = Mathf.Lerp(1f, 0f, t);

                if (t >= 1f) {
                    IsFinished = true;
                }
            }
            else {
                Debug.LogError($"Could not get TextMeshPro component from {owner.name}");
            }
        }
    }
}