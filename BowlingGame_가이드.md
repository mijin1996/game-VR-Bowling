# 🎳 BowllyBoll 볼링 게임 정리 노트

> 마지막 정리: 2026-10-11
> 스크립트 위치: `Assets/cs/` · 프리팹 위치: `Assets/Resources/mete/` · 씬: `Assets/Scenes/BowlingGame.unity`

---

## 1. 게임 흐름 한눈에 보기

```
게임 시작
  ↓  BallSpawner : 소켓 4칸에 랜덤 공 놓기
  ↓  PinSpawner  : 정해 둔 자리마다 핀 놓기
  ↓
공을 잡고 던짐
  ↓  BallSpawner : 0.5초 뒤 빈 소켓을 새 공으로 채움
  ↓
핀이 쓰러져 Floor 바닥에 닿음
  ↓  Pin         : 점수 +100, 효과음
  ↓  Pin         : 더 쓰러지는 핀 없이 3초 지나면 전부 제자리로
  ↓
던진 공이 멀리 가거나 떨어짐 / 30초 지남
  ↓  Ball        : "회수해 주세요" 신호
  ↓  BallSpawner : 공을 꺼서 창고(풀)에 보관, 다음에 재사용

리셋 버튼
  ↓  GameResetter : 스포너 전부 다시 깔기 + 점수 0
```

---

## 2. 파일 구조

```
[약속 = 인터페이스]
  ISpawner  "물건을 다시 깔 수 있음"      Spawn()
  IReset    "상태를 처음 값으로"          Reset()

[부모 클래스]
  Spawner<T>  (ISpawner 약속을 대표로 맺음)
    ├─ BallSpawner : Spawner<Ball>
    └─ PinSpawner  : Spawner<Pin>

[물건 자체]
  Ball  (공 하나)
  Pin   (핀 하나)

[관리자]
  GameManager  (점수, IReset)
  SoundManager (소리)
  GameResetter (전체 리셋 버튼)
```

| 파일 | 종류 | 하는 일 |
|---|---|---|
| `ISpawner.cs` | 인터페이스 | "다시 깔 수 있다" 약속 · `Spawn()` |
| `IReset.cs` | 인터페이스 | "처음 값으로 돌아갈 수 있다" 약속 · `Reset()` |
| `Spawner.cs` | 추상 클래스 | 스포너 공통 코드: 프리팹 검사, 랜덤 고르기, 만들기. 시작하면 자동으로 `Spawn()` |
| `BallSpawner.cs` | 스포너 | 소켓 채우기, 던지면 다시 채우기, 회수한 공 창고에 보관 후 재사용 |
| `PinSpawner.cs` | 스포너 | 정해 둔 자리(Spawn Points)마다 핀 만들기 |
| `Ball.cs` | 물건 | 잡기/놓기 감지, 던짐 판단, 회수 조건 검사 |
| `Pin.cs` | 물건 | 쓰러짐 판단(Floor 태그), 점수 +100, 3초 뒤 자동으로 다시 서기 |
| `GameManager.cs` | 관리자 | 점수 계산, 점수 글자 표시, `Reset()`으로 점수 0 |
| `SoundManager.cs` | 관리자 | 배경음, 효과음 |
| `GameRestter.cs` | 관리자 | 리셋 버튼: `ISpawner`는 `Spawn()`, `IReset`은 `Reset()` 호출 |

### 추상 클래스와 인터페이스의 차이

| | 추상 클래스 (`Spawner<T>`) | 인터페이스 (`ISpawner`, `IReset`) |
|---|---|---|
| 비유 | 반쯤 만든 붕어빵 틀 (기능 들어 있음) | 자격증 (할 줄 안다는 약속만) |
| 코드 | 실제 동작 코드가 있음 | 함수 이름만 있음 |
| 개수 | 1개만 상속 가능 | 여러 개 붙일 수 있음 |
| 쓰는 때 | 자식들이 같은 코드를 나눠 쓸 때 | 종류가 달라도 같은 명령을 내릴 때 |

