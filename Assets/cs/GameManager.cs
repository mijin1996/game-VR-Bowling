using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour, IReset{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;

    public event System.Action<int> ScoreChanged;
    public int CurrentScore => currentScore;

    [Header("핀 점수")]
    [Tooltip("핀 하나가 쓰러질 때 오르는 점수")]
    [SerializeField] private int pinScore = 100;
    [Tooltip("떠오르는 점수 숫자의 색")]
    [SerializeField] private Color popupColor = new Color(1f, 0.85f, 0.2f);

    private ITextPopEffect scorePop;

    void Awake(){
        if (Instance != null){        // 이미 GameManager 가 있으면
            Destroy(gameObject);      // 나는 중복이니까 지우고
            return;                   // 아래 준비는 할 필요 없음
        }

        Instance = this;
        FindScorePop();
    }

    // 점수 글자에 붙은 "톡" 효과를 찾아 둔다 (없으면 효과 없이 동작)
    private void FindScorePop(){
        if (scoreText == null) return;
        scorePop = scoreText.GetComponent<ITextPopEffect>();
    }

    public void AddScore(int scoreToAdd){
        currentScore += scoreToAdd;
        UpdateScoreText();
        if (scorePop != null) scorePop.PlayPop(); //실제 점수 숫자에 팝업효과

        if (ScoreChanged != null) ScoreChanged(currentScore); // 점수가 바뀌었다고 알리기ㄴ
    }
    
    // 핀이 "쓰러졌어" 하고 알려오면: 점수 올리고, 그 위치에 숫자 띄우기
    public void OnPinFallen(Vector3 position){
        AddScore(pinScore);
        EffectNumFlying.Show(position, pinScore, popupColor);
    }


    void UpdateScoreText(){
        if (scoreText != null){
            scoreText.text = "Score : " + currentScore.ToString();
        }
    }

    public void Reset(){
        currentScore = 0;
        UpdateScoreText();
    }
}
