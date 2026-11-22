# QA · 출시 체크리스트 — VR Bowling (BowllyBoll)

> 문서 세트: [목차](common_Index.md) · 기획: [컨셉](planning_Concept.md) · [시스템](planning_System.md) · [전투·개체](planning_Combat_Entity.md) · [레벨 디자인](planning_LevelDesign.md) · [밸런스](planning_Balance.md) · 사업·운영: [사업 검토](owner_Business.md) · [요구사항·로드맵](planning_Requirements.md) · [Business Model](planning_Monetization.md) · [서버·데이터](dev_Server_Data.md) · [리소스](design_Resources.md) · [로그·지표](planning_Analytics.md) · **QA·출시** · [라이브 운영](ops_LiveOps.md) · [사업화·법무](owner_Business_Legal.md) · [needFix](common_needFix.md)

> **문서 정보** · 문서 번호 `D11-QA` · 버전 v1.0 · 상태 `Draft` · 담당 QA · 최종 수정 2026-11-21 · 관련 문서 [시스템](planning_System.md) · [사업화·법무](owner_Business_Legal.md)

**변경 이력**

| 버전 | 날짜 | 변경 내용 | 사유 | 결정자 | 영향 범위 |
|---|---|---|---|---|---|
| v1.0 | 2026-11-21 | 최초 작성 | — | QA | — |

---

## 1. 목적
**출시해도 되는 상태인지** 판정하는 문서예요. VR은 **편의 · 안전 · 성능**이 스토어 심사와 리뷰를 좌우해요.

## 2. 버그 등급
| 등급 | 기준 | 출시 조건 |
|---|---|---|
| 치명 | 진행 불가 · 크래시 · 프레임 급락 · **저작권 문제 리소스 포함** | 0건 |
| 높음 | 판정 · 규칙 오류 | 0건 목표 |
| 보통 · 낮음 | 불편 · 보기 문제 | 목록 관리 |

## 3. 테스트 케이스
| TC ID | 확인 방법 | 기대 결과 | 관련 | 등급 |
|---|---|---|---|---|
| `TC_001` | 출시 빌드 곡 목록 · 배경음 · 폰트 확인 | let-it-work · 맑은 고딕 없음 | [REQ_100](planning_Requirements.md) | 치명 |
| `TC_002` | 공이 굴러가는 중 두 번째 공 던지기 | 정한 규칙대로 판정 (섞이지 않음) | [S-01](common_needFix.md) | 높음 |
| `TC_003` | 기회 2 남기고 30초 넘기기 | 기회 1로 계속 | [S-02](common_needFix.md) | 높음 |
| `TC_004` | 플레이 중 헤드셋 벗기 | 타이머 멈춤 | [S-03](common_needFix.md) | 높음 |
| `TC_005` | 레인 3개 모두 사용 | 카운트다운 · 핀 · 보관대가 3개로 일치 | [S-11](common_needFix.md) | 보통 |
| `TC_006` | 천천히 · 빠르게 · 옆으로 휘둘러 던지기 | 힘 · 방향이 팔 움직임대로 | [S-12](common_needFix.md) | 높음 |
| `TC_007` | 스트라이크 · 스페어 | 보너스 점수 · 연출 | [S-05](common_needFix.md) | 보통 |
| `TC_008` | 레벨 3 클리어 → 앱 재시작 | 별 · 기록 유지 | [REQ_110](planning_Requirements.md) | 높음 |
| `TC_009` | 공을 몸 뒤 · 천장으로 던지기 | 공이 사라지면 보관대에 다시 나옴 | — | 보통 |
| `TC_010` | 30분 연속 플레이 | 프레임 · 발열 · 메모리 이상 없음 | — | 치명 |

## 4. VR 편의 · 안전 기준 (정할 것 → [F-09](common_needFix.md#8-정할-것))
| 항목 | 기준 (제안) | 이유 |
|---|---|---|
| 프레임 | 헤드셋 기본 주사율 유지, 떨어지는 구간 0 | 멀미 |
| 카메라 | 게임이 시점을 강제로 움직이지 않음 | 멀미 |
| 시작 안내 | 팔을 크게 휘두를 공간 확보 · 컨트롤러 끈 착용 | 던지는 게임 = 기기 · 주변 파손 위험 |
| 앉아서 모드 · 키 보정 | 공 보관대 높이를 사용자에 맞춤 | 접근성 |
| 주손 선택 | 왼손 · 오른손 | 접근성 |
| 경계(가디언) | 시스템 경계를 막지 않음 | 안전 |
| 번쩍임 | 음악 조명 번쩍임 세기 조절 · 끄기 옵션 | 광과민성 (박자 조명이 핵심 연출이라 특히 중요) |
| 글자 | 가까이 · 크게 | VR 가독성 |

## 5. 스토어 제출 준비
| 항목 | 상태 |
|---|---|
| 1차 스토어 결정 · 빌드 설정 정리 (지금 Oculus · Windows MR 설정 + OpenXR 패키지) | 대기 ([F-01](common_needFix.md)) |
| 스토어 기술 기준을 TC로 옮기기 | 대기 |
| 연령 등급 · 편의 등급 | 대기 |
| 트레일러 · 스크린샷 · 아이콘 | 대기 |
| 크레딧 · 라이선스 화면 (Sketchfab CC 표기) | 대기 ([REQ_113](planning_Requirements.md)) |

## 6. 출시 판정
- [ ] 치명 버그 0 (TC_001 포함)
- [ ] VR 편의 · 안전 기준 전부 통과
- [ ] [리소스](design_Resources.md) 라이선스 전부 확인
- [ ] [사업화 · 법무](owner_Business_Legal.md) 전 항목 완료
