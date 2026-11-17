using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]   // 판넬 전체(자식 포함)를 한 번에 투명하게 하는 컴포넌트, 자동으로 같이 붙음
public class EffectPanelOpen : MonoBehaviour {
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float startScale = 0.8f;

    private CanvasGroup group;
    private Vector3 originalScale;

    [Header("움직임")]
    [SerializeField] private bool useMotion = true;

    [Header("파티클 효과 (비워 두면 없음)")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float effectScale = 0.5f;
    [SerializeField] private float forwardOffset = 0.1f;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    private void OnEnable() {
        SpawnEffect(effectPrefab, transform, effectScale, forwardOffset);   // 파티클
        if (!useMotion) return;                                             // 움직임 끄면 여기서 끝
        StopAllCoroutines();
        StartCoroutine(Open());                                             // 움직임
    }

    private void OnDisable() {
        transform.localScale = originalScale;   // 효과 도중에 꺼져도 원래대로
        if (group != null) group.alpha = 1f;
    }

    private IEnumerator Open() {
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime) {
            float p = t / duration;
            float ease = 1f - (1f - p) * (1f - p);   // 처음엔 빠르게, 끝은 부드럽게
            transform.localScale = originalScale * Mathf.Lerp(startScale, 1f, ease);
            group.alpha = ease;
            yield return null;
        }
        transform.localScale = originalScale;
        group.alpha = 1f;
    }

    // UI 위치에 파티클 효과를 하나 꺼낸다 (EffectButtonPress 도 이 함수를 같이 씀 → 같은 코드를 두 번 쓰지 않음)
    // prefab        : 꺼낼 효과 (비어 있으면 아무것도 안 함)
    // target        : 효과가 나타날 UI (판넬이나 버튼)
    // scale         : 효과 크기 (JMO 효과는 1~3m 라서 UI 에 맞게 줄여야 함)
    // forwardOffset : UI 보다 카메라 쪽으로 얼마나 앞에 놓을지 (뒤에 숨지 않게)
    public static void SpawnEffect(GameObject prefab, Transform target, float scale, float forwardOffset) {
        if (prefab == null || target == null) return;

        // 카메라 쪽 방향 구하기
        Vector3 toCamera = Camera.main != null
            ? (Camera.main.transform.position - target.position).normalized
            : -target.forward;

        // UI 의 자식으로 넣지 않음 (캔버스 크기 0.01 에 같이 작아지지 않게)
        GameObject fx = Instantiate(prefab, target.position + toCamera * forwardOffset, target.rotation);
        fx.transform.localScale = Vector3.one * scale;
        Debug.Log("[Effect] " + prefab.name + " 생성 / 위치 " + fx.transform.position + " / 크기 " + scale, fx);   // 확인용 (나중에 지워도 됨)

        // 멈춤(timeScale 0) 중에도 효과가 움직이게
        foreach (ParticleSystem ps in fx.GetComponentsInChildren<ParticleSystem>()) {
            ParticleSystem.MainModule main = ps.main;
            main.useUnscaledTime = true;
        }
        // 다 끝난 효과는 JMO 의 CFXR_Effect 가 스스로 지운다 (Clear Behavior : Destroy)
    }
}