# 🎳 VR Bowling (game-VR-Bowling)

> 한 줄 소개: (직접 작성)
> 예) VR 컨트롤러로 공을 잡고 던져 제한 시간 안에 핀을 쓰러뜨리는 레벨제 볼링 게임

---

## 📜 개발 과정 히스토리

### Phase 1 — 기초 뼈대 (교수님 수업 · `VR-Bowling-초기스크립트/`)
수업에서 짠 초기 스크립트 3개(`GameManager`, `SoundManager`, `BowlingPin`)로 **핀을 쓰러뜨리면 점수가 오르는** 기본 루프를 만들었다.

| 사용한 개념 | 적용된 곳 |
|---|---|
| 싱글톤 매니저 | `GameManager.gm`(점수), `SoundManager.sm`(BGM·효과음) |
| Trigger + 태그 충돌 | 핀이 `Floor`에 닿으면 쓰러진 것으로 보고 +100점 |
| AudioSource 제어 | BGM 켜기/끄기 토글, `PlayOneShot` 효과음 |

### Phase 2 — 기능 확장 · 리팩토링 (ME)

**① 객체지향 설계**
- 제네릭 스포너 `Spawner<T>` → `BallSpawner`, `PinSpawner`가 상속해 공·핀 생성/재사용
- 인터페이스: `ISpawner`(채우기·비우기), `IReset`(리셋), `ITextPopEffect`(글자 효과)
- 매니저 역할 분리: `GameManager`(점수) / `LevelManager`(레벨·판정) / `ButtonManager`(화면 표시) / `SoundManager`(음악)

**② VR 조작**
- XR Interaction Toolkit으로 공 잡기·던지기, 던진 공 자동 회수
- 캔버스가 헤드셋을 따라다니게

**③ 게임 규칙 (`LevelManager`)**
- 핀 쓰러짐 판정 개선: 바닥 닿음 + 많이 기울어짐 둘 다 인정
- 레벨 · 기회 · 30초 제한 · 레인 4개, 판정 MISS / ONE MORE / NICE JOB / FAIL
- 결과 판넬 NEXT / RESTART / QUIT, 랭킹 저장·표시(`Ranking`, `RankingBoard`), 일시정지 메뉴

**④ 음악 · 연출**
- 노래 목록 선택(`SongListUI`), Beat Sage 맵(.json)으로 천장 조명 깜빡이기(`BeatSageLights`)
- 핀 위로 떠오르는 점수(Floating Text), 버튼·판넬 효과, 형광(Bloom) 효과

#### 💭 고민한 점 · 배운 점
- (직접 작성)
- (직접 작성)

---

## 🛠 기술 스택
- **Language:** C#
- **Engine:** Unity 6 (6000.3.26f1)
- **그 외:** XR Interaction Toolkit 3.x, URP, Floating Text Engine Lite

---

## 📁 문서
- [`Docs/Proposal/Proposal.md`](Docs/Proposal/Proposal.md) — 기획서 세트(컨셉 · 시스템 · 전투 · 레벨 · 밸런스) · 시뮬레이션
- [`Docs/00_README.md`](Docs/00_README.md) — 문서 모음 목차 (규칙·설정 가이드·코드 구조)
