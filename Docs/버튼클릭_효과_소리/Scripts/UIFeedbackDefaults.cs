using UnityEngine;

namespace UIFeedback {
    // [기본값] 씬에 하나만 두면, 소리·효과 칸이 비어 있는 모든 버튼/판넬이 이 기본값을 쓴다 (없어도 동작함)
    // - 버튼마다 같은 클릭 소리를 일일이 넣지 않아도 됨
    // - 특정 버튼만 다른 소리를 쓰고 싶으면 그 버튼의 칸에만 넣으면 됨 (버튼 칸이 우선)
    // 사용: 빈 오브젝트(예: "UIFeedback") 에 붙이고 소리·효과를 드래그
    [DefaultExecutionOrder(-100)]   // 버튼·판넬보다 먼저 준비되게
    public class UIFeedbackDefaults : MonoBehaviour {
        public static UIFeedbackDefaults Instance { get; private set; }

        [Header("기본 소리")]
        public AudioClip clickSound;        // 버튼 클릭
        public AudioClip openSound;         // 판넬 열림
        public AudioClip closeSound;        // 판넬 닫힘

        [Header("기본 파티클 (비워 두면 없음)")]
        public GameObject clickEffect;
        public GameObject openEffect;

        [Header("전체 UI 소리 크기")]
        [Range(0f, 1f)] public float masterVolume = 1f;

        private void Awake() {
            Instance = this;
            UIAudio.MasterVolume = masterVolume;
        }

        private void OnValidate() {
            UIAudio.MasterVolume = masterVolume;   // Inspector 에서 바꾸면 바로 적용
        }

        private void OnDestroy() {
            if (Instance == this) Instance = null;
        }
    }
}