### 언제 무엇을 쓰나

| 구분 | 인터페이스 | 예시 |
|---|---|---|
| **물건**을 다시 깔기 | `ISpawner` | 공 4개, 핀 10개 |
| **숫자/상태**를 처음 값으로 | `IReset` | 점수, (나중에) 프레임 수, 타이머 |

---

## 3. 프리팹 준비 (Project 창)

### 3-1. 공 프리팹 5개 · `Resources/mete/BALL ~ BALL (4)`

프리팹을 더블클릭해서 열고 아래가 붙어 있는지 확인해요.

| 항목 | 설정 |
|---|---|
| Transform › Scale | **양수**로! 실제 볼링공 크기 약 **0.22** (음수면 물리가 이상해짐) |
| Rigidbody | 있어야 함 |
| Sphere Collider | 있어야 함 |
| XR Grab Interactable | 있어야 함 |
| Ball (Script) | 있어야 함. 기본값: Leave Socket Distance 0.5 / Destroy Y Limit -10 / Max Distance 30 / Max Seconds After Throw 30 |

> 💡 5개를 Shift로 한꺼번에 선택하면 Scale을 한 번에 바꿀 수 있어요.

### 3-2. 핀 프리팹 · `Resources/mete/Pin.prefab`

| 항목 | 설정 |
|---|---|
| Rigidbody | 있어야 함 (없으면 맞아도 안 쓰러지고 점수도 없음) |
| Collider (Is Trigger 꺼짐) | 몸통 충돌용 |
| Box Collider (**Is Trigger 켜짐**) | 쓰러짐 감지용. 핀이 누우면 바닥에 닿는 위치여야 함 |
| Pin (Script) | 있어야 함. Reset Delay 기본 3초 |

> 💡 프리팹에 스크립트를 한 번만 붙이면, 그 프리팹으로 만든 핀 전부에 자동으로 붙어요.

### 3-3. 바닥 태그

핀이 쓰러져 닿는 **레인 바닥, 핀 뒤쪽 바닥** 오브젝트의 Tag를 **`Floor`**로 설정해요.
여기가 빠지면 핀이 쓰러져도 점수가 안 올라가요.

---

## 4. 씬 설정 (Hierarchy · Inspector)

### 4-0. 먼저 정리할 것

- [ ] 씬에 **원래 놓여 있던 BALL 5개 삭제** (스포너가 만들기 때문. 남겨두면 소켓에서 겹쳐 뭉침)
- [ ] 씬에 **원래 놓여 있던 핀 삭제 또는 비활성화** (남겨두면 원래 핀 + 새 핀이 겹침)
- [ ] 소켓이 5개 있다면 **4개만** 남기기 (남는 소켓이 근처 공을 끌어당김)

### 4-1. BallSpawner

1. Hierarchy 우클릭 › **Create Empty** › 이름 `BallSpawner`
2. `BallSpawner.cs` 끌어다 붙이기
3. 인스펙터 설정

| 칸 | 넣을 것 |
|---|---|
| **Prefabs** (Size 5) | `Resources/mete`의 **BALL, BALL (1) ~ BALL (4)** 프리팹 |
| **Sockets** (Size 4) | 씬의 소켓 오브젝트 4개 (XR Socket Interactor 붙은 것) |
| Respawn Delay | 0.5 (던진 뒤 새 공이 나오기까지 초) |

> 💡 소켓은 공 지름보다 넓게 (약 0.3m 간격) 벌려 두세요. 좁으면 공끼리 밀어내요.
> 💡 소켓을 볼링장 모델 안이 아니라 **Scale 1, Rotation 0인 빈 오브젝트(예: `BallRack`) 아래**에 두면 크기/기울기 문제가 안 생겨요.

### 4-2. PinSpawner

