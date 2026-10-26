using UnityEngine;
using UnityEngine.Events;

public class LevelClearUI : MonoBehaviour {
    [Header("버튼이 눌렸을 때 할 일 (게임마다 다르게 연결)")]
    public UnityEvent onNextLevel;
    public UnityEvent onRestart;
    public UnityEvent onQuit;

    // ---------- 버튼 On Click 에 연결 (프리팹 안에서 이미 연결돼 있음) ----------

    public void NextLevel() {
        onNextLevel.Invoke();
    }

    public void Restart() {
        onRestart.Invoke();
    }

    public void Quit() {
        onQuit.Invoke();
    }
}
