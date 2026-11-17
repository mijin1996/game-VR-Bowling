using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongListUI : MonoBehaviour {
    [Tooltip("노래 버튼들 (위에서부터 1번 노래, 2번 노래 ...)")]
    [SerializeField] private List<Button> songButtons = new List<Button>();
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.9f, 1f);

    private void Start() {
        SoundManager sm = SoundManager.instance;
        for (int i = 0; i < songButtons.Count; i++) {
            Button b = songButtons[i];
            if (b == null) continue;

            bool hasSong = sm != null && sm.songs != null && i < sm.songs.Length && sm.songs[i] != null;

            // 버튼은 항상 보이게 두고, 노래가 없는 칸은 글자로 알려 준다 (예전엔 숨겨서 버튼이 사라진 것처럼 보였음)
            TMP_Text label = b.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = hasSong ? sm.songs[i].name : "(empty)";
            b.interactable = hasSong;
            if (!hasSong) {
                Debug.LogWarning("[SongListUI] " + b.name + " : SoundManager 의 Songs " + (i + 1) + "번 칸이 비어 있어요.");
                continue;
            }

            int index = i;                                   // 버튼마다 자기 번호 기억
            b.onClick.AddListener(() => SoundManager.instance.PlaySong(index));
        }

        if (sm != null) {
            sm.SongChanged += Highlight;
            Highlight(sm.CurrentSongIndex);
        }
    }

    private void OnDestroy() {
        if (SoundManager.instance != null) SoundManager.instance.SongChanged -= Highlight;
    }

    // 고른 노래 버튼만 색 바꾸기
    private void Highlight(int selected) {
        for (int i = 0; i < songButtons.Count; i++) {
            if (songButtons[i] == null) continue;
            Graphic g = songButtons[i].targetGraphic;
            if (g != null) g.color = (i == selected) ? selectedColor : normalColor;
        }
    }
}
