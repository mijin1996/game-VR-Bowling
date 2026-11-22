# 요구사항 · 로드맵 — VR Bowling (BowllyBoll)

> 문서 세트: [목차](common_Index.md) · 기획: [컨셉](planning_Concept.md) · [시스템](planning_System.md) · [전투·개체](planning_Combat_Entity.md) · [레벨 디자인](planning_LevelDesign.md) · [밸런스](planning_Balance.md) · 사업·운영: [사업 검토](owner_Business.md) · **요구사항·로드맵** · [Business Model](planning_Monetization.md) · [서버·데이터](dev_Server_Data.md) · [리소스](design_Resources.md) · [로그·지표](planning_Analytics.md) · [QA·출시](qa_QA_Release.md) · [라이브 운영](ops_LiveOps.md) · [사업화·법무](owner_Business_Legal.md) · [needFix](common_needFix.md)

> **문서 정보** · 문서 번호 `D02-REQ` · 버전 v1.0 · 상태 `Draft` · 담당 기획자 · 최종 수정 2026-11-21 · 관련 문서 [needFix](common_needFix.md)

**변경 이력**

| 버전 | 날짜 | 변경 내용 | 사유 | 결정자 | 영향 범위 |
|---|---|---|---|---|---|
| v1.0 | 2026-11-21 | 최초 작성 | — | 기획자 | — |

---

## 1. 목적
**무엇을 · 언제까지 · 어떤 기준으로 완료**할지 정하는 문서예요. 우선순위 `Must` · `Should` · `Could` · `Won't`, 공수는 1인 개발 추정이에요.

## 2. 이미 구현된 것
| ID | 기능 | 근거 문서 | 상태 |
|---|---|---|---|
| `REQ_001` | 공 잡기 · 던지기 (XR 물리) | [시스템](planning_System.md) · [전투 · 개체](planning_Combat_Entity.md) | 완료 (손맛 [S-12](common_needFix.md)) |
| `REQ_002` | 레인 3개 · 핀 판정 · 다시 세우기 | [레벨 디자인](planning_LevelDesign.md) | 완료 (꺼진 4번째 세트 [S-11](common_needFix.md)) |
| `REQ_003` | 레벨 10개 · 목표 점수 · 30초 · ONE MORE | [밸런스](planning_Balance.md) | 완료 ([S-01](common_needFix.md) · [S-02](common_needFix.md)) |
| `REQ_004` | 노래 선택 · 박자 조명 (Bloom) | [시스템](planning_System.md) | 완료 |

## 3. 출시까지 할 일
| ID | 기능 | 우선순위 | 마일스톤 | 공수 | 관련 | 상태 |
|---|---|---|---|---|---|---|
| `REQ_100` | **저작권 곡(let-it-work) 제거 · 대체곡, 맑은 고딕 교체** | Must | 알파 | 1일 | [F-03](common_needFix.md) | ⚠️ 대기 |
| `REQ_101` | 판정 중 연속 투구 정리 | Must | 알파 | 1일 | [S-01](common_needFix.md) | 대기 |
| `REQ_102` | 시간 초과 시 기회 -1 (기회 남으면 계속) | Must | 알파 | 0.5일 | [S-02](common_needFix.md) | 대기 |
| `REQ_103` | 헤드셋 해제 시 일시정지 | Must | 알파 | 0.5일 | [S-03](common_needFix.md) | 대기 |
| `REQ_104` | 꺼진 핀 세트 · 4번째 카운트다운 정리 | Must | 알파 | 0.5일 | [S-11](common_needFix.md) | 대기 |
| `REQ_105` | 던지기 손맛 (스윙 → 힘 · 방향, 공 무게감) | Must | 베타 | 3일 | [S-12](common_needFix.md) | 대기 |
| `REQ_106` | 목표 점수 곡선 확정 (투구당 평균 핀 수 측정 후) | Must | 베타 | 1일 + 테스트 | [S-04](common_needFix.md) · [S-10](common_needFix.md) | 대기 |
| `REQ_107` | 스트라이크 · 스페어 보너스 (ON BEAT 제외) | Must | 베타 | 1일 | [S-05](common_needFix.md) | 대기 |
| `REQ_108` | 기회 규칙 한 표로 정리 · 화면 안내 | Should | 베타 | 0.5일 | [S-13](common_needFix.md) | 대기 |
| `REQ_109` | 레벨 기믹 1 ~ 2종 | Should | 베타 | 3일 | [S-06](common_needFix.md) | 대기 |
| `REQ_110` | 별 1 ~ 3개 · 실패 이유 결과 화면 · 레벨 기록 저장 | Must | 베타 | 1.5일 | needFix §7 | 대기 |
| `REQ_111` | VR 편의 · 안전 (앉아서 · 높이 보정 · 시작 안내) | Must | 베타 | 2일 | [QA](qa_QA_Release.md) §4 | 대기 |
| `REQ_112` | 로그 이벤트 | Should | 베타 | 1일 | [로그](planning_Analytics.md) | 대기 |
| `REQ_113` | 라이선스 · 크레딧 화면 (Sketchfab CC 표기 포함) | Must | 출시 | 0.5일 | [사업화 · 법무](owner_Business_Legal.md) | 대기 |
| `REQ_201` | 레벨 팩 · 공 스킨 · 곡별 랭킹 | Won't | — | — | §6 | 보류 |

## 4. 완료 기준 (Given / When / Then)
| ID | 완료 기준 |
|---|---|
| `REQ_100` | Given 출시 빌드 / When 곡 목록 · 배경음을 확인하면 / Then `let-it-work` 파일과 맑은 고딕이 빌드에 없다 |
| `REQ_101` | Given 첫 공이 굴러가는 중 / When 두 번째 공을 던지면 / Then 판정이 끝날 때까지 새 공이 나오지 않거나, 두 공의 결과가 따로 계산된다 (규칙 확정 후) |
| `REQ_102` | Given 기회 2 · 30초 초과 / When 시간이 끝나면 / Then 기회 1로 다음 투구를 이어 간다 |
| `REQ_110` | Given 레벨 3 클리어 / When 결과 화면 / Then 별 개수가 보이고 앱을 다시 켜도 남아 있다 |

## 5. 마일스톤
| 마일스톤 | 들어가는 것 | 통과 조건 |
|---|---|---|
| 알파 | REQ_100 ~ 104 | 저작권 문제 0, 10레벨 규칙대로 진행 |
| 베타 | REQ_105 ~ 112 | 입문자 테스트에서 레벨 1 클리어율 90% |
| 출시 | REQ_113 + [QA](qa_QA_Release.md) | 스토어 심사 통과 |
