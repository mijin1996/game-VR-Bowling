using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EffectButtonPress : MonoBehaviour {
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 originalScale;

    [Header("움직임")]
    [SerializeField] private bool useMotion = true;

    [Header("파티클 효과 (비워 두면 없음)")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float effectScale = 0.5f;
    [SerializeField] private float forwardOffset = 0.1f;
    
    private void Awake() {
        originalScale = transform.localScale;

        Button button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(Play);

        Toggle toggle = GetComponent<Toggle>();
        if (toggle != null) toggle.onValueChanged.AddListener(isOn => Play());
    }

    private void OnDisable() {
        transform.localScale = originalScale;   // 누르자마자 판넬이 닫혀도 크기가 꼬이지 않게
    }

    public void Play() {
        EffectPanelOpen.SpawnEffect(effectPrefab, transform, effectScale, forwardOffset);   // 파티클 (함수는 EffectPanelOpen 것을 같이 씀)
        if (!useMotion || !isActiveAndEnabled) return;                                      // 움직임 끄거나 이미 꺼졌으면 끝
        StopAllCoroutines();
        StartCoroutine(Press());                                                            // 움직임
    }

    private IEnumerator Press() {
        // unscaledDeltaTime : 멈춤(timeScale 0) 중에도 효과가 움직이게
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime) {
            float s = 1f - (1f - pressScale) * Mathf.Sin(t / duration * Mathf.PI);   // 줄었다가 다시 원래대로
            transform.localScale = originalScale * s;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}