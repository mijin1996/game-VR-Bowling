# 서버 · 데이터 연동 — VR Bowling (BowllyBoll)

> 문서 세트: [목차](common_Index.md) · 기획: [컨셉](planner_Concept.md) · [시스템](planner_System.md) · [전투·개체](planner_Combat_Entity.md) · [레벨 디자인](designer_LevelDesign.md) · [밸런스](planner_Balance.md) · 사업·운영: [사업 검토](owner_Business.md) · [요구사항·로드맵](planner_Requirements.md) · [Business Model](planner_Monetization.md) · **서버·데이터** · [리소스](designer_Resources.md) · [로그·지표](planner_Analytics.md) · [QA·출시](tester_Release.md) · [라이브 운영](operator_LiveOps.md) · [사업화·법무](owner_Legal.md) · [needFix](common_needFix.md)

> **문서 정보**

| 문서 번호 | 버전 | 상태 | 책임자 | 협업 | 최종 수정 | 관련 문서 |
|---|---|---|---|---|---|---|
| `D08-SERVER` | v1.1 | `Draft` | 서버 개발자 | 클라이언트 개발자 | 2026-11-23 | [시스템](planner_System.md) · [Business Model](planner_Monetization.md) |

**변경 이력**

| 버전 | 날짜 | 변경 내용 | 사유 | 결정자 | 영향 범위 |
|---|---|---|---|---|---|

---

## 1. 목적
**어떤 데이터를 어디에 저장하고, 무엇을 서버가 판정할지** 정하는 문서임.

## 2. 지금 저장하는 것 (코드 기준)
**아무것도 저장하지 않음.** 레벨 진행 · 기록은 앱을 끄면 사라짐.

## 3. 저장 위치 (제안)
| 데이터 | 저장 위치 | 이유 |
|---|---|---|
| 레벨별 최고 점수 · 별 개수 · 클리어 여부 | 기기 | 개인 기록(타 사용자 비공개) |
| 마지막으로 고른 곡 · 음량 | 기기 | |
| 플레이 설정 (앉아서 모드 · 키 높이 · 주손) | 기기 | |
| 레벨 팩 구매 | 스토어 구매 기록 | 재설치 시 복원 |
| 랭킹 (보류) | 서버 또는 스토어 리더보드 | 위조 방지 |

**저장 형식 (제안)** — 데이터 원본 JSON 규칙과 맞춤.
```json
{
  "version": 1,
  "seated_mode": false,
  "dominant_hand": "right",
  "levels": [
    { "level": 1, "best_score": 620, "stars": 3, "cleared": true }
  ]
}
```
> `Application.persistentDataPath`에 파일 하나로 저장함. 불러오기 실패 시 기본값으로 시작하고 원본은 `.bak`으로 남김.

## 4. 레벨 데이터
| 항목 | 내용 |
|---|---|
| 지금 | 목표 점수 · 기회 · 시간은 [밸런스](planner_Balance.md) `Balance/level_goal.json` · `rule_params.json`에 정리, 코드 · 씬 값과 같은지 확인 |
| 레벨 팩 | 레벨을 JSON으로 추가하는 구조를 만들면 업데이트 · DLC가 쉬워짐 → [F-05](common_needFix.md#8-정할-것) |
