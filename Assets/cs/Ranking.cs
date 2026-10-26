using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankEntry {
    public long id;
    public int score;
    public int level;
    public string date;
}

public static class Ranking {
    private const string SaveKey = "BowlingRanking";
    private const int MaxCount = 10;
    public static event Action Changed;

    [Serializable]
    private class RankList {
        public List<RankEntry> items = new List<RankEntry>();
    }

    // 이번 판 기록 저장 (이미 있으면 최신 값으로 갱신) → 높은 점수 순으로 정렬해서 보관
    public static void Save(long id, int score, int level) {
        RankList list = Load();

        RankEntry entry = list.items.Find(e => e.id == id);
        if (entry == null) {
            entry = new RankEntry { id = id };
            list.items.Add(entry);
        }
        entry.score = score;
        entry.level = level;
        entry.date = DateTime.Now.ToString("MM/dd HH:mm");

        list.items.Sort(CompareRank);                                       // 1위가 맨 앞
        if (list.items.Count > MaxCount) {
            list.items.RemoveRange(MaxCount, list.items.Count - MaxCount); // 10위 밖은 버림
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(list));
        PlayerPrefs.Save();
        if (Changed != null) Changed();   // 랭킹판이 열려 있으면 바로 다시 그림
    }

    // 랭킹 전체 (이미 높은 점수 순으로 정렬되어 있음)
    public static List<RankEntry> GetAll() {
        return Load().items;
    }

    // 정렬 기준: 1순위 점수 높은 순, 2순위 점수가 같으면 레벨 높은 순
    private static int CompareRank(RankEntry a, RankEntry b) {
        int byScore = b.score.CompareTo(a.score);
        if (byScore != 0) return byScore;
        return b.level.CompareTo(a.level);
    }

    private static RankList Load() {
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json)) return new RankList();
        RankList list = JsonUtility.FromJson<RankList>(json);
        return list ?? new RankList();
    }
}