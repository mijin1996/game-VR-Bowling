using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BlackMassSoftware.FloatingTextEngine.Lite;
using BlackMassSoftware.FloatingTextEngine.Lite.Presets;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Demo
{
    internal class UIController : MonoBehaviour
    {
        public static UIController mInstance;

        // vars
        public float criticalChance;

        // ui controls
        [SerializeField] public RectTransform panelBackground;
        private TextMeshProUGUI critLabel;
        private Slider critSlider;

        // toggle buttons
        public Toggle colorToggle;
        public Toggle expandShrinkToggle;
        public Toggle fadeOutToggle;
        public Toggle moveUpArcToggle;
        public Toggle moveUpToggle;
        public Toggle moveUpExpandToggle;
        public Toggle popToggle;
        public Toggle scaleToToggle;
        public Toggle shakeToggle;
        public Toggle shakeXToggle;
        public Toggle wobbleToggle;
        public Toggle blinkToggle;
        public Toggle fadeInOutToggle;
        public Toggle changeFontSizeToggle;

        // damage type drop down
        public TMP_Dropdown damageTypeDropdown;
        public EFloatingDamageType currentDamageType;

        // heal button
        private Button healButton;

        // block button
        private Button blockButton;

        private void Awake()
        {
            mInstance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            critLabel = panelBackground.transform.Find("CritLabel").GetComponent<TextMeshProUGUI>();
            critSlider = panelBackground.transform.Find("CritSlider").GetComponent<Slider>();
            critSlider.onValueChanged.AddListener(OnCritSliderValueUpdate);
            InitializeToggleButtons();
            InitializeDamageTypeDropdown();
            InitializeHealButton();
            InitializeBlockButton();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCritSliderValueUpdate(float val)
        {
            criticalChance = val;
            critLabel.text = $"Critical Chance %: {Mathf.RoundToInt(criticalChance * 100.0f)}";
        }

        private void InitializeToggleButtons()
        {
            colorToggle = panelBackground.transform.Find("ColorGradient").GetComponent<Toggle>();
            expandShrinkToggle = panelBackground.transform.Find("ExpandShrink").GetComponent<Toggle>();
            fadeOutToggle = panelBackground.transform.Find("FadeOut").GetComponent<Toggle>();
            moveUpArcToggle = panelBackground.transform.Find("MoveUpArc").GetComponent<Toggle>();
            moveUpToggle = panelBackground.transform.Find("MoveUp").GetComponent<Toggle>();
            moveUpExpandToggle = panelBackground.transform.Find("MoveUpExpand").GetComponent<Toggle>();
            popToggle = panelBackground.transform.Find("Pop").GetComponent<Toggle>();
            scaleToToggle = panelBackground.transform.Find("ScaleTo").GetComponent<Toggle>();
            shakeToggle = panelBackground.transform.Find("Shake").GetComponent<Toggle>();
            shakeXToggle = panelBackground.transform.Find("ShakeX").GetComponent<Toggle>();
            wobbleToggle = panelBackground.transform.Find("Wobble").GetComponent<Toggle>();
            blinkToggle = panelBackground.transform.Find("Blink").GetComponent<Toggle>();
            fadeInOutToggle = panelBackground.transform.Find("FadeInOut").GetComponent<Toggle>();
            changeFontSizeToggle = panelBackground.transform.Find("ChangeFontSize").GetComponent<Toggle>();
        }

        private void InitializeDamageTypeDropdown()
        {
            damageTypeDropdown = panelBackground.transform.Find("DamageTypes").GetComponent<TMP_Dropdown>();
            damageTypeDropdown.onValueChanged.AddListener(OnDamageTypeDropdownValueChanged);
            damageTypeDropdown.value = -1;
        }

        private void OnDamageTypeDropdownValueChanged(int idx)
        {
            Debug.Log($"Damage Type index {idx - 1}");
            currentDamageType = (EFloatingDamageType)(idx - 1);
        }

        private void InitializeHealButton()
        {
            healButton = panelBackground.transform.Find("HealButton").GetComponent<Button>();
            healButton.onClick.AddListener(OnHealButtonClicked);
        }

        private void InitializeBlockButton()
        {
            blockButton = panelBackground.transform.Find("BlockButton").GetComponent<Button>();
            blockButton.onClick.AddListener(OnBlockButtonClicked);
        }

        private void OnHealButtonClicked()
        {
            FloatingTextEngine.CreateTextAtScreenSpace(Vector3.zero, 100, Color.white)
            .WithPreset(FloatingTextPresets.Heal());
        }

        private void OnBlockButtonClicked()
        {
            FloatingTextEngine.CreateTextAtScreenSpace(Vector3.zero, "BLOCK", Color.white)
            .WithPreset(FloatingTextPresets.Block());
        }
    }

}