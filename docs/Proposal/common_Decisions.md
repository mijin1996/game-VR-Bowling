# 결정 로그 — VR Bowling

> 문서 세트: [목차](common_Index.md) · 공통 관리: [용어집](common_Glossary.md) · **결정 로그** · [needFix](common_needFix.md)

> **문서 정보**

| 문서 번호 | 버전 | 상태 | 책임자 | 협업 | 최종 수정 | 관련 문서 |
|---|---|---|---|---|---|---|
| `D00-DEC` | v1.1 | `Review` | 기획자 | 대표 (결정자) | 2026-11-23 | [목차](common_Index.md) |

**변경 이력**

| 버전 | 날짜 | 변경 내용 | 사유 | 결정자 | 영향 범위 |
|---|---|---|---|---|---|

---

## 1. 목적
회의 · 메신저에서 정한 중요한 결정을 한 곳에 모음. 결정이 바뀌면 지우지 않고 새 줄을 더하고, 이전 줄의 상태를 `Deprecated`로 바꿈. 아직 정하지 않은 것은 [needFix](common_needFix.md)의 "정할 것"에 있음.

## 2. 결정
| 번호 | 날짜 | 결정 | 이유 | 버린 대안 | 결정자 | 관련 | 상태 |
|---|---|---|---|---|---|---|---|
| DEC_001 | 2026-11-21 | 기획 문서는 기업용 기획 가이드 기준으로 `docs/Proposal/`에 둔다 | 서비스 출시 · 사업자 등록까지 쓰는 사내 문서, 모든 프로젝트 위치 통일 | 포트폴리오용 문서 | 대표 | [목차](common_Index.md) | `Confirmed` |
| DEC_002 | 2026-11-21 | 고칠 점 · 정할 것은 `needFix.md` 한 곳에 모은다 (Suggestions에서 이름 변경) | 분산 관리 시 결정 주체 · 기한 누락 | 문서마다 따로 적기 | 대표 | [needFix](common_needFix.md) | `Confirmed` |
| DEC_003 | 2026-11-21 | 설정 · 콘텐츠 데이터 원본은 JSON | 코드 · 엔진이 바로 읽고 검증 규칙(JSON Schema)을 붙이기 쉬움 | CSV | 대표 | [목차](common_Index.md) | `Confirmed` |
| DEC_004 | 2026-11-21 | 수익 모델 문서 이름은 "Business Model" (BM 약어 쓰지 않음) | 약어는 신규 참여자가 이해하기 어려움 | BM | 대표 | [Business Model](planner_Monetization.md) | `Confirmed` |
| DEC_005 | 2026-11-21 | 담당은 사람 이름이 아니라 직무(대표 · 기획자 · 디자이너 · 개발자 · QA 테스터 · 운영 매니저)로 적고, 문서마다 책임자 1명 + 협업으로 나눈다 | 실제 회사 문서는 역할 기준 | 개인 기준 표기 (예: "본인 확인") | 대표 | [목차](common_Index.md) | `Confirmed` |
| DEC_006 | 2026-11-21 | 기획서는 사이트에 공개하지 않는다 — GitHub Actions로 `docs/index.html`만 배포 (GitHub Pages) | 수익 · 법무 위험이 담긴 사내 문서 | 저장소 폴더 통째로 공개 | 대표 | [라이브 운영](operator_LiveOps.md) | `Confirmed` |
| DEC_007 | 2026-11-23 | 파일 이름 접두사는 책임자 직무명(`owner_` · `planner_` · `designer_` · `developer_` · `tester_` · `operator_`, 공용은 `common_`)으로 쓰고, 문서명에 접두사 단어를 반복하지 않는다 | 업무명 · 직무명 혼용(`planning_` · `owner_`)과 중복(`qa_QA_`) 해소 | 업무명 접두사 (`planning_` · `qa_`) | 대표 | [목차](common_Index.md) | `Confirmed` |
