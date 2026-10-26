# Beat Sage 로 천장 조명 깜빡이기 (최신)

> 예전 문서 [10_BeatSage_천장효과.md](10_BeatSage_천장효과.md) 의 최신판. 지금 씬 설정과 새 노래 추가 방법까지 정리했습니다.
> 음악 선택/재생은 [14_음악선택_재생.md](14_음악선택_재생.md)

---

## 1. 원리
- **Beat Sage**(beatsage.com) 는 노래를 올리면 AI 가 듣고 **리듬에 맞는 순간(노트)** 을 찍어 Beat Saber 맵 파일로 줍니다.
- 게임이 소리를 실시간으로 추측하지 않고 **그 노트 시간표대로** 천장을 깜빡임 → 빠른 구간은 빠르게, 조용한 구간은 느리게.
- 노트 시간은 "박" 단위 → **초 = 박 × 60 ÷ BPM** (Beat Sage 의 Info 파일 BPM 은 항상 **120**)

```
SoundManager Bgm Audio Source 재생 중 (music.time = 지금 몇 초)
   ↓ 매 프레임 (BeatSageLights.Update)
지금 노래에 맞는 맵 찾기 (FindMapIndex) → 처음 한 번만 파일 읽어 "깜빡일 순간 목록" 만들기 (LoadHits, 캐시)
   ↓
music.time 이 다음 순간을 지나면 Flash()
   · 같은 순간에 노트 2개 이상(양손) → 강한 박 → 천장 전부 밝게
   · 노트 1개 → 50% 확률로 일부만 밝게 (On Chance)
   · 다음 노트까지 남은 시간의 30% 동안 밝게 유지(Hold Portion) → 나머지 동안 어두워짐(Off Level 0.1)
   · 앞뒤 2초 넘게 노트가 없는 조용한 구간 → 원래 밝기로
   ↓
Smooth() → 조명은 Light.intensity, 발광 재질은 MaterialPropertyBlock 으로 _EmissionColor/_BaseColor 밝기 조절
```

---

## 2. 파일 준비 (Beat Sage → Unity)

### Beat Sage 사이트에서 고를 것
| 항목 | 선택 |
|---|---|
| Difficulty | **Expert** 는 꼭 (나머지는 상관없음) |
| Game Mode | **Standard** |
| Song Events | 상관없음 (조명에 영향 없음) |

받은 zip 을 풀면: `Info.dat`, `Expert.dat`, `ExpertPlus.dat`, `Hard.dat`, `Normal.dat`, `song.ogg`, `cover.jpg`

### ⚠️ `.dat` → `.json` 복사 (중요)
Unity 는 `.dat` 를 글자 파일(TextAsset)로 못 읽어서 **Song Maps 칸에 끌어다 놓을 수 없습니다.**
→ 같은 폴더에 `Expert.dat` → **`Expert.json`**, `Info.dat` → **`Info.json`** 복사본을 만듭니다. (원본 .dat 는 남겨 둠)
- Windows 탐색기에서 복사 후 확장자만 `.json` 으로 바꾸면 됨
- 7곡 모두 `.json` 복사본이 있음 (jumpingbunny, let-it-work 는 나중에 추가)

폴더 위치: `Assets/Music/Beat Sage_ <노래>.mp3 - N (v2 HENE+,S,DB)/`

---

## 3. 씬 설정 (Hierarchy → **RhythmBeatSaga** → Beat Sage Lights)

| 칸 | 지금 값 | 뜻 |
|---|---|---|
| Music | SoundManager (AudioSource) | 들을 음악. 비우면 SoundManager 의 Bgm Audio Source 자동 |
| **Song Maps** | 6개 (아래 표) | 노래마다 Song + Beat Map + Info |
| **Ceiling Effects** | 천장 효과 부모 1개 | 깜빡일 오브젝트 (자식의 Light / 발광 Renderer 전부) |
| Off Level | 0.1 | 꺼졌을 때 밝기 |
| On Chance | 0.5 | 노트 1개짜리 박에서 켜지는 비율 |
| Hold Portion | 0.3 | 밝게 유지하는 비율 |
| Rise Time | 0.03 | 켜지는 속도 (초) |
| Same Moment Window | 0.03 | 이 시간 안의 노트는 "같은 순간" |
| Restore When Quiet | ✅ | 조용한 구간은 원래 밝기 |