1. Create Empty › 이름 `PinSpawner` (Scale **1, 1, 1** 유지)
2. `PinSpawner.cs` 붙이기
3. PinSpawner 아래에 Create Empty를 10번 › `PinPoint_1` ~ `PinPoint_10`
4. 각 PinPoint를 핀이 설 자리로 옮기기 (씬 화면에 **노란 동그라미**가 보여요)
5. 인스펙터 설정

| 칸 | 넣을 것 |
|---|---|
| **Prefabs** (Size 1 이상) | `Resources/mete/Pin.prefab` (여러 종류를 넣으면 자리마다 랜덤) |
| **Spawn Points** (Size 10) | `PinPoint_1` ~ `PinPoint_10` |

> 💡 Hierarchy에서 PinPoint 10개를 한꺼번에 선택하고 **Spawn Points 글자 위로 통째로 드래그**하면 한 번에 들어가요.
> 💡 핀이 누워서 생기면 PinPoint의 Rotation을 돌려 보세요. 핀은 각 자리의 방향을 따라가요.

### 4-3. GameManager

| 칸 | 넣을 것 |
|---|---|
| **Score Text** | Score Canvas 안의 **Score Text** (TextMeshPro - Text (UI)) |

### 4-4. SoundManager

| 칸 | 넣을 것 |
|---|---|
| Bgm Audio Source / Sfx Audio Source | 오디오 소스 2개 |
| Bgm Clip | 배경음 |
| **Hit Clip** | 핀 맞는 소리 (비어 있어도 에러는 안 남) |

### 4-5. GameResetter (전체 리셋)

1. Create Empty › 이름 `GameResetter`
2. 리셋 스크립트 붙이기 (⚠️ 아래 6번 "파일 이름 오타"를 먼저 고쳐야 붙어요)
3. 따로 넣을 칸 없음. 씬에서 약속(인터페이스) 가진 스크립트를 알아서 찾아요

### 4-6. Score Canvas (VR에서 카메라 앞에 따라다니는 점수판)

| 대상 | 설정 |
|---|---|
| Score Canvas › Lazy Follow | Target 비워두기 (자동으로 Main Camera) · Target Offset Z 0.5 ~ 1.5 |
| Score Canvas › Rect Transform | Width 400, Height 300 정도 |
| 자식 Score Text, Image | **Pos X 0**, Pos Z 0, Pos Y로 위아래만 조절 |

> ⚠️ 자식의 Pos X가 크면 (예: 494) 캔버스는 정면에 있어도 글자가 5m 옆에 떠 있어요.
> 부모 위치는 플레이하면 Lazy Follow가 덮어쓰니까, **자식 위치만** 신경 쓰면 돼요.

---

## 5. 버튼 연결 (On Click)

버튼은 Option Canvas 우클릭 › **UI › Button - TextMeshPro**로 만들어요.
버튼 인스펙터 맨 아래 **On Click ()** › **+** › 오브젝트를 끌어다 놓고 › 함수 선택.

| 버튼 | 끌어다 놓을 오브젝트 | 고를 함수 | 결과 |
|---|---|---|---|
| 전체 재시작 | `GameResetter` | `GameResetter › ResetAll()` | 공 + 핀 새로 깔기, 점수 0 |
| 핀만 다시 | `PinSpawner` | `PinSpawner › Spawn()` | 핀만 새로 깔기 |
| 공만 다시 | `BallSpawner` | `BallSpawner › Spawn()` | 공만 새로 깔기 |

> ⚠️ VR에서 버튼이 안 눌리면 Option Canvas에 **Tracked Device Graphic Raycaster**가 있는지 확인하세요.

코드에서 부를 때:

```csharp
[SerializeField] private PinSpawner pinSpawner;   // 인스펙터에서 연결

private void OnFrameEnd() {
    pinSpawner.Spawn();   // 핀만 다시 깐다. 공은 그대로
}
```

---

## 6. ⚠️ 지금 남아 있는 할 일

