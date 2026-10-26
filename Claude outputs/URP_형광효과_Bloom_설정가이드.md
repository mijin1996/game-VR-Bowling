# Unity URP 형광(네온) 효과 만들기 — Bloom 설정 가이드

> 대상: Unity 6 (6000.x) · URP · VR(XR) 볼링 프로젝트
> 목표: 볼링 레인(또는 원하는 오브젝트)이 **형광처럼 빛나고 주변으로 빛이 번지게** 만들기

---

## 원리 (딱 3줄)

1. **재질(Material)**이 밝은 빛을 내야 한다 → `Emission` + `Intensity`
2. **Bloom 효과**가 그 밝은 빛을 주변으로 번지게 한다 → `Global Volume`
3. **카메라와 URP 설정**에서 후처리(Post-processing)가 켜져 있어야 Bloom이 보인다

이 셋 중 **하나라도 빠지면 안 빛난다.**

---

## ⚠️ 하면 안 되는 것

- 레인 오브젝트에 **Bloom (Script)** 컴포넌트를 붙이지 말 것 → 옛날 방식이라 URP에서 작동 안 함
- 레인 오브젝트에 **Camera** 컴포넌트를 붙이지 말 것 → 카메라가 하나 더 생겨 화면이 꼬임
- 이미 붙였다면: 컴포넌트 오른쪽 `⋮` → **Remove Component**로 삭제

---

## STEP 1. 재질에 Emission(발광) 켜기

1. **Project 창** → `Assets` → `스케치팹에서 다운로드한 에셋` 폴더 열기
2. **`공굴러갈 바닥의 재질`** 클릭
   - (또는 Hierarchy에서 `LaneCollider (1)` 선택 → 인스펙터 맨 아래 재질)
3. 인스펙터 `Surface Inputs`에서 **Emission** 체크 ✅
4. **Emission Map 옆 색상 칸(HDR)** 클릭 → 색상 창이 열림
5. 원하는 색 선택 (예: 분홍, 시안)
6. 색상 창 아래쪽 **Intensity** 값을 **1 ~ 1.5** 로 설정
   - 0이면 Bloom이 반응하지 않음
   - 너무 높으면(3 이상) 색이 **하얗게 날아감**
7. 색상 창 닫기

> 💡 팁: `Base Map` 색과 `Emission` 색을 같은 계열로 맞추면 더 형광 같아 보임
> 💡 같은 재질을 쓰는 오브젝트는 전부 같이 빛남

---

## STEP 2. Global Volume 만들고 Bloom 추가

1. **Hierarchy 빈 곳**에서 우클릭 → `Volume` → **`Global Volume`**
2. 새로 생긴 `Global Volume` 선택
3. 인스펙터 `Volume` → `Profile` 옆 **New** 클릭
4. **Add Override** 클릭
5. 검색창에 **`Bloom`** 입력 → **Bloom** 클릭
   - (`post-` 처럼 치면 안 나옴. 그냥 `Bloom`)
6. 생긴 Bloom 항목에서 **왼쪽 체크박스를 켜고** 값 입력:

| 항목 | 체크 | 값 | 설명 |
|---|---|---|---|
| Threshold | ✅ | `1` | 이 밝기보다 밝은 것만 빛남 |
| Intensity | ✅ | `2` | 빛 번짐 세기 |
| Scatter | ✅ (선택) | `0.7` | 빛이 퍼지는 넓이 |
| Tint | (선택) | 흰색 | 빛 색 덧입히기 |

> 체크박스를 안 켜면 값을 바꿔도 적용 안 됨!

---

## STEP 3. 카메라에서 Post Processing 켜기

1. Hierarchy에서 **`Complete XR Origin Set Up` → `Camera Offset` → `Main Camera`** 선택
   - 맨 위에 회색(비활성) `Main Camera`는 무시
2. 인스펙터 **Camera** 왼쪽 ▶ 눌러 펼치기
3. `Render Type`은 **Base 그대로** (건드리지 말 것)
4. **▶ Rendering** 펼치기
5. **Post Processing** 체크 ✅

---

## STEP 4. URP Asset에서 HDR 켜기

1. 메뉴 `Edit` → `Project Settings` → **Quality**
2. 맨 위 표에서 **PC 열(모니터 아이콘)** 과 **Android 열**의 **초록 체크**된 단계 확인
3. 그 단계를 클릭 → 아래 **Render Pipeline Asset** 칸의 파일 클릭 (Project 창에서 노랗게 표시됨)
4. Project 창에서 그 URP Asset 선택 (예: `Performance URP Config`)
5. 인스펙터 `Quality` → **HDR** 체크 ✅

---

## STEP 5. Renderer에서 Post-processing 켜기 ⭐ (자주 빠뜨림)

1. STEP 4의 URP Asset 인스펙터 위쪽 **Renderer List** 확인
2. 목록에 있는 Renderer 파일(예: `Android Preset (Universal Renderer Data)`) **더블클릭**
3. 인스펙터를 아래로 내려 **Post-processing** 체크 ✅

> VR 템플릿은 성능 때문에 여기가 꺼져 있는 경우가 많음.
> 이게 꺼져 있으면 1~4단계를 다 해도 **절대 안 빛남**.

---

## STEP 6. 확인

1. **Play** ▶ 누르고 **Game 뷰**에서 확인
2. Scene 뷰에서도 보고 싶으면: Scene 뷰 상단 툴바 **효과 버튼(겹친 원 아이콘)** 드롭다운 → **Post Processing** 체크

---

## 🔧 안 될 때 체크리스트

| 증상 | 원인 | 해결 |
|---|---|---|
| 전혀 변화 없음 | Renderer Post-processing 꺼짐 | STEP 5 |
| 전혀 변화 없음 | 카메라 Post Processing 꺼짐 | STEP 3 |
| 전혀 변화 없음 | 다른 URP Asset을 설정함 | STEP 4의 Quality에서 실제 쓰는 Asset 확인 |
| 레인이 하얗기만 하고 번짐 없음 | Bloom이 적용 안 됨 + Intensity 너무 높음 | STEP 3·5 확인, 재질 Intensity 1로 낮추기 |
| 다 빛남(천장, 조명까지) | Bloom Threshold 너무 낮음 | Threshold `1 ~ 1.2`로 올리기 |
| 빛이 약함 | 재질 Intensity 또는 Bloom Intensity 낮음 | 재질 1.5, Bloom 2~3 |
| Bloom 값 바꿔도 반응 없음 | 항목 체크박스 꺼짐 | 왼쪽 체크박스 켜기 |

---

## 🥽 VR(Quest) 성능 팁

Bloom은 단독형 VR 기기에서 무거움. 프레임이 떨어지면:

- Bloom → **High Quality Filtering** 끄기
- Bloom → **Downscale** 을 `Quarter`로
- 그래도 무거우면 Bloom 끄고 **Emission만** 사용 (번짐은 없지만 밝게는 빛남)

---

## 📌 한 줄 요약

**재질 Emission(Intensity 1~1.5) → Global Volume + Bloom(Threshold 1, Intensity 2) → 카메라 Post Processing ✅ → URP Asset HDR ✅ → Renderer Post-processing ✅**
