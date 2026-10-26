using BlackMassSoftware.FloatingTextEngine.Lite;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;
using BlackMassSoftware.FloatingTextEngine.Lite.Presets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackMassSoftware.FloatingTextEngine.Lite.Demo
{

    public class UIController3D : MonoBehaviour
    {
        public static UIController3D instance;
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
        public EBillboardType currentBillboardType;


        void Awake()
        {
            if (instance != null) {
                Destroy(this);
                return;
            }

            instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            critLabel = panelBackground.transform.Find("CritLabel").GetComponent<TextMeshProUGUI>();
            critSlider = panelBackground.transform.Find("CritSlider").GetComponent<Slider>();
            critSlider.onValueChanged.AddListener(OnCritSliderValueUpdate);
            InitializeToggleButtons();
            InitializeDamageTypeDropdown();
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
            damageTypeDropdown = panelBackground.transform.Find("BillboardTypes").GetComponent<TMP_Dropdown>();
            damageTypeDropdown.onValueChanged.AddListener(OnBillboardTypeDropdownValueChanged);
            damageTypeDropdown.value = -1;
        }

        private void OnBillboardTypeDropdownValueChanged(int idx)
        {
            Debug.Log($"Billboard Type index {idx}");
            currentBillboardType = (EBillboardType)(idx);
        }
    }

}