# 🎳 핀 점수 숫자 효과 (Floating Text) 적용 기록

## 1. 무엇을 만들었나

핀이 쓰러지면

1. "탁" 소리가 나고
2. 점수판에 **+100** 이 더해지고
3. 쓰러진 핀 위에 숫자가 **+10 → +20 → … → +100** 으로 올라가며 떠오르다가 사라져요.

```
공이 핀을 침
   ↓
Pin.OnTriggerEnter            "나 쓰러졌어" (Floor 태그에 닿음)
   ├─ SoundManager.PlaySFX(hitClip)          "탁" 소리
   └─ GameManager.OnPinFallen(핀 머리 위치)
         ├─ AddScore(pinScore)                점수판 +100
         └─ ScorePopup.Show(위치, 100, 색)
               ↓
            FloatingTextEngine (에셋)         숫자 오브젝트를 풀에서 꺼냄
               ↓ .With(...) 효과 4개 동시에
            Billboard   항상 나(카메라)를 바라봄
            CountUp     0.5초 동안 +10 ~ +100   ← 직접 만든 효과
            MoveUp      1.2초 동안 0.8m 위로
            FadeOut     0.6초 기다렸다가 0.6초 동안 사라짐
               ↓
            효과가 다 끝나면 자동으로 풀에 반납 (재사용)
```

---

## 2. 역할 나누기 (SOLID)

| 스크립트 | 맡은 일 | 점수 효과에서 하는 일 |
|---|---|---|
| **Pin** | 핀 하나의 상태 | "쓰러졌어" + 내 머리 위치를 알려주기만 |
| **GameManager** | 점수 담당 | 점수 값(100) 결정, 점수판 갱신, 숫자 띄우기 지시, 숫자 색 |
| **ScorePopup** 🆕 | 숫자 띄우기 | 어떤 효과를 조합할지 결정 (**에셋을 아는 유일한 곳**) |
| **CountUpBehavior** 🆕 | 숫자 올리기 효과 | +10 ~ +100 으로 숫자 바꾸기 |
| **SoundManager** | 소리 | 핀 소리 |
| FloatingTextEngine (에셋) | 숫자 오브젝트 관리 | 만들기, 재사용 |

- **단일 책임**: 점수에 관한 건 GameManager, 쓰러짐은 Pin, 화면 효과는 ScorePopup
- **개방-폐쇄**: 에셋 코드는 한 줄도 안 고치고, 에셋과 같은 틀(`IFloatingBehavior`)로 새 효과(CountUp)를 추가
- **의존 역전**: Pin 과 GameManager 는 에셋을 직접 모르고 `ScorePopup.Show()` 만 앎. 에셋을 바꿔도 ScorePopup 만 고치면 됨

---

## 3. 새로 만든 스크립트

### 3-1. 🆕 `Assets/cs/CountUpBehavior.cs`

- **오브젝트에 붙이지 않아요.** ScorePopup 이 코드에서 `new CountUpBehavior(...)` 로 사용
- 에셋에 "숫자 올리기" 효과가 없어서 직접 만들었어요. 에셋 효과들과 같은 부모(`IFloatingBehavior`)를 상속해서 `.With()` 에 그대로 끼울 수 있어요

```csharp
using TMPro;
using UnityEngine;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;

// 떠오르는 숫자를 step 단위로 target 까지 올려 보여주는 효과
// 에셋의 다른 효과(MoveUp, FadeOut ...)와 같은 틀(IFloatingBehavior)을 상속해서 .With() 에 그대로 쓸 수 있다
public class CountUpBehavior : IFloatingBehavior {
    private readonly int target;      // 마지막 숫자 (예: 100)
    private readonly int step;        // 한 번에 오르는 양 (예: 10)
    private readonly float duration;  // 다 오르는 데 걸리는 시간 (초)

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
```

### 3-2. 🆕 `Assets/cs/ScorePopup.cs`

- **`static class`** 라서 오브젝트에 붙이지 않아요. `ScorePopup.Show(...)` 로 바로 불러요
- ⚠️ `static class` 뒤에 `: MonoBehaviour` 를 붙이면 에러 (CS0713)

```csharp
using UnityEngine;
using BlackMassSoftware.FloatingTextEngine.Lite;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;

public static class ScorePopup {
    private static readonly Color scoreColor = new Color(1f, 0.85f, 0.2f);   //  금색

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
```

| 효과 | 뜻 |
|---|---|
| `BillboardTextObject(EBillboardType.Full, cam.transform)` | 숫자가 항상 카메라(VR 머리)를 바라봄. **VR 필수** (없으면 뒤집혀 보임) |
| `new CountUpBehavior(score, 10, 0.5f)` | 0.5초 동안 10씩 올라 score 까지 |
| `MoveUp(0.8f, 1.2f)` | 1.2초 동안 0.8m 위로 |
| `FadeOut(0.6f).DelayFor(0.6f)` | 0.6초 기다렸다가 0.6초 동안 투명해짐 |

> 💡 `.With()` 로 이은 효과들은 **동시에** 실행되고, 전부 끝나면 숫자가 풀(재사용 창고)로 돌아가요.

---

## 4. 수정한 스크립트

### 4-1. ✏️ `Assets/cs/GameManager.cs` (추가만)

