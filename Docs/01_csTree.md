# csTree — 스크립트 파일 지도

모든 게임 스크립트는 `Assets/cs/` 에 있고, 에디터 전용 도구는 `Assets/cs/Editor/` 에 있습니다.
(Editor 폴더 안 스크립트는 Unity 에디터에서만 쓰이고, 게임 빌드(Quest)에는 들어가지 않습니다.)

```
Assets/cs/
│
├─ ■ 매니저 (게임 전체에 하나씩)
│   ├─ GameManager.cs      점수 (+100, Score Text, 떠오르는 숫자)
│   ├─ LevelManager.cs     레벨·목표 점수·기회·투구 판정 (MISS/GREAT/GORGEOUS/NICE JOB/FAIL)
│   ├─ ButtonManager.cs    화면 표시 (레벨 글자, 가운데 팝업, 클리어/실패 판넬, 랭킹, 멈춤) + 버튼 클릭
│   └─ SoundManager.cs     BGM, 노래 목록, 효과음, 공 굴러가는 소리
│
├─ ■ 핀
│   ├─ Pin.cs              핀 하나 (고정 → 공이 오면 풀림, 쓰러짐 판단, 다시 세우기)
│   └─ PinSpawner.cs       피봇 10곳에 핀 놓기 + 바닥에 딱 맞추기
│
├─ ■ 공
│   ├─ Ball.cs             공 하나 (잡기/놓기, 던졌는지 판단, 회수, 굴러가는 소리)
│   └─ BallSpawner.cs      소켓에 공 놓기, 던지면 0.5초 뒤 빈 소켓 채우기, 공 창고(재사용)
│
├─ ■ 스포너 공통 뼈대
│   ├─ Spawner.cs          Spawner<T>: 프리팹 랜덤 고르기 + 만들기 (PinSpawner, BallSpawner 의 부모)
│   └─ ISpawner.cs         "새로 깔 수 있다" 약속
│
├─ ■ 랭킹
│   ├─ Ranking.cs          랭킹 저장/불러오기 (PlayerPrefs + JSON, 10위까지)
│   └─ RankingBoard.cs     랭킹 판넬의 RANK / SCORE / DATE 칸에 숫자 채우기
│
├─ ■ 재사용 UI 프리팹의 "뇌" (버튼이 눌렸다고 알리기만 함)
│   ├─ LevelClearUI.cs     클리어 판넬 프리팹(CanvasOp) 버튼 알림
│   ├─ PauseMenuUI.cs      멈춤 메뉴 프리팹 ([II] 열기/닫기, 계속·랭킹·처음부터·나가기)
│   └─ SongListUI.cs       노래 목록 프리팹 (버튼 글자 = 노래 이름, 누르면 그 노래 선택)
│
├─ ■ 효과
│   ├─ EffectPop.cs        글자 "톡" 커졌다 돌아오기 (레벨 글자, 점수 글자)
│   ├─ ITextPopEffect.cs   "톡 효과를 줄 수 있다" 약속
│   ├─ EffectPanelOpen.cs  판넬 켜질 때 커지며 나타나기 + 파티클
│   ├─ EffectButtonPress.cs 버튼 누를 때 눌리는 움직임 + 파티클
│   ├─ EffectNumFlying.cs  핀 쓰러진 자리에 +10→+100 떠오르는 숫자
│   └─ CountUpBehavior.cs  떠오르는 숫자를 10씩 올리는 동작 (EffectNumFlying 이 사용)
│
├─ ■ 음악 연출 (둘 중 하나만 켜서 사용)
│   ├─ rhthem.cs           소리를 실시간으로 듣고 박을 추측해 천장 효과 깜빡이기
│   └─ BeatSageLights.cs   Beat Sage 맵 파일(AI 가 찍은 노트 타이밍)대로 천장 효과 깜빡이기
│
├─ IReset.cs               "처음 상태로 되돌릴 수 있다" 약속 (GameManager, LevelManager)
│
└─ Editor/  (에디터 전용 도구 — Unity 상단 메뉴 Tools → BowllyBoll)
    ├─ OptionUIBuilder.cs  UI 만들기 메뉴 모음 (레벨 UI, 멈춤 메뉴, 게임 시작 버튼, 노래 목록, ButtonManager 자동 연결)
    ├─ FontChanger.cs      모든 글자를 Yorktown / OldFolksShuffle 글꼴로 바꾸기
    └─ PinLayoutTool.cs    핀 자리 겹침 검사, 정삼각형으로 맞추기
```

## 누가 누구를 부르는가 (흐름)