- [ ] **파일 이름 오타**: `GameRestter.cs` 를 **`GameResetter`** 로 이름 바꾸기 (Project 창에서)
  - 파일 이름과 클래스 이름이 달라서 지금은 오브젝트에 붙일 수 없어요
- [ ] 스포너의 **Prefabs 칸 다시 채우기** (필드 이름이 `prefabs`로 바뀌어서 비어 있음)
- [ ] **Ctrl + S 씬 저장** (씬이 오늘 새벽 2:55 이후로 저장되지 않았어요)
- [ ] (선택) `IReset`의 `Reset()` 이름 바꾸기
  - 유니티는 컴포넌트를 붙일 때나 인스펙터 ⋮ › Reset을 누를 때 `Reset()`이라는 함수를 **자동으로** 불러요
  - 지금은 점수가 0이 되는 것뿐이라 큰 문제는 없지만, 헷갈리지 않으려면 `ResetState()` 같은 이름을 추천

---

## 7. 기능을 늘릴 때 규칙

| 새로 생긴 것 | 할 일 | GameResetter 수정? |
|---|---|---|
| 새로 **깔아야 하는 물건** (예: 장애물) | `Spawner<장애물>` 상속, `Spawn()` 구현 | ❌ 안 고침 |
| 새로 **되돌려야 하는 상태** (예: 타이머, 프레임) | `IReset` 붙이고 `Reset()` 구현 | ❌ 안 고침 |

새 스크립트를 만들 때 **"이것도 처음으로 돌아가야 하나?"** 한 번만 생각해 보고, 그렇다면 인터페이스를 붙이면 돼요.

### SOLID 원칙과 연결

- **S 단일 책임**: Ball은 공 하나, Pin은 핀 하나, 스포너는 만들기만, GameResetter는 리셋 명령만
- **O 개방-폐쇄**: 새 기능은 상속이나 인터페이스로 추가하고, GameResetter와 Spawner는 수정하지 않음
- **L 리스코프 치환**: 어떤 스포너든 `Spawn()` 약속을 똑같이 지킴
- **I 인터페이스 분리**: 인터페이스마다 함수 1개씩. 필요 없는 기능을 억지로 만들 일 없음
- **D 의존 역전**: GameResetter는 Ball, Pin을 직접 모르고 약속(인터페이스)에만 기댐

---

## 8. 자주 나는 문제

| 증상 | 원인 | 해결 |
|---|---|---|
| 공들이 한쪽에 뭉침 | 씬에 원래 공이 남아 있음 / 공이 소켓 간격보다 큼 / Scale이 음수 | 원래 공 삭제, 프리팹 Scale 양수 0.22, 소켓 간격 넓히기 |
| 점수가 안 올라감 | 핀이 떨어진 곳이 Floor 태그가 아님 / 핀에 Rigidbody 없음 | 바닥 Tag를 Floor로, 핀에 Rigidbody 추가 |
| 점수가 두 배로 올라감 | 핀이 바닥에 두 번 닿으면 점수도 두 번 | Pin의 `OnTriggerEnter` 맨 앞에 `if (isFallen) return;` 추가 |
| 점수판이 옆에 떠 있음 | Score Canvas 자식의 Pos X가 큼 | 자식 Pos X 0으로 |
| `MonoBehaviour could not be found` | 파일 맨 위 `using UnityEngine;` 빠짐 | 첫 줄에 추가 |
| 스크립트를 오브젝트에 못 붙임 | 파일 이름과 클래스 이름이 다름 | 파일 이름을 클래스 이름과 똑같이 |
| `CS0161` 값을 반환하지 않음 | `return` 없이 끝나는 함수 | 함수 끝에 `return` 추가 또는 함수 정리 |
| 리셋 버튼 눌러도 핀이 새로 안 깔림 | PinSpawner가 Spawner를 상속 안 함 | `PinSpawner : Spawner<Pin>` 확인 |
