using UnityEngine;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Behaviors
{

    /// <summary>
    /// Enum that determines billboard behavior.
    /// </summary>
    public enum EBillboardType
    {
        /// <summary>
        /// No billboarding enabled.
        /// </summary>
        None,
        /// <summary>
        /// Full rotation, no constraints.
        /// </summary>
        Full,
        /// <summary>
        /// Rotation around the Y axis.
        /// </summary>
        YAxisOnly
    }

    /// <summary>
    /// This behavior causes text to billboard towards camera based on provided mode.
    /// </summary>
    public class BillboardBehavior : IFloatingBehavior
    {
        /// <summary>
        /// Billboarding type, None, Full, YAxisOnly.
        /// </summary>
        private EBillboardType billboardType;
        /// <summary>
        /// The eye (camera) transform we're going to look at.
        /// </summary>
        private Transform eyeTransform;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="type">The billboard type. None, Full, YAxisOnly</param>
        /// <param name="eye">The eye (camera) transform we're going to look at.</param>
        public BillboardBehavior(EBillboardType type, Transform eye)
        {
            billboardType = type;
            eyeTransform = eye;
            // Flag this as a finished behavior.
            // This has no "finished" condition so whenever the rest finish allow the object to be removed.
            IsPersistentBehavior = true; 
        }

        /// <summary>
        /// Initialzes the behavior
        /// </summary>
        /// <param name="owner">The owning floating text GameObject.</param>
        public override void InitializeBehavior(GameObject owner)
        {
            base.InitializeBehavior(owner);
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="dt">The delta time</param>
        public override void UpdateBehavior(float dt)
        {
            DoBillboardFromType();
        }

        /// <summary>
        /// Method used to determine and set billboard transforms.
        /// </summary>
        private void DoBillboardFromType()
        {
            if (eyeTransform == null) return;
            if (billboardType == EBillboardType.Full) {
                Vector3 dir = owner.transform.position - eyeTransform.position;
                owner.transform.rotation = Quaternion.LookRotation(dir);
            }
            else if (billboardType == EBillboardType.YAxisOnly) {
                Vector3 dir = owner.transform.position - eyeTransform.position;
                dir.y = 0f; // lock
                if (dir.sqrMagnitude < 0.001f) return;
                owner.transform.rotation = Quaternion.LookRotation(dir);
            }

        }
    }

}