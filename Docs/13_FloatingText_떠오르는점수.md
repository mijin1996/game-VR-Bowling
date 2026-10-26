# Floating Text — 핀 위로 떠오르는 점수 숫자

> 핀이 쓰러지면 그 핀 위에 **+10 → +20 → … → +100** 으로 올라가며 떠오르다 사라지는 숫자.
> 외부 에셋 **Floating Text Engine Lite** (BlackMassSoftware) 를 사용했습니다.
> 예전 기록 `FloatingText_적용기록.md`(프로젝트 맨 위) 의 `ScorePopup` 은 지금 **`EffectNumFlying`** 으로 이름이 바뀌었습니다.

---

## 1. 흐름

```
공이 핀을 침
   ↓
Pin.MarkFallen()                 "나 쓰러졌어" (머리 쪽 Trigger 가 Floor 에 닿음 또는 45° 이상 기울어짐)
   ├─ SoundManager.PlaySFX(hitClip)               "딱" 소리
   └─ GameManager.OnPinFallen(핀 머리 위치)
         ├─ AddScore(100)                          점수판 +100 (+ 점수 글자 톡 효과)
         └─ EffectNumFlying.Show(위치, 100, 금색)
               ↓
            FloatingTextEngine.CreateTextAtWorldSpace(...)   숫자 오브젝트를 풀에서 꺼냄
               ↓ .With(...) 효과 4개를 동시에
            Billboard    항상 플레이어(Main Camera)를 바라봄
            CountUp      0.5초 동안 +10 → +100      ← 직접 만든 효과
            MoveUp       1.2초 동안 0.8m 위로
            FadeOut      0.6초 기다렸다가 0.6초 동안 사라짐
               ↓
            효과가 다 끝나면 자동으로 풀에 반납 (다시 씀)
```

---

## 2. 역할 나누기

| 스크립트 | 하는 일 |
|---|---|
| `Pin` | 쓰러졌다는 사실과 **머리 위치**만 알림 (`TopPosition()` = 렌더러 맨 위 + 15cm) |
| `GameManager` | 점수 값(**Pin Score** 100), 숫자 색(**Popup Color** 금색) 결정 → 숫자 띄우기 부탁 |
| `EffectNumFlying` | 어떤 효과를 어떤 순서로 조합할지 결정 — **에셋을 아는 유일한 곳** |
| `CountUpBehavior` | 숫자를 10씩 올리는 효과 (에셋에 없어서 직접 만듦) |
| FloatingTextEngine (에셋) | 숫자 오브젝트 만들기·재사용(오브젝트 풀) |

- Pin 과 GameManager 는 에셋을 몰라도 됩니다. 에셋을 바꾸면 `EffectNumFlying` 만 고치면 됨.
- 에셋 코드는 한 줄도 고치지 않고, 에셋의 효과 틀(`IFloatingBehavior`)을 상속해서 새 효과를 끼움.

---

## 3. 코드

### `Assets/cs/EffectNumFlying.cs` (static — 오브젝트에 붙이지 않음)
```csharp
public static void Show(Vector3 position, int score, Color color) {
    Camera cam = Camera.main;
    if (cam == null) return;

    FloatingTextEngine.CreateTextAtWorldSpace(position, "+10", color)
        .With(FloatingTextBehaviors.BillboardTextObject(EBillboardType.Full, cam.transform))
        .With(new CountUpBehavior(score, 10, 0.5f))                 // 0.5초 동안 +10 ~ +100
        .With(FloatingTextBehaviors.MoveUp(0.8f, 1.2f))             // 그동안 위로 떠오름
        .With(FloatingTextBehaviors.FadeOut(0.6f).DelayFor(0.6f));  // 숫자가 다 오른 뒤 사라지기 시작
}
```

### `Assets/cs/CountUpBehavior.cs` (에셋 효과와 같은 틀)
```csharp
public class CountUpBehavior : IFloatingBehavior {
    public CountUpBehavior(int target, int step, float duration) { ... }

    public override void InitializeBehavior(GameObject owner) {   // 시작할 때 한 번
        base.InitializeBehavior(owner);
        text = owner.GetComponent<TextMeshPro>();
        SetValue(step);                                            // +10 부터
    }

    public override void UpdateBehavior(float dt) {                // 매 프레임
        elapsed += dt;
        float t = Mathf.Clamp01(elapsed / duration);
        int current = Mathf.Max(1, Mathf.CeilToInt(t * (target / step))) * step;
        SetValue(current);
        if (t >= 1f) { SetValue(target); IsFinished = true; }      // 마지막은 정확히 +100
    }
}
```

### `GameManager.cs`
```csharp
public void OnPinFallen(Vector3 position) {
    AddScore(pinScore);                                    // 점수 +100
    EffectNumFlying.Show(position, pinScore, popupColor);  // 숫자 띄우기
}
```

---

## 4. 씬 설정
1. Hierarchy 에 **`FloatingTextEngine`** 오브젝트가 있어야 합니다 (에셋의 엔진 — 숫자 오브젝트 풀 관리).
2. 플레이어 카메라(`Complete XR Origin … > Main Camera`) 의 Tag 가 **MainCamera** 여야 Billboard 가 플레이어를 바라봅니다.
3. 숫자 크기·글꼴은 FloatingTextEngine 쪽 설정(에셋)에서 바꿉니다.

## 5. 바꾸고 싶을 때
| 바꿀 것 | 위치 |
|---|---|
| 점수 값 (100) | GameManager → **Pin Score** (숫자도 자동으로 그 값까지 올라감) |
| 숫자 색 | GameManager → **Popup Color** |
| 올라가는 간격·시간 | `EffectNumFlying.cs` 의 `new CountUpBehavior(score, 10, 0.5f)` |
| 떠오르는 높이·시간 | `MoveUp(0.8f, 1.2f)` (높이 m, 시간 초) |
| 사라지는 시간 | `FadeOut(0.6f).DelayFor(0.6f)` |
| 숫자 위치 | `Pin.TopPosition()` 의 `+ 0.15f` |

## 6. 안 보일 때
| 증상 | 확인 |
|---|---|
| 숫자가 아예 안 나옴 | Hierarchy 에 FloatingTextEngine 있는지, Console 에 에러 |
| 숫자가 옆이나 뒤를 봄 | 카메라 Tag = MainCamera |
| 점수는 오르는데 숫자만 없음 | 핀 위치가 카메라 뒤/너무 먼지, 글자 크기 |
