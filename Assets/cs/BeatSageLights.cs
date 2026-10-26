using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatSageLights : MonoBehaviour {

    [Serializable]
    public class SongMap {
        [Tooltip("이 맵을 쓸 노래 (비우면 SoundManager Songs 와 같은 순서로 짝지음)")]
        public AudioClip song;
        [Tooltip("Beat Sage 난이도 파일 (.json 으로 바꾼 것, 예: Expert.json)")]
        public TextAsset beatMap;
        [Tooltip("Beat Sage Info 파일 (Info.json). 비우면 BPM 120 으로 계산")]
        public TextAsset info;
        [Tooltip("깜빡임이 노래보다 빠르면 +, 늦으면 - (초)")]
        public float offset = 0f;
    }

    [Header("음악")]
    [Tooltip("비워 두면 SoundManager 의 BGM AudioSource 를 사용")]
    [SerializeField] private AudioSource music;

    [Header("노래별 Beat Sage 맵 (SoundManager Songs 와 같은 순서)")]
    [SerializeField] private List<SongMap> songMaps = new List<SongMap>();

    [Header("천장 효과")]
    [Tooltip("깜빡일 천장 효과 오브젝트들 (부모를 넣으면 자식까지 모두 적용)")]
    [SerializeField] private List<GameObject> ceilingEffects = new List<GameObject>();

    [Header("깜빡임")]
    [Tooltip("꺼졌을 때 밝기 (0 = 완전히 꺼짐, 1 = 원래 밝기)")]
    [Range(0f, 1f)] [SerializeField] private float offLevel = 0.1f;
    [Tooltip("노트 1개짜리 박에서 켜지는 비율")]
    [Range(0f, 1f)] [SerializeField] private float onChance = 0.5f;
    [Tooltip("다음 노트까지 시간 중 밝게 유지하는 비율 (나머지 동안 어두워짐)")]
    [Range(0.05f, 1f)] [SerializeField] private float holdPortion = 0.3f;
    [Tooltip("켜질 때 걸리는 시간 (초). 0 이면 번쩍 바로")]
    [SerializeField] private float riseTime = 0.03f;
    [Tooltip("이 시간(초) 안에 몰린 노트는 '같은 순간'으로 봄")]
    [SerializeField] private float sameMomentWindow = 0.03f;
    [Tooltip("노트가 없는 긴 구간에서는 원래 밝기로 돌아갈지")]
    [SerializeField] private bool restoreWhenQuiet = true;

    [Serializable] private class V2Note { public float _time; public int _type; }
    [Serializable] private class V2Map { public List<V2Note> _notes; }
    [Serializable] private class V3Note { public float b; public int c; }
    [Serializable] private class V3Map { public List<V3Note> colorNotes; }
    [Serializable] private class InfoFile { public float _beatsPerMinute; }

    private struct Hit {
        public float time;
        public int count;
    }

    private class Target {
        public Light light;
        public float baseIntensity;
        public Renderer renderer;
        public Color baseColor;
        public string colorProperty;
        public float current = 1f;
        public float goal = 1f;
        public float holdUntil;
    }

    private readonly List<Target> targets = new List<Target>();
    private readonly Dictionary<int, List<Hit>> cache = new Dictionary<int, List<Hit>>();
    private MaterialPropertyBlock block;

    private List<Hit> hits;
    private int mapIndex = -1;
    private int next;
    private float lastTime;
    private float fadeTime = 0.2f;

    private void Start() {
        if (music == null && SoundManager.instance != null) music = SoundManager.instance.bgmAudioSource;
        block = new MaterialPropertyBlock();
        CollectTargets();

        if (music == null) Debug.LogWarning("[BeatSageLights] 들을 음악(AudioSource)이 없어요.");
        if (songMaps.Count == 0) Debug.LogWarning("[BeatSageLights] Song Maps 칸이 비어 있어요. 노래마다 Beat Sage 파일을 넣으세요.");
        if (targets.Count == 0) Debug.LogWarning("[BeatSageLights] 깜빡일 천장 효과(Light / Renderer)가 없어요. Ceiling Effects 칸을 확인하세요.");
        else Debug.Log("[BeatSageLights] 깜빡일 대상 " + targets.Count + "개 준비 완료");
    }

    private void Update() {
        bool playing = music != null && music.isPlaying && music.clip != null;
        if (!playing) {
            foreach (Target t in targets) t.goal = 1f;            // 음악이 멈추면 원래 밝기로
            Smooth();
            return;
        }

        // 지금 노래에 맞는 맵 고르기 (노래가 바뀌면 다시 고름)
        int index = FindMapIndex();
        if (index != mapIndex) {
            mapIndex = index;
            hits = index >= 0 ? LoadHits(index) : null;
            SeekTo(music.time);
        }
        if (hits == null) { Smooth(); return; }

        float now = music.time + songMaps[mapIndex].offset;
        if (now < lastTime - 0.1f) SeekTo(now);                   // 노래가 처음으로 돌아감(반복) 또는 다시 재생

        // 지나간 순간들을 차례로 처리
        while (next < hits.Count && hits[next].time <= now) {
            float gapToNext = next + 1 < hits.Count ? hits[next + 1].time - hits[next].time : 1f;
            Flash(hits[next], now, gapToNext);
            next++;
        }

        // 노트가 한참(앞뒤 2초 이상) 없는 조용한 구간 → 원래 밝기로, 아니면 유지 시간이 끝난 효과는 어둡게
        bool quiet = restoreWhenQuiet && QuietNow(now);
        foreach (Target t in targets) {
            if (quiet) t.goal = 1f;
            else if (t.goal > offLevel && now > t.holdUntil) t.goal = offLevel;
        }

        lastTime = now;
        Smooth();
    }

    private bool QuietNow(float now) {
        float waitNext = next < hits.Count ? hits[next].time - now : 999f;
        float sinceLast = next > 0 ? now - hits[next - 1].time : 999f;
        return waitNext > 2f && sinceLast > 2f;
    }

    // 한 순간 깜빡이기
    private void Flash(Hit hit, float now, float gapToNext) {
        bool strong = hit.count >= 2;
        float hold = Mathf.Clamp(gapToNext * holdPortion, 0.05f, 0.5f);
        fadeTime = Mathf.Clamp(gapToNext * (1f - holdPortion) * 0.5f, 0.04f, 0.6f);   // 다음 노트 전까지 어두워지게

        foreach (Target t in targets) {
            if (strong || UnityEngine.Random.value < onChance) {
                t.goal = 1f;
                t.holdUntil = now + hold;
            } else {
                t.goal = offLevel;
            }
        }
    }

    // 밝기를 부드럽게 바꾸기 (켜질 땐 빠르게, 꺼질 땐 fadeTime 동안)
    private void Smooth() {
        foreach (Target t in targets) {
            float time = t.goal > t.current ? riseTime : fadeTime;
            float k = time <= 0f ? 1f : 1f - Mathf.Exp(-3f * Time.unscaledDeltaTime / time);
            t.current = Mathf.Lerp(t.current, t.goal, k);
            Apply(t);
        }
    }

    // 음악 시간에 맞춰 "다음 순간" 위치 다시 찾기
    private void SeekTo(float time) {
        next = 0;
        lastTime = time;
        if (hits == null) return;
        while (next < hits.Count && hits[next].time < time) next++;
    }

    // 지금 나오는 노래에 맞는 맵 번호 (Song 칸 일치 → 노래 목록 순서)
    private int FindMapIndex() {
        for (int i = 0; i < songMaps.Count; i++) {
            if (songMaps[i].song != null && songMaps[i].song == music.clip && songMaps[i].beatMap != null) return i;
        }
        int song = SoundManager.instance != null ? SoundManager.instance.CurrentSongIndex : -1;
        if (song >= 0 && song < songMaps.Count && songMaps[song].song == null && songMaps[song].beatMap != null) return song;
        return -1;
    }

    // 맵 파일 → 깜빡일 순간 목록 (한 번 읽으면 기억해 둠)
    private List<Hit> LoadHits(int index) {
        List<Hit> list;
        if (cache.TryGetValue(index, out list)) return list;

        SongMap map = songMaps[index];
        float bpm = 120f;
        if (map.info != null) {
            InfoFile info = JsonUtility.FromJson<InfoFile>(map.info.text);
            if (info != null && info._beatsPerMinute > 0f) bpm = info._beatsPerMinute;
        }
        float secPerBeat = 60f / bpm;

        // 노트 시간(박)만 꺼내기 (v2: _notes._time / v3: colorNotes.b)
        List<float> beats = new List<float>();
        V2Map v2 = JsonUtility.FromJson<V2Map>(map.beatMap.text);
        if (v2 != null && v2._notes != null && v2._notes.Count > 0) {
            foreach (V2Note n in v2._notes) beats.Add(n._time);
        } else {
            V3Map v3 = JsonUtility.FromJson<V3Map>(map.beatMap.text);
            if (v3 != null && v3.colorNotes != null) foreach (V3Note n in v3.colorNotes) beats.Add(n.b);
        }
        beats.Sort();

        // 같은 순간끼리 묶기
        list = new List<Hit>();
        foreach (float beat in beats) {
            float sec = beat * secPerBeat;
            if (list.Count > 0 && sec - list[list.Count - 1].time <= sameMomentWindow) {
                Hit h = list[list.Count - 1];
                h.count++;
                list[list.Count - 1] = h;
            } else {
                list.Add(new Hit { time = sec, count = 1 });
            }
        }

        cache[index] = list;
        Debug.Log("[BeatSageLights] " + map.beatMap.name + " : 깜빡일 순간 " + list.Count + "개 (BPM " + bpm + ")");
        return list;
    }

    // 넣어 둔 오브젝트(자식 포함)에서 조명과 발광 렌더러를 모은다
    private void CollectTargets() {
        targets.Clear();
        foreach (GameObject root in ceilingEffects) {
            if (root == null) continue;
            foreach (Light l in root.GetComponentsInChildren<Light>(true)) {
                targets.Add(new Target { light = l, baseIntensity = l.intensity });
            }
            foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) {
                Material m = r.sharedMaterial;
                if (m == null) continue;
                Target t = new Target { renderer = r };
                if (m.HasProperty("_EmissionColor") && m.IsKeywordEnabled("_EMISSION")) t.colorProperty = "_EmissionColor";
                else if (m.HasProperty("_BaseColor")) t.colorProperty = "_BaseColor";
                else if (m.HasProperty("_Color")) t.colorProperty = "_Color";
                else continue;
                t.baseColor = m.GetColor(t.colorProperty);
                targets.Add(t);
            }
        }
    }

    private void Apply(Target t) {
        if (t.light != null) {
            t.light.intensity = t.baseIntensity * t.current;
            return;
        }
        if (t.renderer == null) return;
        t.renderer.GetPropertyBlock(block);
        Color c = t.baseColor * t.current;
        if (t.colorProperty != "_EmissionColor") c.a = t.baseColor.a;
        block.SetColor(t.colorProperty, c);
        t.renderer.SetPropertyBlock(block);
    }

    // 꺼질 때 원래 밝기로 돌려놓기
    private void OnDisable() {
        if (block == null) return;
        foreach (Target t in targets) {
            t.current = 1f;
            Apply(t);
        }
    }
}
