using UnityEngine;
using UnityEngine.Events;

public class PauseMenuUI : MonoBehaviour {
    [Tooltip("멈춤을 누르면 나타나는 판넬 (계속, 랭킹, 재시작, 나가기 버튼이 들어 있음)")]
    [SerializeField] private GameObject panel;

    [Header("버튼이 눌렸을 때 할 일 (게임마다 다르게 연결)")]
    public UnityEvent onPause;
    public UnityEvent onResume;
    public UnityEvent onRanking;
    public UnityEvent onRestart;
    public UnityEvent onQuit;

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Start() {
        SetPanel(false);           // 처음엔 판넬 숨김
    }

    // ---------- 버튼 On Click 에 연결 (프리팹 안에서 이미 연결돼 있음) ----------

    public void Pause() {
        if (panel != null && panel.activeSelf) {   // 이미 열려 있으면 → 닫기 (계속하기와 같음)
            Resume();
            return;
        }
        SetPanel(true);
        onPause.Invoke();
    }

    public void Resume() {
        SetPanel(false);
        onResume.Invoke();
    }

    public void ShowRanking() {
        onRanking.Invoke();
    }

    public void Restart() {
        onRestart.Invoke();
    }

    public void Quit() {
        onQuit.Invoke();
    }

    private void SetPanel(bool active) {
        if (panel != null) panel.SetActive(active);
    }
}
