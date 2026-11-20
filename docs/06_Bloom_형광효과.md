# Bloom 형광 효과 (URP)

아래 5단계 중 **하나라도 빠지면 안 빛나요.**

| 순서 | 어디서 | 할 일 |
|---|---|---|
| 1 | 재질 | **Emission** ✅, 색 고르고 Intensity **1~1.5** (3 이상이면 하얗게 날아감) |
| 2 | Hierarchy | 우클릭 → Volume → **Global Volume** → Profile New → Add Override → **Bloom** (Threshold 1, Intensity 2 — 왼쪽 체크박스 켜기) |
| 3 | Main Camera (XR Origin 안) | Rendering → **Post Processing** ✅ |
| 4 | Project Settings → Quality → 쓰는 URP Asset | **HDR** ✅ |
| 5 | URP Asset의 Renderer 파일 | **Post-processing** ✅ ⭐ 자주 빠뜨림 |

- 오브젝트에 Bloom(Script)·Camera 컴포넌트는 붙이지 않기
- 다 빛나면 Threshold를 1~1.2로 올리기
- Quest에서 무거우면 High Quality Filtering 끄기, Downscale Quarter, 그래도 무거우면 Emission만
