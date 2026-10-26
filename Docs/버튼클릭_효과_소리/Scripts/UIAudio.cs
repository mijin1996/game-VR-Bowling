using UnityEngine;

namespace UIFeedback {
    // [소리] UI 소리를 재생하는 공용 창구 (씬에 따로 놓을 필요 없음)
    // - 처음 소리를 낼 때 숨은 AudioSource 하나를 스스로 만들고, 씬이 바뀌어도 계속 씀
    // - 2D 소리(거리와 상관없이 같은 크기), 여러 소리가 겹쳐도 됨 (PlayOneShot)
    // - SoundManager 같은 다른 스크립트가 없어도 동작한다
    public static class UIAudio {
        // 모든 UI 소리 크기 (0 ~ 1). 설정 화면 같은 곳에서 바꿔도 됨
        public static float MasterVolume = 1f;

        private static AudioSource source;

        public static void Play(AudioClip clip, float volume = 1f) {
            if (clip == null) return;
            if (source == null) CreateSource();
            source.PlayOneShot(clip, Mathf.Clamp01(volume) * Mathf.Clamp01(MasterVolume));
        }

        private static void CreateSource() {
            GameObject go = new GameObject("[UIAudio]");
            Object.DontDestroyOnLoad(go);
            source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;           // 2D
            source.ignoreListenerPause = true;  // 멈춤 메뉴(AudioListener.pause) 중에도 버튼 소리
        }
    }
}
