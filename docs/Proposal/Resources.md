# 리소스 (아트 · 사운드 · 곡) 명세 — VR Bowling (BowllyBoll)

> 문서 세트: [목차](Proposal.md) · 기획: [컨셉](Concept.md) · [시스템](System.md) · [전투·개체](Combat_Entity.md) · [레벨 디자인](LevelDesign.md) · [밸런스](Balance.md) · 사업·운영: [사업 검토](Business.md) · [요구사항·로드맵](Requirements.md) · [Business Model](Monetization.md) · [서버·데이터](Server_Data.md) · **리소스** · [로그·지표](Analytics.md) · [QA·출시](QA_Release.md) · [라이브 운영](LiveOps.md) · [사업화·법무](Business_Legal.md) · [needFix](needFix.md)

> **문서 정보** · 문서 번호 `D09-RES` · 버전 v1.0 · 상태 `Draft` · 담당 아티스트 · 사운드 디자이너 · 최종 수정 2026-11-21 · 관련 문서 [전투·개체](Combat_Entity.md) · [사업화·법무](Business_Legal.md)

**변경 이력**

| 버전 | 날짜 | 변경 내용 | 사유 | 결정자 | 영향 범위 |
|---|---|---|---|---|---|
| v1.0 | 2026-11-21 | 최초 작성 | — | 아티스트 | — |

---

## 1. 이 문서의 역할
게임에 들어가는 **모든 곡 · 모델 · 소리 · 폰트의 목록 · 라이선스**를 관리해요.
> **규칙:** 라이선스가 `확인됨`이 아닌 리소스가 있으면 출시하지 않아요.

## 2. ⚠️ 먼저 고칠 것 — 출시 차단
| 리소스 | 위치 | 문제 (2026-11-21 확인) | 대응 |
|---|---|---|---|
| **let-it-work** | `Music/1 let-it-work(저작권있음 사용x).mp3` · `Music/Beat Sage_ let-it-work.mp3 - 6/song.ogg` | 파일 이름에 "저작권있음 사용x"라고 적혀 있는데, **`씬/BowlingGame.unity`의 곡 목록(`song`)과 배경음(`bgmClip`) · 오디오 소스에서 쓰이고 있어요** | 씬에서 대체곡으로 바꾸고 두 파일 모두 프로젝트에서 빼기 |
| **맑은 고딕** | `Fonts/MalgunGothic.ttf` · `MalgunGothic SDF.asset` | Microsoft 폰트 — 게임에 넣어 배포하면 안 될 수 있어요. 씬과 `Yorktown SDF`의 대체 폰트(fallback)로 쓰이는 중 | 상업 허용 한글 폰트(OFL 등)로 교체 |
| `Jupiter.ttf` · `Yorktown.ttf` | `Fonts/` | 라이선스 파일 없음. `Yorktown`은 씬에서 사용 | 출처 · 상업 이용 확인 |

→ [F-03](needFix.md#8-정할-것)

## 3. 곡 (노래 선택 · 조명 박자)
| 곡 ID (제안) | 파일 (이름 기준) | 출처 (추정) | 조명 박자 데이터 | 라이선스 |
|---|---|---|---|---|
| `SONG_001` | `kiravale.mp3` | 무료 음원 사이트 (Pixabay 작곡가 이름 형식) | Beat Sage | ⚠️ 원문 저장 |
| `SONG_002` | `kontraa-no-sleep.mp3` | 위와 같음 | Beat Sage | ⚠️ |
| `SONG_003` | `1 crusadope.mp3` | 위와 같음 | Beat Sage | ⚠️ |
| `SONG_004` | `ludosoundx.mp3` | 위와 같음 | Beat Sage | ⚠️ |
| `SONG_005` | `sigmamusicart.mp3` | 위와 같음 | Beat Sage | ⚠️ |
| `SONG_007` | `jumpingbunny.mp3` | 위와 같음 | Beat Sage | ⚠️ |
| ~~`SONG_006`~~ | ~~`let-it-work`~~ | 저작권 있음 | — | ❌ 사용 금지 (§2) |
| `BGM_COMBAT_*` | `The combat collection STARTER/` (5곡) | 음원 팩 (STARTER) | — | ⚠️ 팩 라이선스 확인 · 쓰는지 확인 |

> 조명 박자 데이터는 Beat Sage(AI 악보 생성)로 만들었어요. 생성물의 상업 이용 조건도 확인해요.

## 4. 그 밖의 리소스
| 리소스 ID | 위치 | 쓰는 곳 | 출처 | 라이선스 |
|---|---|---|---|---|
| `MDL_BALL` · `MDL_CLUB` | `스케치팹에서 다운로드한 에셋/bowling_balls.glb` · `bowling_club.glb` | 공 · 볼링장 | Sketchfab | ⚠️ 모델마다 CC 라이선스가 다름 — **CC BY면 작가 표기 필수, NC(비상업)면 사용 불가**. 다운로드 페이지 확인 |
| `FX_HIT_*` | `Travis Game Assets/Hit Impact Effects` | 핀 충돌 이펙트 | 에셋 팩 | ⚠️ |
| `FX_CFXR_*` | `JMO Assets/Cartoon FX Remaster` | 스트라이크 이펙트 | Unity Asset Store (JMO) | ⚠️ 다운로드 기록 |
| `UI_FLOATTEXT` | `BlackMassSoftware/FloatingTextEngineLite` | 점수 글자 | 에셋 (고지 파일 있음) | ✅ 폰트 SIL OFL 1.1 |
| `UI_SAMPLES` | `Unity UI Samples` (Jupiter 폰트 사용) | 버튼 · 제목 | Unity 샘플 | ⚠️ 배경 이미지 `LICENSE.txt` 확인 |
| `XR_TEMPLATE_*` | `VRTemplateAssets` · `XRI` · `Samples` | 손 · 메뉴 | Unity 템플릿 | ✅ Unity 패키지 |
| `SFX_BALL_ROLL` · `SFX_PIN_*` · `SFX_STRIKE_*` | `Sounds/` · `Music/공이 굴러가는 소리.wav` | 공 · 핀 | 미확인 | ⚠️ |
| `VO_*` | `Music/0 great-woman.mp3` · `0-0 fatastic.mp3` · `0 nice.mp3` 등 | 칭찬 음성 | 미확인 | ⚠️ 목소리는 성우 권리도 확인 |

## 5. 새로 필요한 리소스
| 리소스 ID | 종류 | 관련 | 상태 |
|---|---|---|---|
| `SONG_REPLACE` | let-it-work 대체곡 | §2 | ⚠️ 대기 |
| `FONT_KR` | 맑은 고딕 대체 한글 폰트 | §2 | ⚠️ 대기 |
| `UI_RESULT_STARS` | 별 1 ~ 3개 결과 화면 · 실패 이유 | [REQ_110](Requirements.md) | 대기 |
| `UI_SAFETY` | 시작 안전 안내 (주변 공간) | [QA](QA_Release.md) §4 | 대기 |
| `UI_CREDITS` | 크레딧 (Sketchfab 작가 · 곡 · 폰트) | [REQ_113](Requirements.md) | 대기 |
| `STORE_*` | VR 스토어 이미지 · 트레일러 | [QA](QA_Release.md) §5 | 대기 |
