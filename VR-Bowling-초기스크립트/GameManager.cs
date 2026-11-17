using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager gm;
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;

    void Awake() {
        if (gm == null) {
            gm = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void AddScore(int scoreToAdd) {
        currentScore += scoreToAdd;
        UpdateScoreText();
    }

    void UpdateScoreText() {
        if (scoreText != null) {
            scoreText.text = "Score : " + currentScore.ToString();
        }
    }
}