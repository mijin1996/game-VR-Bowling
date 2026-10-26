using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;
using BlackMassSoftware.FloatingTextEngine.Lite.Presets;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;


#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    using UnityEngine.InputSystem;
#endif


namespace BlackMassSoftware.FloatingTextEngine.Lite.Demo
{
    internal class DemoEntity3DController : MonoBehaviour
    {

        // click materials
        [SerializeField] private Material baseMaterial;
        [SerializeField] private Material leftClickMaterial;
        [SerializeField] private Material rightClickMaterial;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // check left click
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current.leftButton.wasPressedThisFrame) {
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonDown(0)) {
#endif
                // Cast our mouse ray and check what we hit
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out RaycastHit hitData, 1000);
#elif ENABLE_LEGACY_INPUT_MANAGER
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitData, 1000);
#endif
                if (hitData.collider != null && string.Compare(hitData.collider.gameObject.tag, "enemy", true) == 0) {
                    hitData.collider.GetComponent<MeshRenderer>().material = leftClickMaterial;
                    // make sure the collider is the one we hit not all of them
                    if (hitData.collider != null && hitData.collider == GetComponent<CapsuleCollider>()) {
                        Debug.Log("Mouse Ray Hit (left): " + hitData.collider.gameObject.name);

                        // calculate a fake crit chance from our UIController slider value
                        bool isCritical = Random.value <= UIController3D.instance.criticalChance ? true : false;
                        // lets generate a random damage value too.
                        float randomDamageValue = Random.Range(1, 2000);

                        // if its magic damage, just do that instead, otherwise normal damage 
                        CheckToggleSettingsAndApplyBehavior(randomDamageValue);

                        if (isCritical) {
                            FloatingTextEngine.CreateTextAtWorldSpace(transform.position, randomDamageValue, Color.forestGreen)
                            .WithPreset(FloatingTextPresets.Critical());
                        }
                    }
                }
            }


#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current.rightButton.wasPressedThisFrame) {
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonDown(1)) {
#endif
                // Cast our mouse ray and check what we hit
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out RaycastHit hitData, 1000);
#elif ENABLE_LEGACY_INPUT_MANAGER
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitData, 1000);
#endif
                if (hitData.collider != null && string.Compare(hitData.collider.gameObject.tag, "enemy", true) == 0) {
                    GetComponent<MeshRenderer>().material = rightClickMaterial;
                    // make sure the collider is the one we hit not all of them
                    Debug.Log("Mouse Ray Hit (right): " + hitData.collider.gameObject.name);

                    // calculate a fake crit chance from our UIController slider value
                    bool isCritical = Random.value <= UIController3D.instance.criticalChance ? true : false;
                    // lets generate a random damage value too.
                    float randomDamageValue = Random.Range(1, 2000);

                    CheckToggleSettingsAndApplyBehavior(randomDamageValue);
                    if (isCritical) {
                        FloatingTextEngine.CreateTextAtWorldSpace(transform.position, randomDamageValue)
                        .WithPreset(FloatingTextPresets.Critical());
                    }
                }
            }
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current.escapeKey.wasPressedThisFrame) {
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape)) {
#endif
                Application.Quit();
            }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (!Mouse.current.leftButton.isPressed && !Mouse.current.rightButton.isPressed) {
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
#endif
                GetComponent<MeshRenderer>().material = baseMaterial;
            }
        }

        private void CheckToggleSettingsAndApplyBehavior(float dmg)
        {
            // Create the initial floating text
            FloatingBehaviorEngine engine = FloatingTextEngine.CreateTextAtWorldSpace(transform.position, dmg);
            if (UIController3D.instance.colorToggle.isOn) {
                engine.With(FloatingTextBehaviors.ColorGradient(Color.yellow, Color.blue, 1f));
            }
            if (UIController3D.instance.expandShrinkToggle.isOn) {
                engine.With(FloatingTextBehaviors.ExpandShrink(2f, 0.6f, 2));
            }
            if (UIController3D.instance.fadeOutToggle.isOn) {
                engine.With(FloatingTextBehaviors.FadeOut(1f));
            }
            if (UIController3D.instance.moveUpArcToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUpArc3D(2f, 1.4f, 0.5f));
            }
            if (UIController3D.instance.moveUpToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUp(2f, 0.3f));
            }
            if (UIController3D.instance.moveUpExpandToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUpExpand(2f, 1f, 2f, 0.3f));
            }
            if (UIController3D.instance.popToggle.isOn) {
                engine.With(FloatingTextBehaviors.Pop(2f, 0.1f, 0.1f));
            }
            if (UIController3D.instance.scaleToToggle.isOn) {
                engine.With(FloatingTextBehaviors.ScaleTo(2f, 0.3f));
            }
            if (UIController3D.instance.shakeToggle.isOn) {
                engine.With(FloatingTextBehaviors.Shake3D(5f));
            }
            if (UIController3D.instance.shakeXToggle.isOn) {
                engine.With(FloatingTextBehaviors.ShakeX3D(3f));
            }
            if (UIController3D.instance.wobbleToggle.isOn) {
                engine.With(FloatingTextBehaviors.Wobble(3f));
            }
            if (UIController3D.instance.blinkToggle.isOn) {
                engine.With(FloatingTextBehaviors.Blink(0.2f, 3f));
            }
            if (UIController3D.instance.fadeInOutToggle.isOn) {
                engine.With(FloatingTextBehaviors.FadeInOut(1f, 5f));
            }
            if (UIController3D.instance.changeFontSizeToggle.isOn) {
                engine.With(FloatingTextBehaviors.ChangeFontSize(1f));
            }
            if (UIController3D.instance.currentBillboardType >= 0) {
                engine.With(FloatingTextBehaviors.BillboardTextObject(UIController3D.instance.currentBillboardType, Camera.main.transform));
            }
        }
    }
}