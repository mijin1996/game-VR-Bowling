using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : Spawner<Ball> {
    public event Action BallThrown;
    [Tooltip("공이 놓일 소켓 4개")]
    [SerializeField] private Transform[] sockets = new Transform[4];

    [Tooltip("던진 뒤 새 공이 나타나기까지 기다리는 시간 (초)")]
    [SerializeField] private float respawnDelay = 0.5f;

    private readonly Dictionary<Ball, Queue<Ball>> pool = new Dictionary<Ball, Queue<Ball>>();
    private readonly Dictionary<Ball, Ball> prefabOf = new Dictionary<Ball, Ball>();
    private readonly Dictionary<Ball, int> socketOf = new Dictionary<Ball, int>();

    // ISpawner 약속: 지금 나와 있는 공을 전부 창고에 넣고, 소켓을 새로 채운다
    // 게임 시작 때도, 리셋 버튼을 눌렀을 때도 이 함수 하나로 처리한다
    public override void Spawn() {
        StopAllCoroutines();   // "던진 뒤 다시 채우기" 예약이 남아 있으면 취소 (공이 겹치지 않게)

        // 켜져 있는 공을 전부 창고로 (new List: 반복 중에 목록이 바뀌어도 안전하게 복사본으로 돈다)
        foreach (Ball ball in new List<Ball>(prefabOf.Keys)) {
            if (ball != null && ball.gameObject.activeSelf) {
                OnBallOutOfBounds(ball);
            }
        }

        for (int i = 0; i < sockets.Length; i++) {
            SpawnAt(i);
        }
    }

    // 나와 있는 공을 전부 창고에 넣는다 (화면에서 사라짐)
    // 레벨 클리어 창이 떠 있는 동안 공을 숨길 때 LevelManager 가 부른다
    public void ClearBalls() {
        StopAllCoroutines();   // "던진 뒤 다시 채우기" 예약도 취소 (숨긴 동안 공이 다시 나오지 않게)

        foreach (Ball ball in new List<Ball>(prefabOf.Keys)) {
            if (ball != null && ball.gameObject.activeSelf) {
                OnBallOutOfBounds(ball);
            }
        }
    }

    // i 번째 소켓에 랜덤 공을 놓는다
    private void SpawnAt(int socketIndex) {
        if (!HasPrefabs()) return;                  // 부모 기능
        if (sockets[socketIndex] == null) return;   // 빈 소켓 칸은 건너뛴다

        Ball prefab = PickRandomPrefab();           // 부모 기능
        Ball ball = GetBall(prefab);

        socketOf[ball] = socketIndex;
        ball.gameObject.SetActive(true);
        ball.PlaceAt(sockets[socketIndex]);
    }

    // 창고에 같은 종류가 있으면 꺼내 쓰고, 없으면 새로 만든다
    private Ball GetBall(Ball prefab) {
        Queue<Ball> queue;
        if (pool.TryGetValue(prefab, out queue) && queue.Count > 0) {
            return queue.Dequeue();
        }

        // 부모 기능. 공은 멀리 날아가므로 부모(parent) 없이 만든다
        Ball ball = Create(prefab, Vector3.zero, Quaternion.identity, null);
        prefabOf[ball] = prefab;
        // 이벤트는 처음 만들 때 한 번만 연결 (재사용할 때 중복 연결 방지)
        ball.Thrown += OnBallThrown;
        ball.OutOfBounds += OnBallOutOfBounds;
        return ball;
    }

    private void OnBallThrown(Ball ball) {
        if (BallThrown != null) BallThrown();
        StartCoroutine(RespawnAfterDelay(socketOf[ball]));
    }

    // 던진 공은 소켓으로 되돌리지 않고 창고에 넣는다 (되돌리면 5개가 되니까)
    private void OnBallOutOfBounds(Ball ball) {
        ball.gameObject.SetActive(false);

        Ball prefab = prefabOf[ball];
        if (!pool.ContainsKey(prefab)) pool[prefab] = new Queue<Ball>();
        pool[prefab].Enqueue(ball);
    }

    private IEnumerator RespawnAfterDelay(int socketIndex) {
        yield return new WaitForSeconds(respawnDelay);
        SpawnAt(socketIndex);
    }
}