```
[공을 던짐]
 Ball ──Thrown──▶ BallSpawner ──BallThrown──▶ LevelManager (판정 시작)
                       └─ 0.5초 뒤 빈 소켓에 새 공

[핀이 쓰러짐]
 Pin ──OnPinFallen()──▶ GameManager ──ScoreChanged──▶ LevelManager
                            ├─ Score Text 갱신 + EffectPop
                            └─ EffectNumFlying (+100 숫자)

[판정 결과]
 LevelManager ──ShowPopup()──────────▶ ButtonManager (가운데 큰 글자 + 소리)
              ──ShowLevelClear/Failed()▶ ButtonManager (판넬)
              ──Pin.ResetAllPins()───▶ 모든 핀 다시 세우기
              ──Ranking.Save()───────▶ 랭킹 저장

[버튼]
 클리어 판넬 버튼 → LevelClearUI → ButtonManager.OnClickNextLevel → LevelManager (다음 레벨 / 같은 레벨 다시)
 [II] 멈춤 버튼   → PauseMenuUI  → ButtonManager.OnClickPause / Resume / ToggleRanking / Restart / Quit
 [START]          → ButtonManager.OnClickStartGame → LevelManager (1단계 시작)
 노래 버튼        → SongListUI → SoundManager.PlaySong(번호)
 List Item Toggle → SoundManager.ToggleBGM (켜기/끄기)

[음악]
 SoundManager.bgmAudioSource ──(소리를 들음)──▶ rhthem ──▶ 천장 효과 밝기
 SoundManager.bgmAudioSource ──(재생 시간)─▶ BeatSageLights ◀── Beat Sage 맵(.json) ──▶ 천장 효과 밝기
```

## 에디터 메뉴 (Tools → BowllyBoll)
| 메뉴 | 하는 일 | 다시 눌러도? |
|---|---|---|
| CanvasOp 안에 UI 만들기 | 예전 레벨 UI 일괄 생성 | **누르지 마세요** (levelText 를 옛 위치에 다시 만듦) |
| buttonmanager 칸 자동 연결 | ButtonManager 칸들을 이름으로 찾아 연결 | 안전 |
| CanvasMusic 안에 멈춤 메뉴 만들기 (프리팹) | 멈춤 메뉴 프리팹 생성/배치 + 연결 | 이미 있으면 안 만듦 |
| CanvasOp 프리팹으로 만들기 | 레벨 UI 를 프리팹으로 저장 | 이미 프리팹이면 안 함 |
| CanvasMusic 안에 게임 시작 버튼 만들기 | [START] 버튼 생성 + 연결 (위치는 처음 한 번만 가운데) | 이미 있으면 연결만 확인, **위치 안 건드림** |
| CanvasMusic 안에 노래 목록 만들기 (프리팹) | 노래 버튼 5개 목록 프리팹 | 이미 있으면 안 만듦 |
| 모든 글자를 Yorktown 으로 바꾸기 / OldFolksShuffle 로 바꾸기 | 씬 + 내 프리팹의 TextMeshPro 글꼴 일괄 변경 | 안전 (Ctrl+Z 가능) |
| 핀 자리 확인 (겹침 검사) | 핀끼리 겹치는 자리 알려주기 | 아무것도 안 바꿈 |
| 핀 자리 정삼각형으로 맞추기 | 핀 지름에 맞춰 10자리 재배치 | Ctrl+Z 가능 |

---

## 추가/변경 (2026-10-18)
| 파일 | 하는 일 |
|---|---|
| `LevelManager.cs` | ONE MORE(1개 이상, 레벨당 3번), MISS, NICE JOB, 30초 카운트다운, 레인 4개 글자(맞힌 핀·카운트다운) 자동 연결, 결과 판넬 RESTART 처리 → [11](11_게임규칙_현재.md), [12](12_텍스트_소리_효과.md) |
| `ButtonManager.cs` | NEXT 미달 안내 문구, 결과 판넬 RESTART(같은 레벨)/QUIT(로비), 랭킹 열면 멈춤 해제 |
| `PinSpawner.cs` | `FallenCount` — 핀 세트(레인)별 쓰러진 개수 |
| `SoundManager.cs` | `oneMoreClip` 추가 |
| `Ranking.cs` / `RankingBoard.cs` | 기록 저장 시 `Changed` 알림 → 열린 랭킹판 바로 갱신 |
| `PauseMenuUI.cs` | `IsOpen` (랭킹 열 때 멈춤 메뉴 닫기용) |
| `Editor/InspectorFiller.cs` | Tools → 인스펙터 빈칸 채우기 (비어 있는 칸만) |
| `Editor/LaneTextToTMP.cs` | Tools → 모든 Text(Legacy)를 TextMeshPro 로 바꾸기 (변환 완료) |
| `Editor/KoreanFontSetup.cs` | 맑은 고딕을 TMP 대체 글꼴로 자동 연결 (한글 표시) |
| `Editor/LaneTextFixer.cs` | 비어 있음 (삭제 가능) |
