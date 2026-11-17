using TMPro;
using UnityEngine;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;

public class CountUpBehavior : IFloatingBehavior {
    private readonly int target;
    private readonly int step;
    private readonly float duration;

    private float elapsed;
    private TextMeshPro text;

    public CountUpBehavior(int target, int step, float duration) {
        this.target = target;
        this.step = step;
        this.duration = duration;
    }

    // 효과가 시작될 때 한 번
    public override void InitializeBehavior(GameObject owner) {
        base.InitializeBehavior(owner);
        text = owner.GetComponent<TextMeshPro>();
        elapsed = 0f;
        SetValue(step);                                   // 처음엔 +10 부터
    }

    // 매 프레임: 지난 시간 비율만큼 숫자를 올린다
    public override void UpdateBehavior(float dt) {
        elapsed += dt;
        float t = Mathf.Clamp01(elapsed / duration);      // 0 ~ 1

        int stepCount = target / step;                    // 100 / 10 = 10칸
        int current = Mathf.Max(1, Mathf.CeilToInt(t * stepCount)) * step;
        SetValue(current);

        if (t >= 1f) {
            SetValue(target);                             // 마지막은 정확히 +100
            IsFinished = true;
        }
    }

    private void SetValue(int value) {
        if (text != null) text.text = "+" + value;
    }
}