using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;
using BlackMassSoftware.FloatingTextEngine.Lite.Demo;
using BlackMassSoftware.FloatingTextEngine.Lite.Presets;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    using UnityEngine.InputSystem;
#endif


namespace BlackMassSoftware.FloatingTextEngine.Lite.Demo
{
    internal class DemoEnemyController : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            // This will spawn the text with slightly varied x axis offsets.  Can be set through the inspector
            // FloatingTextEngine.instance.EnableSpatialXVariance(true);
        }

        // Update is called once per frame
        private void Update()
        {
            // check left click
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current.leftButton.wasPressedThisFrame) {
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonDown(0)) {
#endif
                // Cast our mouse ray and check what we hit
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                Vector3 mouseRay = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
#elif ENABLE_LEGACY_INPUT_MANAGER
                Vector3 mouseRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
                RaycastHit2D mouseRayHit = Physics2D.Raycast(mouseRay, Vector2.zero);
                if (mouseRayHit.collider != null && string.Compare(mouseRayHit.collider.gameObject.tag, "enemy", true) == 0) {
                    // make sure the collider is the one we hit not all of them
                    if (mouseRayHit.collider == GetComponent<BoxCollider2D>()) {
                        Debug.Log("Mouse Ray Hit: " + mouseRayHit.collider.gameObject.name);

                        // calculate a fake crit chance from our UIController slider value
                        bool isCritical = Random.value <= UIController.mInstance.criticalChance ? true : false;
                        // lets generate a random damage value too.
                        float randomDamageValue = Random.Range(1, 2000);

                        // if its magic damage, just do that instead, otherwise normal damage 
                        if (UIController.mInstance.currentDamageType >= 0) {
                            CreateDamageTypeText(randomDamageValue);
                        }
                        else {
                            CheckToggleSettingsAndApplyBehavior(randomDamageValue);
                        }

                        if (isCritical) {
                            FloatingTextEngine.CreateTextAtScreenSpace(transform.position, randomDamageValue, Color.white)
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
                Vector3 mouseRay = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
#elif ENABLE_LEGACY_INPUT_MANAGER
                Vector3 mouseRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
                RaycastHit2D mouseRayHit = Physics2D.Raycast(mouseRay, Vector2.zero);
                if (mouseRayHit.collider != null && string.Compare(mouseRayHit.collider.gameObject.tag, "enemy", true) == 0) {
                    // make sure the collider is the one we hit not all of them
                    Debug.Log("Mouse Ray Hit: " + mouseRayHit.collider.gameObject.name);

                    // calculate a fake crit chance from our UIController slider value
                    bool isCritical = Random.value <= UIController.mInstance.criticalChance ? true : false;
                    // lets generate a random damage value too.
                    float randomDamageValue = Random.Range(1, 2000);

                    // if its magic damage, just do that instead, otherwise normal damage 
                    if (UIController.mInstance.currentDamageType >= 0) {
                        CreateDamageTypeText(randomDamageValue);
                    }
                    else {
                        CheckToggleSettingsAndApplyBehavior(randomDamageValue);
                    }   

                    if (isCritical) {
                        FloatingTextEngine.CreateTextAtScreenSpace(transform.position, randomDamageValue, Color.white)
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
        }

        private void CheckToggleSettingsAndApplyBehavior(float dmg)
        {
            // Create the initial floating text
            FloatingBehaviorEngine engine = FloatingTextEngine.CreateTextAtScreenSpace(transform.position, dmg, Color.white);
            if (UIController.mInstance.colorToggle.isOn) {
                engine.With(FloatingTextBehaviors.ColorGradient(Color.yellow, Color.blue, 1f));
            }
            if (UIController.mInstance.expandShrinkToggle.isOn) {
                engine.With(FloatingTextBehaviors.ExpandShrink(2f, 0.6f, 2));
            }
            if (UIController.mInstance.fadeOutToggle.isOn) {
                engine.With(FloatingTextBehaviors.FadeOut(1f));
            }
            if (UIController.mInstance.moveUpArcToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUpArc2D(2f, 1.4f, 1f));
            }
            if (UIController.mInstance.moveUpToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUp(2f, 1f));
            }
            if (UIController.mInstance.moveUpExpandToggle.isOn) {
                engine.With(FloatingTextBehaviors.MoveUpExpand(2f, 1f, 2f, 0.3f));
            }
            if (UIController.mInstance.popToggle.isOn) {
                engine.With(FloatingTextBehaviors.Pop(2f, 0.1f, 0.1f));
            }
            if (UIController.mInstance.scaleToToggle.isOn) {
                engine.With(FloatingTextBehaviors.ScaleTo(2f, 0.3f));
            }
            if (UIController.mInstance.shakeToggle.isOn) {
                engine.With(FloatingTextBehaviors.Shake2D(5f));
            }
            if (UIController.mInstance.shakeXToggle.isOn) {
                engine.With(FloatingTextBehaviors.ShakeX2D(3f));
            }
            if (UIController.mInstance.wobbleToggle.isOn) {
                engine.With(FloatingTextBehaviors.Wobble(3f));
            }
            if (UIController.mInstance.blinkToggle.isOn) {
                engine.With(FloatingTextBehaviors.Blink(0.2f, 3f));
            }
            if (UIController.mInstance.fadeInOutToggle.isOn) {
                engine.With(FloatingTextBehaviors.FadeInOut(1f, 5f));
            }
            if (UIController.mInstance.changeFontSizeToggle.isOn) {
                engine.With(FloatingTextBehaviors.ChangeFontSize(1f));
            }
        }

        private void CreateDamageTypeText(float dmg)
        {
            if (UIController.mInstance.currentDamageType >= 0) {
                FloatingTextEngine.CreateTextAtScreenSpace(transform.position, dmg, Color.white)
                .WithPreset(FloatingTextPresets.MagicDamage(UIController.mInstance.currentDamageType));
            }
        }
    }
}