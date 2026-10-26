using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour, IReset {
    [SerializeField] private int maxLevel = 10;
    [Tooltip("레벨마다 목표가 이만큼씩 늘어난다 (1단계 500, 2단계 1000 ... 10단계 5000)")]
    [SerializeField] private int scorePerLevel = 500;

    [Header("기회")]
    [Tooltip("레벨을 시작할 때 던질 수 있는 횟수")]
    [SerializeField] private int startChances = 1;
    [Tooltip("ONE MORE 때 더 주는 기회 수")]
    [SerializeField] private int bonusFor5 = 1;
    [Tooltip("ONE MORE 를 받는 최소 핀 개수 (이보다 적으면 MISS)")]
    [SerializeField] private int minPinsForOneMore = 1;
    [Tooltip("한 레벨에서 ONE MORE 를 받을 수 있는 최대 횟수")]
    [SerializeField] private int maxOneMorePerLevel = 3;
    private int oneMoreUsed;
    [Tooltip("마지막 핀이 쓰러진 뒤 이 시간(초) 동안 더 안 쓰러지면 결과 판정")]
    [SerializeField] private float settleTime = 1.5f;
    [Tooltip("던진 뒤 이 시간(초)이 지나도 하나도 안 쓰러지면 MISS")]
    [SerializeField] private float missTime = 8f;

    private int level = 1;
    private int chances;
    private bool isJudging;
    private bool isLevelOver;
    private bool lastWasFail;
    private bool gameStarted;
    private int pendingThrows;
    private long sessionId;

    [Tooltip("다음 레벨에 공을 새로 놓을 스포너 (비워 두면 씬에서 자동으로 찾음)")]
    [SerializeField] private BallSpawner ballSpawner;

    private BallSpawner[] ballSpawners = new BallSpawner[0];

    [Tooltip("핀 10개의 부모 오브젝트 (비워 두면 PinSpawner 가 붙은 오브젝트를 자동으로 찾음)")]
    [SerializeField] private GameObject pinRoot;

    private GameObject[] pinRoots = new GameObject[0];

    [Header("맞힌 핀 개수 글자")]
    [Tooltip("한 번 던질 때마다 쓰러뜨린 핀 개수를 \"5 pin\" 처럼 보여줄 글자. 비워 두면 이름에 '맞힌' 이 들어간 글자를 자동으로 찾음")]
    [SerializeField] private TMP_Text pinCountText;

    [Header("제한 시간")]
    [Tooltip("기회마다 이 시간(초) 안에 공을 던져야 함. 못 던지면 MISS → 게임 종료(실패 판넬)")]
    [SerializeField] private float throwTimeLimit = 30f;

    private class CountdownLabel {
        public GameObject go;
        public TMP_Text tmp;
        public PinSpawner lane;
    }
    private readonly List<CountdownLabel> countdownLabels = new List<CountdownLabel>();
    private readonly List<CountdownLabel> pinCountLabels = new List<CountdownLabel>();
    private PinSpawner[] lanes = new PinSpawner[0];
    private PinSpawner activeLane;
    private float timeLeft;
    private bool timerRunning;

    private int GoalScore => level * scorePerLevel;

    private void Awake() {
        sessionId = DateTime.Now.Ticks;   // 지금 시각을 이번 판 번호로 사용
        chances = startChances;
    }

    // 다른 매니저의 알림 듣기 시작
    private void Start() {
        ButtonManager.instance.NextLevelClicked += OnNextLevel;
        ButtonManager.instance.GameExiting += SaveProgress;
        ButtonManager.instance.StartGameClicked += OnStartGame;
        ButtonManager.instance.RestartLevelClicked += OnRestartLevel;
        ButtonManager.instance.ReturnToLobbyClicked += OnReturnToLobby;
        ButtonManager.instance.UpdateLevelText(level, GoalScore);
        if (GameManager.Instance != null) GameManager.Instance.ScoreChanged += OnScoreChanged;
        ballSpawners = FindObjectsByType<BallSpawner>(FindObjectsSortMode.None);
        if (ballSpawner == null && ballSpawners.Length > 0) ballSpawner = ballSpawners[0];
        foreach (BallSpawner sp in ballSpawners) sp.BallThrown += OnBallThrown;   // 어느 보관대에서 던져도 판정
        PinSpawner[] pinSpawners = FindObjectsByType<PinSpawner>(FindObjectsSortMode.None);
        pinRoots = new GameObject[pinSpawners.Length];
        for (int i = 0; i < pinSpawners.Length; i++) pinRoots[i] = pinSpawners[i].gameObject;
        if (pinRoot == null && pinRoots.Length > 0) pinRoot = pinRoots[0];
        lanes = pinSpawners;
        FindPinCountText();
        FindCountdownLabels();
        StartCoroutine(WaitForStartButton());
    }

    // [게임 시작] 을 누르기 전: 핀/공/점수판을 숨기고 기다린다
    // (한 프레임 뒤에 숨긴다 → 핀과 공이 먼저 만들어진 다음에 숨기기 위해)
    private IEnumerator WaitForStartButton() {
        yield return null;
        if (gameStarted) yield break;
        SetPlayObjectsVisible(false);
        ButtonManager.instance.SetPlayUIVisibleBeforeStart(false);
        StopTimer();                               // 시작 전에는 카운트다운 숨김
    }

    // [게임 시작] 버튼 → 1단계부터 플레이 시작 ("LEVEL 1" 팝업)
    private void OnStartGame() {
        if (gameStarted) return;
        gameStarted = true;
        ButtonManager.instance.SetPlayUIVisibleBeforeStart(true);
        StartLevel();
    }

    // 결과 판넬의 [X] → 랭킹 저장 후 로비([게임 시작] 대기)로. 씬을 다시 불러오지 않음 (음악·천장 효과 계속)
    // 다시 [게임 시작] 을 누르면 새 판으로 1단계부터
    private void OnReturnToLobby() {
        SaveProgress();                            // 랭킹 저장 (이번 판 점수)
        StopAllCoroutines();
        StopTimer();
        isJudging = false;
        isLevelOver = false;
        lastWasFail = false;
        pendingThrows = 0;
        gameStarted = false;
        level = 1;
        chances = startChances;
        oneMoreUsed = 0;
        sessionId = DateTime.Now.Ticks;            // 다음 판은 새 판 번호 (랭킹에 따로 남음)
        if (GameManager.Instance != null) GameManager.Instance.Reset();
        SetPlayObjectsVisible(false);              // 핀/공 숨기기
        ButtonManager.instance.SetPlayUIVisibleBeforeStart(false);   // 레벨 글자, 점수판 숨기기
        ButtonManager.instance.UpdateLevelText(level, GoalScore);
    }

    // 사라질 때 알림 듣기 그만 (에러 방지)
    private void OnDestroy() {
        if (ButtonManager.instance != null) {
            ButtonManager.instance.NextLevelClicked -= OnNextLevel;
            ButtonManager.instance.GameExiting -= SaveProgress;
            ButtonManager.instance.StartGameClicked -= OnStartGame;
            ButtonManager.instance.RestartLevelClicked -= OnRestartLevel;
            ButtonManager.instance.ReturnToLobbyClicked -= OnReturnToLobby;
        }
        if (GameManager.Instance != null) GameManager.Instance.ScoreChanged -= OnScoreChanged;
        foreach (BallSpawner sp in ballSpawners) if (sp != null) sp.BallThrown -= OnBallThrown;
    }

    // ---------- 투구 판정 ----------

    // 공이 던져졌다 → 결과를 기다렸다가 판정 (결과를 기다리는 동안 또 던진 공은 따로 세지 않음)
    private void OnBallThrown() {
        if (!gameStarted || isLevelOver) return;
        if (isJudging) { pendingThrows++; return; }   // 앞 공 판정 중 → 끝나면 이어서 판정 (예전엔 이 공을 무시해서 게임이 멈췄음)
        StartCoroutine(JudgeThrow());
    }

    // 점수가 목표에 닿았는데 판정 중이 아니면 바로 판정 (NICE JOB 을 놓치지 않게)
    private void OnScoreChanged(int score) {
        if (!gameStarted || isLevelOver || isJudging) return;
        if (score >= GoalScore) StartCoroutine(JudgeThrow());
    }

    private IEnumerator JudgeThrow() {
        isJudging = true;
        StopTimer();                               // 공을 던졌으니 카운트다운 멈춤

        // ① 핀이 다 쓰러지고 잠잠해질 때까지 기다린다
        float throwTime = Time.time;
        int lastCount = Pin.FallenCount;
        float lastChange = Time.time;
        ShowPinCount(lastCount);
        while (true) {
            yield return null;
            int count = Pin.FallenCount;
            ShowPinCount(count);                                            // 쓰러지는 대로 숫자가 올라감
            if (count != lastCount) { lastCount = count; lastChange = Time.time; }

            if (count > 0 && Time.time - lastChange >= settleTime) break;   // 쓰러질 만큼 다 쓰러짐
            if (count == 0 && Time.time - throwTime >= missTime) break;     // 오래 기다려도 하나도 안 쓰러짐
        }

        int fallen = Pin.FallenCount;
        ShowPinCount(fallen);                                               // 이번 투구 결과 (다음 공을 던질 때까지 그대로)
        RememberHitLane();                                                  // 어느 레인을 맞혔는지 (다음 카운트다운을 그 레인에 보여줌)
        int score = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
        SoundManager sm = SoundManager.instance;
        ButtonManager bm = ButtonManager.instance;

        // ② 목표 점수 도달 → (몇 개를 쓰러뜨렸든) 바로 NICE JOB → 클리어
        bool goalReached = score >= GoalScore;
        if (goalReached) {
            Debug.Log("[LevelManager] NICE JOB 판정: 점수 " + score + " / 목표 " + GoalScore + ", 지금 쓰러진 핀 " + fallen + "개");
            isLevelOver = true;
            bm.ShowPopup("NICE JOB!", sm != null ? sm.niceJobClip : null);
            yield return new WaitForSeconds(bm.PopupDuration);
            LevelClear(score);
            isJudging = false;
            yield break;
        }

        // ③ 기회 1번 사용 + 결과에 따라 문구 / 기회 추가 (목표 점수 전까지)
        //   0개      → MISS (글자 + 소리 + 효과)
        //   1개 이상 → ONE MORE (글자 + 소리 + 효과) + 기회 1번 더 + 핀 10개 다시 세움 (레벨당 최대 3번)
        chances--;
        bool isMiss = fallen < minPinsForOneMore;
        if (isMiss) {
            SpawnEffect(missEffect);
            bm.ShowPopup("MISS", sm != null ? sm.missClip : null);
            yield return new WaitForSeconds(bm.PopupDuration);
        } else if (oneMoreUsed < maxOneMorePerLevel) {
            oneMoreUsed++;
            chances += bonusFor5;
            yield return ShowOneMore();                    // 기회가 늘었다는 걸 알려줌
        }
        // (ONE MORE 를 3번 다 받은 뒤 맞힌 투구는 기회 추가 없이 그대로 진행)

        Debug.Log("[LevelManager] 쓰러진 핀 " + fallen + "개, 남은 기회 " + chances + "번");

        // ④ 기회가 없으면 실패, 남았으면 핀 10개 다시 세우고 계속
        // MISS 와 FAIL 은 나오는 기준이 다르다
        //   MISS : 이번에 4개 이하 (MISS 만 보여주고, FAIL 문구는 띄우지 않음)
        //   FAIL : 기회를 다 쓰고도 목표 점수에 못 미침
        if (chances <= 0 && goalReached) {
            // 보너스 기회까지 다 썼고 목표 점수는 넘었음 → NICE JOB → 클리어
            isLevelOver = true;
            bm.ShowPopup("NICE JOB!", sm != null ? sm.niceJobClip : null);
            yield return new WaitForSeconds(bm.PopupDuration);
            LevelClear(score);
        } else if (chances <= 0) {
            isLevelOver = true;
            if (!isMiss) {
                bm.ShowPopup("FAIL", sm != null ? sm.failClip : null);
                yield return new WaitForSeconds(bm.PopupDuration);
            }
            LevelFail(score);
        } else {
            Pin.ResetAllPins();
        }
        isJudging = false;

        // 판정하는 동안 또 던진 공이 있으면 이어서 판정, 없으면 다음 기회의 30초 시작
        if (!isLevelOver && pendingThrows > 0) {
            pendingThrows--;
            StartCoroutine(JudgeThrow());
        } else if (!isLevelOver) {
            StartTimer();
        }
    }

    // 클리어: 랭킹 저장 → 핀/공 숨기기 → 클리어 판넬 (점수 표시)
    private void LevelClear(int score) {
        StopTimer();
        lastWasFail = false;
        SaveProgress();
        SetPlayObjectsVisible(false);
        if (level >= maxLevel) {
            ButtonManager.instance.ShowGameComplete(score);   // 10단계 완료
        } else {
            ButtonManager.instance.ShowLevelClear(score);
        }
    }

    // 실패: 랭킹 저장 → 핀/공 숨기기 → 실패 판넬 (점수 표시, [RETRY])
    private void LevelFail(int score) {
        StopTimer();
        lastWasFail = true;
        SaveProgress();
        SetPlayObjectsVisible(false);
        ButtonManager.instance.ShowLevelFailed(score);
    }

    // [NEXT LEVEL] / [RETRY] 버튼이 눌렸을 때
    private void OnNextLevel() {
        if (!lastWasFail) level++;                 // ① 클리어였으면 다음 레벨, 실패였으면 같은 레벨 다시
        lastWasFail = false;
        StartLevel();
    }

    // 레벨 시작 준비: 점수 0, 기회 초기화, 핀/공 다시 놓기
    private void StartLevel() {
        StopAllCoroutines();
        isJudging = false;
        isLevelOver = false;
        pendingThrows = 0;
        chances = startChances;
        oneMoreUsed = 0;                           // 새 레벨: ONE MORE 횟수 초기화
        GameManager.Instance.Reset();              // 점수 0
        SetPlayObjectsVisible(true);               // 핀 다시 보이기 (켜야 핀 명단에 다시 들어감)
        Pin.ResetAllPins();                        // 켜진 뒤에 핀 다시 세우기 (순서 중요)
        foreach (BallSpawner sp in ballSpawners) sp.Spawn();   // 공도 모든 보관대 소켓에 새로
        ButtonManager.instance.UpdateLevelText(level, GoalScore);
        ButtonManager.instance.ShowPopup("LEVEL " + level);
        ShowPinCount(0);
        activeLane = null;                         // 새 레벨: 아직 아무 레인도 안 맞힘 → 4개 레인 모두에 카운트다운
        StartTimer();
    }

    [Header("ONE MORE (기회 추가 알림)")]
    [SerializeField] private string oneMoreMessage = "ONE MORE!";
    [Tooltip("ONE MORE 때 맞힌 레인 위에 띄울 효과 (JMO 등 파티클 프리팹, 비워 두면 글자 효과만)")]
    [SerializeField] private GameObject oneMoreEffect;
    [SerializeField] private float oneMoreEffectHeight = 1f;

    [Tooltip("MISS 때 맞힌 레인 위에 띄울 효과 (비워 두면 글자 효과만)")]
    [SerializeField] private GameObject missEffect;

    // 맞힌 레인(없으면 LevelManager 위치) 위에 효과를 잠깐 띄운다
    private void SpawnEffect(GameObject prefab) {
        if (prefab == null) return;
        Vector3 pos = (activeLane != null ? activeLane.transform.position : transform.position) + Vector3.up * oneMoreEffectHeight;
        GameObject fx = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(fx, 4f);
    }

    private IEnumerator ShowOneMore() {
        SoundManager sm = SoundManager.instance;
        ButtonManager bm = ButtonManager.instance;
        SpawnEffect(oneMoreEffect);
        bm.ShowPopup(oneMoreMessage, sm != null ? sm.oneMoreClip : null);   // 글자 톡 튀어나옴 + 소리
        yield return new WaitForSeconds(bm.PopupDuration);
    }

    // 결과 판넬의 [RESTART] → 같은 레벨을 처음부터 (점수 0, 기회 초기화, 핀/공 새로)
    private void OnRestartLevel() {
        lastWasFail = false;
        StartLevel();
    }

    // ---------- 제한 시간 ----------
    // 매 프레임 남은 시간을 줄이고, 0 이 되면 시간 초과 처리 (멈춤 메뉴로 시간이 멈추면 같이 멈춤)
    private void Update() {
        if (!timerRunning || !gameStarted || isLevelOver || isJudging) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) {
            timeLeft = 0f;
            timerRunning = false;
            StartCoroutine(TimeOut());
        }
        ShowCountdown();
    }

    // 30초 안에 못 던짐 → MISS + "0 pin" → 게임 종료 (실패 판넬)
    private IEnumerator TimeOut() {
        isLevelOver = true;
        SetCountdownVisible(false);
        ShowPinCount(0);
        SoundManager sm = SoundManager.instance;
        ButtonManager bm = ButtonManager.instance;
        bm.ShowPopup("MISS", sm != null ? sm.missClip : null);
        yield return new WaitForSeconds(bm.PopupDuration);
        int score = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
        LevelFail(score);
    }

    private void StartTimer() {
        timeLeft = throwTimeLimit;
        timerRunning = true;
        ShowCountdown();
        // 확인용: 카운트다운 글자가 실제로 화면에 켜졌는지
        string info = "";
        foreach (CountdownLabel l in countdownLabels)
            info += " [" + (l.go != null ? l.go.name + " 보임=" + l.go.activeInHierarchy : "없음") + " 레인=" + (l.lane != null ? l.lane.name : "-") + "]";
        Debug.Log("[LevelManager] 카운트다운 시작 " + throwTimeLimit + "초, 시간배속=" + Time.timeScale + info);
    }

    private void StopTimer() {
        timerRunning = false;
        SetCountdownVisible(false);
    }

    // 맞힌 레인의 countDown 에만 숫자를 보여준다 (아직 아무 레인도 안 맞혔으면 4개 모두)
    private void ShowCountdown() {
        string msg = Mathf.CeilToInt(timeLeft).ToString();
        foreach (CountdownLabel l in countdownLabels) {
            bool show = activeLane == null || l.lane == activeLane;
            if (l.go.activeSelf != show) l.go.SetActive(show);
            if (!show) continue;
            if (l.tmp != null) l.tmp.text = msg;
        }
    }

    private void SetCountdownVisible(bool visible) {
        foreach (CountdownLabel l in countdownLabels) if (l.go.activeSelf != visible) l.go.SetActive(visible);
    }

    // 이번 투구에서 핀이 가장 많이 쓰러진 레인을 기억 (하나도 안 쓰러졌으면 그대로 둠)
    private void RememberHitLane() {
        PinSpawner best = null;
        int bestCount = 0;
        foreach (PinSpawner lane in lanes) {
            if (lane == null) continue;
            int c = lane.FallenCount;
            if (c > bestCount) { bestCount = c; best = lane; }
        }
        if (best != null) activeLane = best;
    }

    // 글자 하나를 레인과 짝짓는다: 핀 세트(PinSpawner) 밑에 있으면 그 세트, 아니면 가로 거리로 가장 가까운 세트
    // TextMeshPro 글자만 사용 (Text(Legacy) 는 Tools → BowllyBoll → 모든 Text(Legacy)를 TextMeshPro 로 바꾸기 로 변환)
    private CountdownLabel MakeLabel(Transform t) {
        if (!t.gameObject.activeSelf) return null;   // Hierarchy 에서 꺼 둔 글자(안 쓰는 것)는 연결하지 않음
        CountdownLabel l = new CountdownLabel {
            go = t.gameObject,
            tmp = t.GetComponent<TMP_Text>(),
        };
        if (l.tmp == null) {
            if (t.GetComponent<UnityEngine.UI.Graphic>() != null)
                Debug.LogWarning("[LevelManager] '" + t.name + "' 은 TextMeshPro 가 아니에요. Tools → BowllyBoll → 모든 Text(Legacy)를 TextMeshPro 로 바꾸기 를 눌러 주세요.");
            return null;
        }
        l.lane = t.GetComponentInParent<PinSpawner>(true);
        if (l.lane == null) {
            float best = float.MaxValue;
            foreach (PinSpawner lane in lanes) {
                if (lane == null) continue;
                Vector3 d = lane.transform.position - t.position;
                float dist = d.x * d.x + d.z * d.z;                          // 높이는 빼고 가로 거리만
                if (dist < best) { best = dist; l.lane = lane; }
            }
        }
        return l;
    }

    // 이름이 countDown 인 글자를 모두 찾아, 가장 가까운 핀 세트(레인)와 짝짓는다
    private void FindCountdownLabels() {
        countdownLabels.Clear();
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            bool isCountdown = t.name.Trim().StartsWith("countDown", StringComparison.OrdinalIgnoreCase) || t.name.Contains("카운트다운");
            if (!isCountdown) continue;
            CountdownLabel l = MakeLabel(t);
            if (l != null) countdownLabels.Add(l);
        }
        if (countdownLabels.Count == 0) Debug.LogWarning("[LevelManager] 이름이 countDown 인 글자를 못 찾았어요.");
        else Debug.Log("[LevelManager] countDown 글자 " + countdownLabels.Count + "개 찾음");
    }

    // ---------- 맞힌 핀 개수 글자 ----------
    // 예: 5개 쓰러뜨리면 "5 pin", 1개면 "1 pin"
    // 레인마다 자기 핀 세트에서 쓰러진 개수를 보여준다 (count < 0 이면 레인별 실제 개수, 0 이면 모두 "0 pin")
    private void ShowPinCount(int count) {
        string msg = count + " pin";
        if (pinCountText != null) pinCountText.text = msg;
        foreach (CountdownLabel l in pinCountLabels) {
            int n = (count > 0 && l.lane != null) ? l.lane.FallenCount : count;
            string m = n + " pin";
            if (l.tmp != null) l.tmp.text = m;
        }
    }

    // 인스펙터에서 비워 뒀으면 이름에 "맞힌" 이 들어간 글자를 찾는다 (꺼져 있는 것도 포함)
    // 레인마다 있는 글자를 모두 찾는다 (핀 세트 밑에 있으면 그 레인, 아니면 가장 가까운 레인과 짝)
    private void FindPinCountText() {
        pinCountLabels.Clear();
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            if (!t.name.Contains("맞힌")) continue;
            CountdownLabel l = MakeLabel(t);
            if (l != null) pinCountLabels.Add(l);
        }
        if (pinCountLabels.Count > 0) {
            Debug.Log("[LevelManager] 맞힌 핀 갯수 글자 " + pinCountLabels.Count + "개 찾음");
            return;
        }
        if (pinCountText != null) return;
        Debug.LogWarning("[LevelManager] 맞힌 핀 개수 글자를 못 찾았어요. LevelManager 의 Pin Count Text 칸에 넣어 주세요.");
    }

    // 핀과 공을 숨기거나 다시 보이게 한다
    // 숨길 때: 핀 부모를 끄면 핀 10개가 한 번에 사라짐 + 나와 있는 공은 창고로
    // 보일 때: 핀 부모만 다시 켠다 (공은 StartLevel 에서 Spawn 으로 새로 놓음)
    private void SetPlayObjectsVisible(bool visible) {
        foreach (GameObject root in pinRoots) if (root != null) root.SetActive(visible);   // 핀 세트 4개 모두
        if (!visible) foreach (BallSpawner sp in ballSpawners) sp.ClearBalls();
    }

    // 이번 판 기록을 랭킹에 저장 (0점이면 저장 안 함)
    public void SaveProgress() {
        if (GameManager.Instance == null) return;
        int score = GameManager.Instance.CurrentScore;
        if (score <= 0) return;
        Ranking.Save(sessionId, score, level);
    }

    // VR: 헤드셋을 벗거나 홈 버튼으로 나갈 때
    private void OnApplicationPause(bool paused) {
        if (paused) SaveProgress();
    }

    // 게임 창을 닫을 때
    private void OnApplicationQuit() {
        SaveProgress();
    }

    // IReset: 리셋 버튼 → 지금 판은 저장하고 1단계부터 새 판
    public void Reset() {
        SaveProgress();
        StopAllCoroutines();
        level = 1;
        chances = startChances;
        isJudging = false;
        isLevelOver = false;
        lastWasFail = false;
        pendingThrows = 0;
        sessionId = DateTime.Now.Ticks;
        if (ButtonManager.instance != null) ButtonManager.instance.UpdateLevelText(level, GoalScore);
    }
}
