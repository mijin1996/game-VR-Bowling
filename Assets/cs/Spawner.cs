using UnityEngine;

public abstract class Spawner<T> : MonoBehaviour, ISpawner where T : Component {
    [Header("프리팹")]
    [Tooltip("랜덤으로 뽑을 프리팹들")]
    [SerializeField] protected T[] prefabs;

    // 시작하면 기본으로 전부 깐다 (자식이 Start 를 따로 쓰지 않아도 됨)
    protected virtual void Start() {
        Spawn();
    }

    // ISpawner 약속. "어떻게 깔지"는 자식마다 다르니까 abstract (자식이 꼭 구현)
    public abstract void Spawn();

    // 프리팹이 비어 있는지 검사. 비어 있으면 경고를 띄우고 false
    protected bool HasPrefabs() {
        if (prefabs != null && prefabs.Length > 0) return true;

        Debug.LogWarning($"[{GetType().Name}] prefabs 가 비어 있습니다.");   // 어떤 스포너인지 이름까지 표시
        return false;
    }

    // 배열에서 랜덤으로 프리팹 하나 고르기
    protected T PickRandomPrefab() {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    // 실제로 만들기
    // virtual: 기본 동작은 여기 있지만, 자식이 필요하면 바꿔 쓸 수 있다
    protected virtual T Create(T prefab, Vector3 position, Quaternion rotation, Transform parent) {
        return Instantiate(prefab, position, rotation, parent);
    }
}