**변수 추가** (인스펙터에 칸이 생김)
```csharp
    [Header("핀 점수")]
    [Tooltip("핀 하나가 쓰러질 때 오르는 점수")]
    [SerializeField] private int pinScore = 100;
    [Tooltip("떠오르는 점수 숫자의 색")]
    [SerializeField] private Color popupColor = new Color(1f, 0.85f, 0.2f);
```

**함수 추가**
```csharp
    // 핀이 "쓰러졌어" 하고 알려오면: 점수 올리고, 그 위치에 숫자 띄우기
    public void OnPinFallen(Vector3 position){
        AddScore(pinScore);
        ScorePopup.Show(position, pinScore, popupColor);
    }
```

기존 `AddScore`, `UpdateScoreText`, `Reset` 은 그대로예요.

### 4-2. ✏️ `Assets/cs/Pin.cs` (`OnTriggerEnter` 교체 + 함수 추가)

**전**
```csharp
    private void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Floor")){
            isFallen = true;
            if (GameManager.Instance != null){
                GameManager.Instance.AddScore(100);                  // Pin 이 점수를 직접 정함
                SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
            }
        }
    }
```

**후**
```csharp
    private void OnTriggerEnter(Collider other){
        if (isFallen) return;                                    // 이미 쓰러진 핀은 무시
        if (other.gameObject.CompareTag("Floor")){
            isFallen = true;
            if (GameManager.Instance != null){
                SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
                GameManager.Instance.OnPinFallen(TopPosition()); // 점수, 숫자는 GameManager 가
            }
        }
    }

    // 이 핀의 머리 위치 (핀 모양 기준)
    private Vector3 TopPosition(){
        Renderer r = GetComponent<Renderer>();
        if (r == null) return transform.position + Vector3.up * 0.3f;
        Bounds b = r.bounds;
        return new Vector3(b.center.x, b.max.y + 0.15f, b.center.z);
    }
```


## 5. 씬 적용 (Hierarchy · Inspector)

### 5-1. FloatingTextEngine 오브젝트 ✅

1. Hierarchy 우클릭 › **Create Empty** › 이름 `FloatingTextEngine`
2. Add Component › **Floating Text Engine**
3. **프리팹 드래그 앤 드롭**

```
[Project 창]                                        [Inspector 창]
Assets › BlackMassSoftware › FloatingTextEngineLite
  › Runtime › Prefabs › 🟦 NumEffectFlying   ── 끌어서 ──▶   Floating Text Object Prefab
```

   - Hierarchy 에서 `FloatingTextEngine` 을 클릭해 둔 상태로 Project 창의 `NumEffectFlying` 를 끌어다 칸에 놓기
   - Inspector 가 바뀌어 버리면 오른쪽 위 🔒 자물쇠로 고정, 또는 칸 옆 ⊙ 버튼으로 선택

| 칸 | 값 |
|---|---|
| Floating Text Object Prefab | NumEffectFlying |
| Floating Text Font Size | 6 (크면 4) |
| X Spatial Variance | ✅ |
| X Spatial Variance Value | 0.3 |




### 5-2. GameManager 인스펙터 ✅

| 칸 | 현재 값 |
|---|---|
| Score Text | Score Canvas › Score Text |
| **Pin Score** | **100** |
| **Popup Color** | 🎨 분홍빛 빨강 (R 255, G 51, B 104) |

> 💡 색을 바꿀 때는 **알파(A)를 255** 로 두세요. 낮추면 숫자가 투명해져요.



## 6. 조절하고 싶을 때

| 바꾸고 싶은 것 | 어디서 | 고칠 곳 |
|---|---|---|
| 핀 점수 | GameManager 인스펙터 | Pin Score |
| 숫자 색 | GameManager 인스펙터 | Popup Color |
| 글자 크기 | FloatingTextEngine 인스펙터 | Floating Text Font Size |
| 몇씩 올라갈지 | ScorePopup.cs | `new CountUpBehavior(score, 10, 0.5f)` 의 **10** |
| 올라가는 속도 | ScorePopup.cs | 같은 줄의 **0.5f** |
| 떠오르는 높이/시간 | ScorePopup.cs | `MoveUp(0.8f, 1.2f)` |
| 사라지는 때 | ScorePopup.cs | `FadeOut(0.6f).DelayFor(0.6f)` |
| 숫자가 뜨는 높이 | Pin.cs | `TopPosition()` 의 `+ 0.15f` |

### Lite 버전에서 쓸 수 있는 효과

✅ MoveUp · FadeOut · Pop · Wobble · ShakeX3D · Blink · ChangeFontSize · BillboardTextObject
❌ MoveUpArc · ScaleTo · ColorGradient · Shake2D/3D · FadeInOut · 프리셋(Critical 등) → **Full 버전 전용** (부르면 콘솔에 빨간 에러)

---

## 7. 다른 게임에 가져가기

| 가져갈 것 | 같이 챙길 것 | 새 게임에서 할 일 |
|---|---|---|
| Spawner | ISpawner.cs | 자식 클래스 하나 만들기 |
| ScorePopup | CountUpBehavior.cs + Lite 에셋 | FloatingTextEngine 오브젝트 만들고 프리팹 연결 |