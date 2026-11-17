using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RankingBoard : MonoBehaviour {
    [Tooltip("표에 보여줄 순위 개수")]
    [SerializeField] private int showCount = 5;

    [Header("칸 방식 (Scene 에서 위치를 직접 정함)")]
    [Tooltip("순위 숫자(1, 2, 3...)가 들어갈 글자")]
    [SerializeField] private TextMeshProUGUI rankColumn;
    [Tooltip("점수가 들어갈 글자")]
    [SerializeField] private TextMeshProUGUI scoreColumn;
    [Tooltip("날짜+시간이 들어갈 글자")]
    [SerializeField] private TextMeshProUGUI dateColumn;

    private void OnEnable() {
        Ranking.Changed += Refresh;   // 열려 있는 동안 기록이 저장되면 바로 다시 그림
        Refresh();
    }

    private void OnDisable() {
        Ranking.Changed -= Refresh;
    }

    public void Refresh() {
        List<RankEntry> list = Ranking.GetAll();   // 이미 1위부터 순서대로

        DrawColumns(list);
    }

    // 칸 방식: 각 칸에 한 줄에 하나씩 채우기만 한다
    private void DrawColumns(List<RankEntry> list) {
        StringBuilder ranks = new StringBuilder();
        StringBuilder scores = new StringBuilder();
        StringBuilder dates = new StringBuilder();

        if (list.Count == 0) {
            ranks.AppendLine("-");
            scores.AppendLine("No records");
            dates.AppendLine("-");
        }

        for (int i = 0; i < list.Count && i < showCount; i++) {
            RankEntry e = list[i];
            ranks.AppendLine((i + 1).ToString());
            scores.AppendLine(e.score.ToString());
            dates.AppendLine(e.date);   // 예: 10/11 22:58
        }

        if (rankColumn != null) rankColumn.text = ranks.ToString();
        if (scoreColumn != null) scoreColumn.text = scores.ToString();
        if (dateColumn != null) dateColumn.text = dates.ToString();
    }
}
