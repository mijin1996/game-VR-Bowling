# Beat Sage 로 천장 효과 깜빡이기 (BeatSageLights.cs)

[Beat Sage](https://beatsage.com/) 의 AI 가 노래를 듣고 **리듬에 맞는 순간들을 노트로 찍어 준 파일**을 받아서, 게임은 그 순간마다 천장 효과를 깜빡입니다.
소리를 게임이 실시간으로 추측하는 `rhthem` 과 달리 **AI 가 미리 분석한 결과**를 쓰므로 정확하고, 빠른 구간은 빠르게 / 조용한 구간은 느리게 깜빡입니다.

> `rhthem` 은 그대로 남아 있습니다. **둘 중 하나만** 켜서 쓰세요 (둘 다 켜면 같은 조명을 서로 덮어씀).

---

## 1. Beat Sage 에서 파일 받기
1. beatsage.com 에서 노래 넣기
2. **Step 2 난이도**: Expert 추천 (Expert+ 는 더 촘촘). 여러 개 골라도 됨
3. **Step 3 Game Modes**: Standard 만 (90/360 Degrees 해제), 무대 디자인은 Default
4. **Step 4 Events**: Bombs·Obstacles 해제 (상관없음)
5. **Model Version**: V2
6. zip 다운로드 → 압축 풀어서 `Assets/Music/` 에 폴더째 넣기

받은 폴더 안 파일
| 파일 | 뜻 |
|---|---|
| `Info.dat` | 곡 정보. `_beatsPerMinute` (Beat Sage 는 항상 **120** = 박을 초로 바꾸는 기준값, 곡의 실제 템포 아님) |
| `Normal.dat` / `Hard.dat` / `Expert.dat` / `ExpertPlus.dat` | 난이도별 노트 목록. 노트마다 `_time` = 몇 번째 박 |
| `song.ogg`, `cover.jpg` | Beat Sage 가 쓴 노래·표지 (게임에선 안 씀) |

## 2. `.dat` → `.json` 복사본 만들기
Unity 는 `.dat` 를 글자 파일(TextAsset)로 읽지 못합니다. **원본 `.dat` 는 그대로 두고** 같은 폴더에 `.json` 복사본을 만듭니다.
- 이번에 5곡 모두 이미 만들어 두었습니다: `Info.json`, `Normal.json`, `Hard.json`, `Expert.json`, `ExpertPlus.json`
- 새 노래를 추가할 때: 탐색기에서 `Expert.dat` 를 복사 → 이름을 `Expert.json` 으로 (Info 도 같이)

```
Assets/Music/
 ├ kiravale.mp3                                  ← 게임에서 트는 노래 (SoundManager Songs)
 └ Beat Sage_ kiravale.mp3 - 1 (v2 HENE+,S,DB)/
     ├ Info.dat   Info.json
     ├ Expert.dat Expert.json   ← BeatSageLights 에 넣을 파일
     └ ...
```

## 3. 씬에 적용하기
1. Hierarchy 에서 기존 `Rhythm` 오브젝트의 **rhthem 체크 끄기** (또는 빈 오브젝트 새로 만들기)
2. 그 오브젝트에 **Add Component → BeatSageLights**
3. Inspector 설정

| 칸 | 넣을 것 |
|---|---|
| Music | 비워 두기 (SoundManager BGM 자동) |
| **Song Maps** | 크기 = 노래 수 (5). **SoundManager Songs 와 같은 순서** |
| └ Song | 비워 두면 순서로 짝지음. 넣으면 그 노래 파일과 짝지음 (순서가 달라도 됨) |
| └ **Beat Map** | 그 노래 폴더의 **`Expert.json`** (또는 원하는 난이도 .json) |
| └ Info | 그 노래 폴더의 `Info.json` (비우면 BPM 120 — Beat Sage 는 어차피 120) |
| └ Offset | 깜빡임이 노래보다 **빠르면 +, 늦으면 −** (초, 예: 0.05) |
| **Ceiling Effects** | 천장 효과 오브젝트 (부모만 넣어도 자식까지) |
| Off Level | 0.1 — 꺼졌을 때 밝기 |
| On Chance | 0.5 — 노트 1개짜리 박에서 켜지는 비율 |
| Hold Portion | 0.3 — 다음 노트까지 시간 중 밝게 유지하는 비율 |
| Rise Time | 0.03 — 켜지는 시간 (0 = 번쩍) |
| Same Moment Window | 0.03 — 이 안에 몰린 노트는 "같은 순간" |
| Restore When Quiet | ✅ — 앞뒤 2초 이상 노트가 없으면 원래 밝기로 |

4. Ctrl + S → Play → 노래 목록에서 노래 고르고 토글로 음악 켜기
5. Console 확인
   - `[BeatSageLights] 깜빡일 대상 ○○개 준비 완료`
   - 노래를 틀면 `[BeatSageLights] Expert : 깜빡일 순간 ○○○개 (BPM 120)`

## 4. 어떻게 만들었나 (원리)
```
① 맵 파일 읽기 (노래가 처음 나올 때 한 번, 그 뒤엔 기억)
   Expert.json 의 _notes 에서 _time(박) 만 꺼냄  →  박 × 60 ÷ BPM = 초   (120 BPM → 박 × 0.5)
   0.03초 안에 몰린 노트는 한 순간으로 묶고 개수를 셈 (2개 이상 = 양손 동시 = 강한 박)

② 매 프레임
   지금 음악 시간(music.time + Offset) 을 지난 순간들을 차례로 꺼내서 깜빡임
   · 강한 박 → 전부 밝게
   · 노트 1개 → On Chance 확률로 밝게, 나머지는 어둡게
   · 밝게 유지 = 다음 노트까지 간격 × Hold Portion
   · 그 뒤 다음 노트 전까지 부드럽게 어두워짐
     → 노트가 촘촘한 빠른 구간은 짧게 톡톡, 노트 간격이 넓은 구간은 길게 천천히

③ 노래가 바뀌면 그 노래의 맵으로, 처음으로 돌아가면(반복) 위치를 다시 찾음
④ 음악이 멈추면 원래 밝기, 컴포넌트를 끄면 즉시 원래 밝기
```
- 음악 재생 시간(`music.time`) 기준이라 멈췄다 켜도 어긋나지 않습니다.
- 재질 파일은 바꾸지 않고 `MaterialPropertyBlock` 으로 그 오브젝트에만 밝기를 덮어씁니다.
- 조명 찾기·밝기 바꾸기 부분은 rhthem 과 같은 방식입니다.

## 5. 안 될 때
| 증상 | 해결 |
|---|---|
| 아무 반응 없음, Console 메시지도 없음 | BeatSageLights 가 씬에 없거나 체크가 꺼져 있음 |
| `Song Maps 칸이 비어 있어요` | 3번 표대로 Song Maps 채우기 |
| 음악은 나오는데 "깜빡일 순간" 메시지가 없음 | 지금 노래와 짝지어진 맵이 없음 → Song Maps 순서가 SoundManager Songs 와 같은지, 또는 Song 칸에 노래를 넣었는지 |
| 박이 조금씩 늦거나 빠름 | 그 노래의 **Offset** 조절 (±0.05 씩) |
| 너무 정신없음 | Expert 대신 Hard.json, On Chance 올리기, Hold Portion 올리기, Off Level 올리기 |
| rhthem 과 같이 켜서 깜빡임이 이상함 | 둘 중 하나만 켜기 |
