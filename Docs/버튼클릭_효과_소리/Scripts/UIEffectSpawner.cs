using UnityEngine;

namespace UIFeedback {
    // [파티클] UI 위치에 파티클 효과를 하나 꺼낸다 (버튼·판넬이 같이 씀 → 같은 코드를 두 번 쓰지 않음)
    public static class UIEffectSpawner {
        // prefab        : 꺼낼 효과 (비어 있으면 아무것도 안 함)
        // target        : 효과가 나타날 UI (버튼, 판넬)
        // scale         : 효과 크기 (파티클 에셋은 보통 1~3m → World Space UI 에서는 0.2 ~ 0.5)
        // forwardOffset : UI 보다 카메라 쪽으로 얼마나 앞에 놓을지 (UI 뒤에 숨지 않게)
        // lifetime      : 이 시간(초) 뒤 자동 삭제 (0 이면 지우지 않음 — 효과가 스스로 지우는 경우)
        public static GameObject Spawn(GameObject prefab, Transform target, float scale, float forwardOffset, float lifetime) {
            if (prefab == null || target == null) return null;

            // 카메라 쪽 방향
            Camera cam = Camera.main;
            Vector3 toCamera = cam != null ? (cam.transform.position - target.position).normalized : -target.forward;

            // UI 의 자식으로 넣지 않음 (Canvas 크기 0.001 같은 값에 같이 작아지지 않게)
            GameObject fx = Object.Instantiate(prefab, target.position + toCamera * forwardOffset, target.rotation);
            fx.transform.localScale = Vector3.one * scale;

            // 멈춤(Time.timeScale = 0) 중에도 파티클이 움직이게
            foreach (ParticleSystem ps in fx.GetComponentsInChildren<ParticleSystem>(true)) {
                ParticleSystem.MainModule main = ps.main;
                main.useUnscaledTime = true;
            }

            if (lifetime > 0f) Object.Destroy(fx, lifetime);
            return fx;
        }
    }
}
