using System.Collections.Generic;
using UnityEngine;

public class PinSpawner : Spawner<Pin> {
    [Header("핀 자리")]
    [Tooltip("핀을 놓을 자리들 (빈 오브젝트). 넣은 개수만큼 핀이 생긴다")]
    [SerializeField] private Transform[] spawnPoints;

    private readonly List<Pin> spawnedPins = new List<Pin>();

    public int FallenCount {
        get {
            int count = 0;
            foreach (Pin pin in spawnedPins) if (pin != null && pin.isFallen) count++;
            return count;
        }
    }

    // ISpawner 약속: 이미 있는 핀은 지우고, 정해진 자리에 새로 만든다
    public override void Spawn() {
        if (!HasPrefabs()) return;   // 부모 기능

        if (spawnPoints == null || spawnPoints.Length == 0) {
            Debug.LogWarning("[PinSpawner] spawnPoints 가 비어 있습니다.");
            return;
        }

        ClearPins();

        // 핀 자리(피봇)는 위치 표시용이라 단단한 몸이 있으면 안 된다
        // → 피봇에 콜라이더가 켜져 있으면 핀이 그 안에서 생겨 위로 튕겨 나가므로 꺼 둔다
        foreach (Transform point in spawnPoints) {
            if (point == null) continue;
            foreach (Collider c in point.GetComponentsInChildren<Collider>()) c.enabled = false;
        }

        foreach (Transform point in spawnPoints) {
            if (point != null) SpawnOne(point);   // 빈 칸은 건너뛴다
        }
    }

    // 자리 하나에 랜덤 핀 하나를 만든다 (자리 오브젝트를 통째로 받아서 위치와 방향을 쓴다)
    private void SpawnOne(Transform point) {
        Pin prefab = PickRandomPrefab();   // 부모 기능

        // 프리팹이 원래 갖고 있던 기울기를 유지한 채 자리 방향에 맞춘다
        Quaternion rotation = point.rotation * prefab.transform.rotation;

        // 부모 기능. 위치를 정해서 만들면 Pin.Awake() 가 이 자리를 "처음 자리"로 기억한다
        Pin pin = Create(prefab, point.position, rotation, transform);
        PlaceOnFloor(pin, point);     // 핀 바닥 가운데를 피봇 위치에 맞추고, 바닥 면에 딱 닿게
        pin.RememberStartPose();      // 맞춘 자리를 "처음 자리"로 기억 (다시 세울 때도 이 자리)
        spawnedPins.Add(pin);
    }

    // 핀 모델은 기준점(피봇)이 핀 바닥이 아니라서, 그냥 만들면 피봇 자리와 어긋나고 바닥에서 뜬다
    // → 눈에 보이는 핀 모양을 기준으로 "핀 바닥 가운데 = 피봇의 X,Z" 그리고 "핀 바닥 = 그 아래 바닥 면" 이 되게 옮긴다
    private void PlaceOnFloor(Pin pin, Transform point) {
        Physics.SyncTransforms();
        Bounds b;
        if (!GetBounds(pin, out b)) return;

        // ① 좌우: 핀 가운데를 피봇 X,Z 에 맞춘다
        Vector3 move = new Vector3(point.position.x - b.center.x, 0f, point.position.z - b.center.z);

        // ② 높이: 피봇 아래 바닥 면을 찾는다 (다른 핀, Trigger 는 무시). 못 찾으면 피봇 높이를 바닥으로 본다
        float floorY = point.position.y;
        Vector3 from = new Vector3(point.position.x, Mathf.Max(b.center.y, point.position.y + 0.05f), point.position.z);
        RaycastHit[] hits = Physics.RaycastAll(from, Vector3.down, b.size.y + 2f, ~0, QueryTriggerInteraction.Ignore);
        float best = float.NegativeInfinity;
        string floorName = "(못 찾음 → 피봇 높이 사용)";
        foreach (RaycastHit h in hits) {
            if (h.collider.GetComponentInParent<Pin>() != null) continue;   // 핀(자기 자신 포함)은 바닥이 아님
            if (IsSpawnPoint(h.collider.transform)) continue;               // 핀 자리 표시(피봇)도 바닥이 아님
            if (h.point.y > best) { best = h.point.y; floorName = h.collider.name; }
        }
        if (!float.IsNegativeInfinity(best)) floorY = best;

        move.y = floorY - b.min.y;     // 핀 맨 아래 = 바닥 면
        pin.transform.position += move;
        Physics.SyncTransforms();

        Debug.Log("[PinSpawner] " + point.name + " → 바닥: " + floorName + ", 높이 보정: " + (move.y * 100f).ToString("0.0") + "cm");
    }

    private bool IsSpawnPoint(Transform t) {
        foreach (Transform p in spawnPoints) {
            if (p != null && (t == p || t.IsChildOf(p))) return true;
        }
        return false;
    }

    // 핀의 눈에 보이는 모양 크기 (없으면 Trigger 가 아닌 콜라이더)
    private static bool GetBounds(Pin pin, out Bounds b) {
        b = new Bounds();
        bool has = false;
        foreach (Renderer r in pin.GetComponentsInChildren<Renderer>()) {
            if (!has) { b = r.bounds; has = true; } else b.Encapsulate(r.bounds);
        }
        if (has) return true;
        foreach (Collider c in pin.GetComponentsInChildren<Collider>()) {
            if (!c.enabled || c.isTrigger) continue;
            if (!has) { b = c.bounds; has = true; } else b.Encapsulate(c.bounds);
        }
        return has;
    }

    // 만들어 둔 핀을 모두 지운다
    private void ClearPins() {
        foreach (Pin pin in spawnedPins) {
            if (pin != null) Destroy(pin.gameObject);
        }
        spawnedPins.Clear();
    }

    // 씬 화면에서 핀이 놓일 자리를 노란 동그라미로 보여준다 (게임에는 영향 없음)
    private void OnDrawGizmos() {
        if (spawnPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (Transform point in spawnPoints) {
            if (point != null) Gizmos.DrawWireSphere(point.position, 0.06f);
        }
    }
}