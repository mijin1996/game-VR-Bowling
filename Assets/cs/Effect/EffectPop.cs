using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectPop : MonoBehaviour, ITextPopEffect {
    public float popScale = 1.4f;
    public float effectDuration = 0.3f;
    public Color flashColor = Color.yellow;
    public int repeat = 1;

    private Graphic graphic;
    private Color originalColor;
    private Vector3 originalScale;

    private void Awake() {
        graphic = GetComponent<Graphic>();
        originalColor = graphic.color;
        originalScale = transform.localScale;
    }

    // 효과 도중에 오브젝트가 꺼지면 코루틴이 멈춰서 노란색·큰 크기로 굳어 버린다 → 꺼질 때 원래대로 돌려놓기
    private void OnDisable() {
        if (graphic == null) return;
        transform.localScale = originalScale;
        graphic.color = originalColor;
    }

    public void PlayPop() {
        StopAllCoroutines();
        StartCoroutine(Pop());
    }

    private IEnumerator Pop() {
        for (int i = 0; i < repeat; i++) {
            for (float t = 0f; t < effectDuration; t += Time.deltaTime) {
                float p = t / effectDuration;
                transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, p);
                graphic.color = Color.Lerp(flashColor, originalColor, p);
                yield return null;
            }
        }
        transform.localScale = originalScale;
        graphic.color = originalColor;
    }
}