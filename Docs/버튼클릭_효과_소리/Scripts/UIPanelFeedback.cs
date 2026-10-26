using System.Collections;
using UnityEngine;

namespace UIFeedback {
    // [판넬 느낌] 판넬(창)이 열리고 닫힐 때의 느낌을 한 곳에서 책임진다 — 판넬에 붙이기만 하면 끝
    //   열릴 때 (오브젝트가 켜질 때 자동): 열림 소리 + 커지며 나타남 + 파티클
    //   닫을 때 (Close() 를 버튼 On Click 에 연결): 닫힘 소리 + 작아지며 사라짐 → 꺼짐
    // - 판넬을 SetActive(true) 로 켜기만 해도 열림 효과가 나온다 (코드 어디서 켜든 상관없음)
    // - CanvasGroup 이 자동으로 같이 붙음 (판넬 전체를 한 번에 투명하게)
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanelFeedback : MonoBehaviour {
        [Header("소리 (비워 두면 UIFeedbackDefaults 의 기본 소리)")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [Range(0f, 1f)] [SerializeField] private float volume = 1f;

        [Header("움직임")]
        [SerializeField] private bool useMotion = true;
        [Tooltip("나타나고 사라지는 시간 (초)")]
        [SerializeField] private float duration = 0.25f;
        [Tooltip("처음(또는 사라질 때) 크기 (0.8 = 80%)")]
        [SerializeField] private float startScale = 0.8f;

        [Header("열릴 때 파티클 (비워 두면 UIFeedbackDefaults 의 기본 효과, 그것도 없으면 효과 없음)")]
        [SerializeField] private GameObject openEffect;
        [SerializeField] private float effectScale = 0.5f;
        [SerializeField] private float forwardOffset = 0.1f;
        [SerializeField] private float effectLifetime = 3f;

        private CanvasGroup group;
        private Vector3 originalScale;
        private bool closing;

        private void Awake() {
            group = GetComponent<CanvasGroup>();
            originalScale = transform.localScale;
        }

        // 켜질 때마다 열림 효과
        private void OnEnable() {
            closing = false;
            UIFeedbackDefaults d = UIFeedbackDefaults.Instance;

            UIAudio.Play(openSound != null ? openSound : (d != null ? d.openSound : null), volume);

            GameObject fx = openEffect != null ? openEffect : (d != null ? d.openEffect : null);
            UIEffectSpawner.Spawn(fx, transform, effectScale, forwardOffset, effectLifetime);

            if (!useMotion) return;
            StopAllCoroutines();
            StartCoroutine(Animate(true));
        }

        private void OnDisable() {
            transform.localScale = originalScale;   // 효과 도중에 꺼져도 원래대로
            if (group != null) group.alpha = 1f;
        }

        // 버튼 On Click 에 연결: 닫힘 소리 + 사라지는 효과 → 끝나면 꺼짐
        public void Close() {
            if (!gameObject.activeInHierarchy || closing) return;
            closing = true;
            UIFeedbackDefaults d = UIFeedbackDefaults.Instance;
            UIAudio.Play(closeSound != null ? closeSound : (d != null ? d.closeSound : null), volume);

            if (!useMotion) { gameObject.SetActive(false); return; }
            StopAllCoroutines();
            StartCoroutine(Animate(false));
        }

        // 버튼 하나로 열고 닫기 (On Click 에 연결)
        public void Toggle() {
            if (gameObject.activeSelf && !closing) Close();
            else gameObject.SetActive(true);
        }

        private IEnumerator Animate(bool opening) {
            // unscaledDeltaTime : 멈춤(timeScale 0) 중에도 움직이게
            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime) {
                float p = t / duration;
                float ease = 1f - (1f - p) * (1f - p);              // 처음 빠르고 끝은 부드럽게
                float k = opening ? ease : 1f - ease;               // 열림: 0→1, 닫힘: 1→0
                transform.localScale = originalScale * Mathf.Lerp(startScale, 1f, k);
                group.alpha = k;
                yield return null;
            }
            transform.localScale = originalScale;
            group.alpha = 1f;
            if (!opening) gameObject.SetActive(false);
        }
    }
}
