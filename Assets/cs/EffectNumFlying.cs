using UnityEngine;
using BlackMassSoftware.FloatingTextEngine.Lite;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;

public static class EffectNumFlying {
    private static readonly Color scoreColor = new Color(1f, 0.85f, 0.2f);

    // 색을 직접 주는 버전
    public static void Show(Vector3 position, int score, Color color) {
        Camera cam = Camera.main;
        if (cam == null) return;

        FloatingTextEngine.CreateTextAtWorldSpace(position, "+10", color)
            .With(FloatingTextBehaviors.BillboardTextObject(EBillboardType.Full, cam.transform))
            .With(new CountUpBehavior(score, 10, 0.5f))                 //  0.5초 동안 +10 ~ +100
            .With(FloatingTextBehaviors.MoveUp(0.8f, 1.2f))             // 그동안 위로 떠오름
            .With(FloatingTextBehaviors.FadeOut(0.6f).DelayFor(0.6f));  // 숫자가 다 오른 뒤에 사라지기 시작
    }
    // 색을 안 주면 기본 금색 (예전처럼 Show(위치, 점수) 로도 쓸 수 있게)
    public static void Show(Vector3 position, int score) {
        Show(position, score, scoreColor);
    }
}