**Song Maps (Element 0~5)**
| # | Song | Beat Map | Info | 쓰이나 |
|---|---|---|---|---|
| 0 | 2 jumpingbunny | Expert.json | Info.json | ✅ (노래 버튼 1) |
| 1 | 2 kiravale | Expert.json | Info.json | ✅ (버튼 2) |
| 2 | 2 kontraa-no-sleep | Expert.json | Info.json | ✅ (버튼 3) |
| 3 | 2 ludosoundx | Expert.json | Info.json | ✅ (버튼 4) |
| 4 | 2 sigmamusicart | Expert.json | Info.json | ✅ (버튼 5) |
| 5 | 1 let-it-work | Expert.json | Info.json | ❌ SoundManager Songs 에 없어서 재생될 일이 없음 |

**맵을 고르는 순서** (`FindMapIndex`)
1. 지금 나오는 노래(`music.clip`) 와 **Song 칸이 같은** 맵 ← 지금은 전부 이 방식
2. Song 칸이 비어 있으면 → 노래 목록 번호(`CurrentSongIndex`) 와 같은 번호의 맵

> 옛날 방식 `rhthem`(소리를 듣고 박자를 추측) 은 **Rhythm** 오브젝트에 있고 **꺼져 있습니다.** 둘을 동시에 켜면 같은 조명을 서로 바꾸려고 해서 깜빡임이 꼬입니다 — **하나만 켜기.**

---

## 4. 새 노래 추가 순서
1. beatsage.com 에 노래 올리기 → Expert / Standard → 다운로드 → zip 풀기
2. 폴더째 `Assets/Music` 에 넣기 (Unity Project 창에 끌어다 놓기)
3. 그 폴더의 `Expert.dat`, `Info.dat` 를 **.json 으로 복사**
4. 노래 파일(mp3)도 `Assets/Music` 에 넣고 **SoundManager → Songs** 에 추가 ([14_음악선택_재생.md](14_음악선택_재생.md))
5. **RhythmBeatSaga → Song Maps** `+` → Song = 그 mp3, Beat Map = `Expert.json`, Info = `Info.json`
6. Play → 노래 선택 → Console 에 `[BeatSageLights] Expert : 깜빡일 순간 ○○○개 (BPM 120)` 가 나오면 성공
7. Ctrl + S

## 5. 조절
| 하고 싶은 것 | 바꿀 칸 |
|---|---|
| 깜빡임이 노래보다 빠름/느림 | 그 노래의 **Offset** — 깜빡임이 **늦으면 +** (앞당김), **빠르면 -** (늦춤), 0.05 단위로 (코드: `now = music.time + offset`) |
| 덜 정신없게 | On Chance ↓, Hold Portion ↑ |
| 더 화려하게 | Off Level ↓ (0 = 완전히 꺼짐), On Chance ↑ |
| 노트가 너무 많음/적음 | Beat Map 을 `Hard.json`(적게) / `ExpertPlus.json`(많게) — 이것도 .json 복사 필요 |

## 6. 안 될 때
| Console / 증상 | 원인 |
|---|---|
| `Song Maps 칸이 비어 있어요` | Song Maps 에 맵을 안 넣음 |
| `깜빡일 천장 효과(Light / Renderer)가 없어요` | Ceiling Effects 비어 있음 / 자식에 Light·Renderer 없음 |
| 음악은 나오는데 안 깜빡임 | Song 칸의 노래 ≠ 지금 노래, 또는 Beat Map 이 비어 있음 |
| Beat Map 칸에 파일이 안 들어감 | `.dat` 를 넣으려 함 → `.json` 복사본 사용 |
| 깜빡임이 이상하게 겹침 | Rhythm(rhthem) 오브젝트도 켜져 있음 → 끄기 |
