using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    public static ButtonManager instance {
        get {
            if (bm == null) {
                bm = FindAnyObjectByType<ButtonManager>();
            }
            return bm;
        }
    }
    private static ButtonManager bm;

    public event Action NextLevelClicked;
    public event Action GameExiting;
    public event Action StartGameClicked;
    public event Action RestartLevelClicked;
    public event Action ReturnToLobbyClicked;

    [Header("게임 시작")]
    [Tooltip("[게임 시작] 버튼 (누르면 숨김). 비워 두면 이름이 StartGameButton 인 오브젝트를 찾음")]
    [SerializeField] private GameObject startGameButton;

    [Header("레벨 글자")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelPopText;

    [Header("판넬")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject licensePanel;

    [Header("레벨 클리어 동안 숨길 것")]
    [Tooltip("점수판 캔버스 (비워 두면 GameManager 의 Score Text 가 들어 있는 캔버스를 자동으로 찾음)")]
    [SerializeField] private GameObject scoreCanvas;

    [Header("레벨 팝업 시간 (초)")]
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float showTime = 1.5f;

    [Header("버튼 소리")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip openClip; 

    private void Start() {
        SetActiveClearPanel(false);     // 처음엔 판넬 숨김
        SetActiveRankingPanel(false);
        SetPopAlpha(0f);                // 팝업 글자도 투명하게
        FindScoreCanvas();
    }

    // 점수판 캔버스를 비워 뒀으면 Score Text 의 캔버스를 찾아 둔다
    private void FindScoreCanvas() {
        if (scoreCanvas != null) return;
        if (GameManager.Instance == null || GameManager.Instance.scoreText == null) return;
        Canvas canvas = GameManager.Instance.scoreText.canvas;
        if (canvas != null) scoreCanvas = canvas.rootCanvas.gameObject;
    }

    // 레벨 글자, 가운데 팝업 글자, 점수판을 숨기거나 다시 보이게 한다
    private void SetPlayUIVisible(bool visible) {
        if (!visible) {                                    // 숨길 때는 진행 중인 팝업도 멈춘다 (팝업이 끝나며 글자를 다시 켜지 않게)
            StopCoroutine(nameof(Popup));
            SetPopAlpha(0f);
        }
        if (levelText != null) levelText.gameObject.SetActive(visible);
        if (levelPopText != null) levelPopText.gameObject.SetActive(visible);
        if (scoreCanvas != null) scoreCanvas.SetActive(visible);
    }

    // ---------- 화면 표시 (LevelManager 가 부름) ----------

    // 레벨 글자 갱신 + 톡 효과
    public void UpdateLevelText(int level, int goalScore) {
        if (levelText == null) return;
        levelText.text = "LEVEL " + level + "\nGOAL " + goalScore;

        ITextPopEffect pop = levelText.GetComponent<ITextPopEffect>();
        if (pop != null) pop.PlayPop();
    }

    [Header("팝업 효과")]
    [Tooltip("팝업 글자가 처음에 이만큼 크게 나타났다가 원래 크기로 줄어든다")]
    [SerializeField] private float popScale = 1.6f;
    private Vector3 popBaseScale = Vector3.zero;

    public float PopupDuration => fadeTime * 2f + showTime;

    // 가운데에 글자가 나타났다 사라짐 ("LEVEL 2", "MISS", "GREAT!" ...) + 소리 + 톡 커지는 효과
    public void ShowPopup(string message, AudioClip clip = null) {
        if (levelPopText == null) return;
        levelPopText.text = message;
        if (clip != null && SoundManager.instance != null) SoundManager.instance.PlaySFX(clip);
        StopCoroutine(nameof(Popup));
        StartCoroutine(nameof(Popup));
    }

    private IEnumerator Popup() {
        SetLevelAndScoreTextVisible(false);                          // 팝업이 나오는 동안 레벨 글자, 점수 글자는 잠깐 숨김 (겹쳐 보이지 않게)

        Transform pt = levelPopText.transform;
        if (popBaseScale == Vector3.zero) popBaseScale = pt.localScale;   // 원래 크기 기억 (처음 한 번)

        for (float t = 0f; t < fadeTime; t += Time.deltaTime) {      // 페이드 인 + 크게 나타났다 줄어들기
            float p = t / fadeTime;
            SetPopAlpha(p);
            float ease = 1f - (1f - p) * (1f - p);                   // 처음엔 빠르게, 끝엔 천천히
            pt.localScale = Vector3.LerpUnclamped(popBaseScale * popScale, popBaseScale, ease);
            yield return null;
        }
        pt.localScale = popBaseScale;
        SetPopAlpha(1f);

        yield return new WaitForSeconds(showTime);                   // 보여주기

        for (float t = 0f; t < fadeTime; t += Time.deltaTime) {      // 페이드 아웃
            SetPopAlpha(1f - t / fadeTime);
            yield return null;
        }
        SetPopAlpha(0f);

        SetLevelAndScoreTextVisible(true);                           // 팝업이 사라지면 다시 보이기
    }

    // 레벨 글자(levelText)와 점수 글자(Score Text)만 켜고 끈다 (캔버스는 그대로라 팝업 글자는 영향 없음)
    private void SetLevelAndScoreTextVisible(bool visible) {
        if (levelText != null) levelText.gameObject.SetActive(visible);
        if (GameManager.Instance != null && GameManager.Instance.scoreText != null) {
            GameManager.Instance.scoreText.gameObject.SetActive(visible);
        }
    }

    private void SetPopAlpha(float opacity) {
        if (levelPopText == null) return;
        Color color = levelPopText.color;
        color.a = opacity;
        levelPopText.color = color;
    }

    public void SetActiveClearPanel(bool active) {
        if (clearPanel != null) clearPanel.SetActive(active);
    }

    // 랭킹을 닫는 곳(시작, 다음 레벨, 결과 판넬, 재시작, 로비)에서는 라이선스 판넬도 같이 닫는다
    public void SetActiveRankingPanel(bool active) {
        if (rankingPanel != null) rankingPanel.SetActive(active);
        if (!active && licensePanel != null) licensePanel.SetActive(false);
    }

    // 창이 열릴 때 소리 (재생은 SoundManager 가 한다)
    private void PlayOpen() {
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(openClip);
    }

    [Header("클리어 / 실패 판넬 글자")]
    [Tooltip("판넬 제목 (비워 두면 clearpanel 안의 clearText 를 자동으로 찾음)")]
    [SerializeField] private TMP_Text clearTitleText;
    [Tooltip("제목 아래 'Score : 000' (비워 두면 clearText 를 복사해서 바로 아래에 자동으로 만듦)")]
    [SerializeField] private TMP_Text clearScoreText;

    // 레벨 클리어: "LEVEL CLEAR!" + 점수 (랭킹은 안 보여줌)
    public void ShowLevelClear(int score) {
        lastResultCleared = true;
        ShowResultPanel("LEVEL CLEAR!", score);
    }

    // 실패: "LEVEL FAILED" + 점수
    public void ShowLevelFailed(int score) {
        lastResultCleared = false;
        ShowResultPanel("LEVEL FAILED", score);
    }

    // 10단계 완료: "ALL CLEAR!" + 점수
    public void ShowGameComplete(int score) {
        lastResultCleared = true;
        ShowResultPanel("ALL CLEAR!", score);
    }

    // 버튼([NEXT LEVEL])은 원래 그대로 두고, 제목과 점수 글자만 바꾼다
    private void ShowResultPanel(string title, int score) {
        PlayOpen();
        SetPlayUIVisible(false);        // 레벨 글자, 팝업 글자, 점수판 숨기기
        SetActiveRankingPanel(false);   // 레벨이 끝날 때는 랭킹을 보여주지 않음 (멈춤 메뉴에서만)
        SetActiveClearPanel(true);
        PrepareResultTexts();

        if (clearTitleText != null) clearTitleText.text = title;
        if (clearScoreText != null) clearScoreText.text = "Score : " + score;
    }

    // 판넬 안의 제목 / 점수 / 버튼을 찾아 두고, 점수 글자가 없으면 제목을 복사해서 바로 아래에 만든다
    private void PrepareResultTexts() {
        if (clearPanel == null) return;
        if (clearTitleText == null) {
            Transform t = clearPanel.transform.Find("clearText");
            if (t != null) clearTitleText = t.GetComponent<TMP_Text>();
        }
        if (clearScoreText == null && clearTitleText != null) {
            GameObject copy = Instantiate(clearTitleText.gameObject, clearTitleText.transform.parent);
            copy.name = "clearScoreText";
            clearScoreText = copy.GetComponent<TMP_Text>();
            clearScoreText.fontSize = clearTitleText.fontSize * 0.55f;
            RectTransform tr = clearTitleText.rectTransform;
            RectTransform sr = clearScoreText.rectTransform;
            sr.sizeDelta = new Vector2(tr.sizeDelta.x, tr.sizeDelta.y * 0.5f);
            sr.anchoredPosition = tr.anchoredPosition - new Vector2(0f, tr.sizeDelta.y * 0.6f);
        }
    }

    [Header("잠시 멈춤")]
    [SerializeField] private GameObject pausePanel;

    // [⏸] 누르면: 시간 멈춤 + 판넬 보이기
    public void OnClickPause() {
        PlayOpen(); 
        PlayClick();
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        RefreshStartButton();   // 멈춤 메뉴가 열려 있는 동안 [게임 시작] 버튼 숨기기
    }

    // [▶] 누르면: 시간 다시 흐름 + 판넬 숨기기
    public void OnClickResume() {
        PlayClick();
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        RefreshStartButton();   // 시작 전이고 랭킹도 닫혀 있을 때만 [게임 시작] 버튼 다시 보이기
    }

    private bool gameStartPressed;

    // [게임 시작] 버튼은 "시작 전 + 멈춤 메뉴 닫힘 + 랭킹 닫힘" 일 때만 보임
    private void RefreshStartButton() {
        PauseMenuUI pause = FindAnyObjectByType<PauseMenuUI>();
        bool pauseOpen = (pausePanel != null && pausePanel.activeSelf) || (pause != null && pause.IsOpen);
        bool rankingOpen = rankingPanel != null && rankingPanel.activeSelf;
        bool licenseOpen = licensePanel != null && licensePanel.activeSelf;
        SetStartButtonVisible(!gameStartPressed && !pauseOpen && !rankingOpen && !licenseOpen);
    }

    private void SetStartButtonVisible(bool visible) {
        if (startGameButton == null) startGameButton = GameObject.Find("StartGameButton");
        if (startGameButton != null) startGameButton.SetActive(visible);
    }

    private bool lastResultCleared;
    [Header("결과 판넬 알림")]
    [Tooltip("[NEXT] 를 눌렀는데 목표 점수를 못 채웠을 때 보여줄 문구 (Score 글자 자리에 잠깐 표시)")]
    [SerializeField] private string notClearedText = "Current score not reached";
    [SerializeField] private float notClearedShowTime = 2f;

    public void OnClickNextLevel() {
        if (!lastResultCleared) {
            PlayClick();
            StopCoroutine(nameof(ShowNotCleared));
            StartCoroutine(nameof(ShowNotCleared));
            return;
        }
        PlayClick();
        SetActiveClearPanel(false);
        SetActiveRankingPanel(false);   // 다음 레벨 시작할 때 랭킹도 닫기
        SetPlayUIVisible(true);         // 숨겼던 레벨 글자, 팝업 글자, 점수판 다시 보이기 ("LEVEL 2" 팝업보다 먼저)
        if (NextLevelClicked != null) NextLevelClicked();   // "눌렸어" 알림 → LevelManager 가 저장 + 레벨업
    }

    // [게임 시작] 버튼 On Click 에 연결 → 버튼 숨기고 "시작" 알림 (LevelManager 가 1단계 시작)
    public void OnClickStartGame() {
        PlayClick();
        gameStartPressed = true;
        SetStartButtonVisible(false);
        if (StartGameClicked != null) StartGameClicked();
    }

    // 게임 시작 전에는 레벨 글자 / 팝업 / 점수판을 숨긴다 (LevelManager 가 부름)
    public void SetPlayUIVisibleBeforeStart(bool visible) {
        SetPlayUIVisible(visible);
    }

    // [랭킹 보기 / 닫기]
    public void OnClickToggleRanking() {
        PlayClick();
        if (rankingPanel == null) return;
        bool show = !rankingPanel.activeSelf;
        rankingPanel.SetActive(show);
        if (show && licensePanel != null) licensePanel.SetActive(false);   // 라이선스 판넬과 같은 자리라 하나만 보이게

        // 멈춤 메뉴는 그대로 둔다 (사용자가 [RESUME] / [II] 로 닫을 때까지 열려 있고 시간도 멈춘 채)

        // 랭킹이 보이는 동안은 점수판(CanvasScore) 숨기기, 닫으면 다시 보이기
        // (단, 클리어/실패 판넬이 떠 있을 때와 [게임 시작] 을 누르기 전에는 점수판을 다시 켜지 않음)
        bool resultShowing = clearPanel != null && clearPanel.activeSelf;
        if (scoreCanvas != null && (show || (!resultShowing && gameStartPressed))) scoreCanvas.SetActive(!show);

        RefreshStartButton();   // 랭킹이 열려 있는 동안 [게임 시작] 버튼 숨기기, 닫으면 (시작 전일 때) 다시 보이기
    }

    // [라이선스 보기 / 닫기] → 랭킹과 똑같이: 점수판 숨기기 + [게임 시작] 버튼 숨기기 (멈춤 메뉴는 그대로 둠)
    public void OnClickToggleLicense() {
        PlayClick();
        if (licensePanel == null) return;
        bool show = !licensePanel.activeSelf;
        licensePanel.SetActive(show);
        if (show && rankingPanel != null) rankingPanel.SetActive(false);   // 랭킹 판넬과 같은 자리라 하나만 보이게

        bool resultShowing = clearPanel != null && clearPanel.activeSelf;
        if (scoreCanvas != null && (show || (!resultShowing && gameStartPressed))) scoreCanvas.SetActive(!show);

        RefreshStartButton();
    }

    // [NEXT] 를 눌렀는데 목표 미달 → Score 글자 자리에 알림을 잠깐 보여준 뒤 원래대로
    private IEnumerator ShowNotCleared() {
        PrepareResultTexts();
        TMP_Text target = clearScoreText != null ? clearScoreText : clearTitleText;
        if (target == null) yield break;
        string before = target.text;
        target.text = notClearedText;
        yield return new WaitForSecondsRealtime(notClearedShowTime);
        if (target != null && target.text == notClearedText) target.text = before;
    }

    // [RESTART]
    // - 결과 판넬(클리어/실패)에서 누르면: 지금 레벨을 처음부터 다시 (점수 0, 핀/공 새로)
    // - 멈춤 메뉴에서 누르면: 예전처럼 저장 후 씬을 다시 불러온다
    public void OnClickRestart() {
        if (clearPanel != null && clearPanel.activeSelf) {
            PlayClick();
            Time.timeScale = 1f;
            SetActiveClearPanel(false);
            SetActiveRankingPanel(false);
            SetPlayUIVisible(true);
            if (RestartLevelClicked != null) RestartLevelClicked();   // → LevelManager 가 같은 레벨 다시 시작
            return;
        }
        PlayClick();
        if (GameExiting != null) GameExiting();             // → LevelManager 가 저장
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // [QUIT]
    // - 결과 판넬(클리어/실패)의 [X]: 그 레벨 게임에서만 나와 로비([게임 시작] 대기 화면)로
    //   랭킹 저장 → 판넬 닫기 → 핀/공/점수판 숨기기 → [게임 시작] 버튼 다시 보이기
    //   씬을 다시 불러오지 않으므로 음악과 천장 효과는 그대로 계속
    // - 멈춤 메뉴에서 누르면: 예전처럼 저장 후 게임 종료
    public void OnClickQuit() {
        if (clearPanel != null && clearPanel.activeSelf) {
            PlayClick();
            Time.timeScale = 1f;
            SetActiveClearPanel(false);
            SetActiveRankingPanel(false);
            gameStartPressed = false;
            if (ReturnToLobbyClicked != null) ReturnToLobbyClicked();   // → LevelManager 가 저장 + 로비 상태로
            RefreshStartButton();
            return;
        }
        PlayClick();
        if (GameExiting != null) GameExiting();             // → LevelManager 가 저장
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayClick() {
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(clickClip);
    }
}