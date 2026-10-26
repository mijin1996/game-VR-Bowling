using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIFeedback {
    // [버튼 느낌] 버튼·토글을 누를 때의 느낌을 한 곳에서 책임진다 — 버튼마다 붙이기만 하면 끝
    //   ① 클릭 소리   ② 눌림 움직임 (작아졌다 돌아옴)   ③ 파티클
    // - 같은 오브젝트의 Button / Toggle 을 스스로 찾아 연결 → On Click 에 따로 넣을 필요 없음
    // - Button 이 꺼져 있으면(Interactable 해제) 반응하지 않음
    // - 버튼이 "무엇을 하는지"(게임 시작 등)는 Button 의 On Click 에 내 스크립트를 드래그해서 연결
    // - Button/Toggle 이 아닌 것(3D 물체, XR 버튼 등)은 이벤트에 Play() 를 연결해서 사용
    [DisallowMultipleComponent]
    public class UIButtonFeedback : MonoBehaviour {
        [Header("소리 (비워 두면 UIFeedbackDefaults 의 기본 클릭 소리)")]
        [SerializeField] private AudioClip clickSound;
        [Range(0f, 1f)] [SerializeField] private float volume = 1f;

        [Header("움직임")]
        [SerializeField] private bool useMotion = true;
        [Tooltip("가장 작아질 때 크기 (0.9 = 90%)")]
        [SerializeField] private float pressScale = 0.9f;
        [Tooltip("줄었다 돌아오는 시간 (초)")]
        [SerializeField] private float duration = 0.15f;

        [Header("파티클 (비워 두면 UIFeedbackDefaults 의 기본 효과, 그것도 없으면 효과 없음)")]
        [SerializeField] private GameObject effectPrefab;
        [Tooltip("World Space UI 는 0.2 ~ 0.5 정도")]
        [SerializeField] private float effectScale = 0.3f;
        [SerializeField] private float forwardOffset = 0.05f;
        [Tooltip("이 시간(초) 뒤 효과 자동 삭제 (0 = 지우지 않음)")]
        [SerializeField] private float effectLifetime = 3f;

        private Vector3 originalScale;

        private void Awake() {
            originalScale = transform.localScale;

            Button button = GetComponent<Button>();
            if (button != null) button.onClick.AddListener(Play);

            Toggle toggle = GetComponent<Toggle>();
            if (toggle != null) toggle.onValueChanged.AddListener(_ => Play());
        }

        private void OnDisable() {
            transform.localScale = originalScale;   // 누르자마자 판넬이 닫혀도 크기가 꼬이지 않게
        }

        // 클릭 느낌 한 번 (다른 이벤트에서도 이 함수를 연결해서 쓸 수 있음)
        public void Play() {
            UIFeedbackDefaults d = UIFeedbackDefaults.Instance;

            // ① 소리
            AudioClip clip = clickSound != null ? clickSound : (d != null ? d.clickSound : null);
            UIAudio.Play(clip, volume);

            // ③ 파티클
            GameObject fx = effectPrefab != null ? effectPrefab : (d != null ? d.clickEffect : null);
            UIEffectSpawner.Spawn(fx, transform, effectScale, forwardOffset, effectLifetime);

            // ② 움직임
            if (!useMotion || !isActiveAndEnabled) return;
            StopAllCoroutines();
            StartCoroutine(Press());
        }

        private IEnumerator Press() {
            // unscaledDeltaTime : 멈춤(timeScale 0) 중에도 움직이게
            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime) {
                float s = 1f - (1f - pressScale) * Mathf.Sin(t / duration * Mathf.PI);   // 줄었다가 원래대로
                transform.localScale = originalScale * s;
                yield return null;
            }
            transform.localScale = originalScale;
        }
    }
}
