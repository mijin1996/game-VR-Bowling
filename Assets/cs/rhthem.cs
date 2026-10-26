using System.Collections.Generic;
using UnityEngine;

public class rhthem : MonoBehaviour {
    [Header("음악")]
    [Tooltip("비워 두면 SoundManager 의 BGM AudioSource 를 사용")]
    [SerializeField] private AudioSource music;

    [Header("천장 효과")]
    [Tooltip("깜빡일 천장 효과 오브젝트들 (부모를 넣으면 자식까지 모두 적용)")]
    [SerializeField] private List<GameObject> ceilingEffects = new List<GameObject>();

    [Header("박자 (알면 넣기, 모르면 0 → 자동)")]
    [Tooltip("노래별 BPM. SoundManager 의 Songs 와 같은 순서 (0 이면 자동)")]
    [SerializeField] private List<float> songBpms = new List<float>();
    [Tooltip("모든 노래에 공통으로 쓸 BPM (0 이면 자동)")]
    [SerializeField] private float bpm = 0f;
    [Tooltip("한 마디의 박 수 (4/4 박자면 4)")]
    [SerializeField] private int beatsPerBar = 4;
    [Tooltip("음악 시작과 첫 박 사이의 시간 차이 (초). BPM 을 넣었을 때만 사용")]
    [SerializeField] private float beatOffset = 0f;

    [Header("자동 박 찾기 (BPM = 0 일 때)")]
    [Tooltip("저음이 평소보다 얼마나 튀어야 '쿵'으로 볼지 (낮추면 더 잘 잡고, 높이면 확실한 것만)")]
    [SerializeField] private float sensitivity = 1.5f;
    [Tooltip("이 주파수(Hz) 아래 소리만 듣는다 (드럼·베이스)")]
    [SerializeField] private float bassMaxHz = 150f;
    [Tooltip("자동으로 찾는 빠르기의 범위 (BPM)")]
    [SerializeField] private Vector2 bpmRange = new Vector2(70f, 180f);

    [Header("밝기")]
    [Tooltip("꺼졌을 때 밝기 (0 = 완전히 꺼짐, 1 = 원래 밝기)")]
    [Range(0f, 1f)] [SerializeField] private float offLevel = 0.1f;
    [Tooltip("일반 박에서 켜지는 비율 (0.5 = 절반 정도)")]
    [Range(0f, 1f)] [SerializeField] private float onChance = 0.5f;
    [Tooltip("한 박 중에서 밝기가 바뀌는 데 쓰는 비율 (작을수록 톡톡 끊기고, 클수록 부드럽게)")]
    [Range(0.05f, 1f)] [SerializeField] private float fadePortion = 0.35f;
    [Tooltip("박을 아직 모를 때 밝기 변하는 속도")]
    [SerializeField] private float fadeSpeed = 8f;

    private class Target {
        public Light light;
        public float baseIntensity;
        public Renderer renderer;
        public Color baseEmission;
        public string colorProperty;
        public float current = 1f;
        public float goal = 1f;
    }

    private readonly List<Target> targets = new List<Target>();
    private MaterialPropertyBlock block;

    private float beatLength;
    private int beatCount;
    private int lastBeatIndex = -1;
    private float lastBeatTime;

    private readonly float[] spectrum = new float[1024];
    private float prevBass;
    private float fluxMean, fluxVar;
    private float lastOnsetTime = -10f;
    private readonly List<float> intervals = new List<float>();
    private const int MaxIntervals = 16;

    private void Start() {
        if (music == null && SoundManager.instance != null) music = SoundManager.instance.bgmAudioSource;
        if (SoundManager.instance != null) SoundManager.instance.SongChanged += OnSongChanged;
        block = new MaterialPropertyBlock();
        CollectTargets();

        // 설정이 빠졌으면 Console 에 알려 주기
        if (music == null) Debug.LogWarning("[rhthem] 들을 음악(AudioSource)이 없어요. SoundManager 가 씬에 있는지 확인하세요.");
        if (ceilingEffects.Count == 0) Debug.LogWarning("[rhthem] Ceiling Effects 칸이 비어 있어요. 천장 효과 오브젝트를 끌어다 넣으세요.");
        else if (targets.Count == 0) Debug.LogWarning("[rhthem] 넣은 오브젝트에 Light 나 Renderer 가 없어서 깜빡일 게 없어요.");
        else Debug.Log("[rhthem] 깜빡일 대상 " + targets.Count + "개 준비 완료");
    }

    private void OnDestroy() {
        if (SoundManager.instance != null) SoundManager.instance.SongChanged -= OnSongChanged;
    }

    // 노래가 바뀌면 빠르기를 처음부터 다시 잰다
    private void OnSongChanged(int index) {
        ResetBeat();
    }

    private void ResetBeat() {
        beatLength = 0f;
        beatCount = 0;
        lastBeatIndex = -1;
        lastBeatTime = 0f;
        lastOnsetTime = -10f;
        intervals.Clear();
        prevBass = 0f;
        fluxMean = 0f;
        fluxVar = 0f;
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
                if (m.HasProperty("_EmissionColor") && m.IsKeywordEnabled("_EMISSION")) {
                    t.colorProperty = "_EmissionColor";       // 발광 재질 → 발광 밝기 조절
                } else if (m.HasProperty("_BaseColor")) {
                    t.colorProperty = "_BaseColor";           // 발광이 없으면 → 색 밝기 조절
                } else if (m.HasProperty("_Color")) {
                    t.colorProperty = "_Color";
                } else continue;
                t.baseEmission = m.GetColor(t.colorProperty);
                targets.Add(t);
            }
        }
    }

    private void Update() {
        bool playing = music != null && music.isPlaying;

        if (!playing) {
            foreach (Target t in targets) t.goal = 1f;      // 음악이 멈추면 모두 원래 밝기로
        } else {
            float knownBpm = CurrentKnownBpm();
            if (knownBpm > 0f) UpdateKnownBpm(knownBpm);
            else UpdateAutoTempo();
        }

        // 부드럽게 밝기 바꾸기 — 한 박이 짧은 빠른 곡일수록 빨리 바뀐다
        float speed = beatLength > 0f ? 1f / Mathf.Max(0.02f, beatLength * fadePortion) : fadeSpeed;
        float k = 1f - Mathf.Exp(-speed * 3f * Time.unscaledDeltaTime);
        foreach (Target t in targets) {
            t.current = Mathf.Lerp(t.current, t.goal, k);
            Apply(t);
        }
    }

    // 지금 노래의 BPM (노래별 칸 → 공통 칸 → 0 = 자동)
    private float CurrentKnownBpm() {
        int song = SoundManager.instance != null ? SoundManager.instance.CurrentSongIndex : -1;
        if (song >= 0 && song < songBpms.Count && songBpms[song] > 0f) return songBpms[song];
        return bpm;
    }

    // BPM 을 알 때: 음악 재생 시간으로 지금이 몇 번째 박인지 계산
    private void UpdateKnownBpm(float knownBpm) {
        beatLength = 60f / knownBpm;
        int beatIndex = Mathf.FloorToInt((music.time - beatOffset) / beatLength);
        if (beatIndex < lastBeatIndex) lastBeatIndex = -1;          // 노래가 처음으로 돌아감 (반복)
        if (beatIndex < 0 || beatIndex == lastBeatIndex) return;
        lastBeatIndex = beatIndex;
        OnBeat(beatIndex % Mathf.Max(1, beatsPerBar) == 0);
    }

    // BPM 을 모를 때: 저음 "쿵" 간격으로 빠르기를 알아내고, 그 빠르기로 박을 친다
    private void UpdateAutoTempo() {
        float now = music.time;
        if (now < lastBeatTime) ResetBeat();                         // 노래가 처음으로 돌아감

        // ① 저음 세기
        music.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float binHz = AudioSettings.outputSampleRate * 0.5f / spectrum.Length;
        int maxBin = Mathf.Clamp(Mathf.CeilToInt(bassMaxHz / binHz), 1, spectrum.Length);
        float bass = 0f;
        for (int i = 0; i < maxBin; i++) bass += spectrum[i];

        // ② 저음이 갑자기 커진 양 (줄어드는 건 무시)
        float flux = Mathf.Max(0f, bass - prevBass);
        prevBass = bass;

        // ③ 평소 수준(평균·흔들림)보다 확 튀면 "쿵"
        float a = 1f - Mathf.Exp(-Time.unscaledDeltaTime / 1.5f);            // 최근 1.5초 정도를 평소로 봄
        float diff = flux - fluxMean;
        fluxMean += a * diff;
        fluxVar = (1f - a) * (fluxVar + a * diff * diff);
        float threshold = fluxMean + sensitivity * Mathf.Sqrt(fluxVar);

        float minGap = beatLength > 0f ? beatLength * 0.5f : 60f / bpmRange.y;
        if (flux > threshold && flux > 1e-4f && now - lastOnsetTime >= minGap) {
            RegisterOnset(now);
        }

        // ④ 빠르기를 알면 그 간격으로 박 (쿵을 놓쳐도 박자는 계속 이어짐)
        if (beatLength > 0f && now - lastBeatTime >= beatLength) {
            lastBeatTime += beatLength;
            if (now - lastBeatTime > beatLength) lastBeatTime = now;   // 많이 밀렸으면 지금으로 맞춤
            Beat();
        }
    }

    // "쿵" 이 들렸을 때: 간격을 모아서 빠르기 계산, 박 위치도 쿵에 맞춰 살짝 당기기
    private void RegisterOnset(float time) {
        float gap = time - lastOnsetTime;
        lastOnsetTime = time;

        float minLen = 60f / bpmRange.y, maxLen = 60f / bpmRange.x;
        if (gap > 0f && gap < maxLen * 4f) {
            while (gap > maxLen) gap *= 0.5f;                         // 2박, 4박 건너뛴 간격은 한 박으로
            while (gap < minLen) gap *= 2f;                           // 반 박 간격은 한 박으로
            intervals.Add(gap);
            if (intervals.Count > MaxIntervals) intervals.RemoveAt(0);
        }

        if (intervals.Count >= 4) {
            float median = Median(intervals);
            beatLength = beatLength > 0f ? Mathf.Lerp(beatLength, median, 0.3f) : median;   // 갑자기 튀지 않게
        }

        if (beatLength <= 0f) {
            Beat();                                                   // 아직 빠르기를 모르면 쿵마다 깜빡
            lastBeatTime = time;
        } else {
            float phase = time - lastBeatTime;                        // 박자 칸과 쿵이 어긋난 정도
            if (phase < beatLength * 0.3f) lastBeatTime = time;       // 박 직후의 쿵 → 박 위치를 쿵에 맞춤
            else if (phase > beatLength * 0.7f) { lastBeatTime = time; Beat(); }   // 다음 박 직전의 쿵 → 바로 박
        }
    }

    private void Beat() {
        OnBeat(beatCount % Mathf.Max(1, beatsPerBar) == 0);
        beatCount++;
    }

    private static float Median(List<float> values) {
        List<float> sorted = new List<float>(values);
        sorted.Sort();
        return sorted[sorted.Count / 2];
    }

    // 박마다: 마디 첫 박은 모두 켜고, 나머지 박은 랜덤으로 일부만 켠다
    private void OnBeat(bool downBeat) {
        foreach (Target t in targets) {
            t.goal = (downBeat || Random.value < onChance) ? 1f : offLevel;
        }
    }

    private void Apply(Target t) {
        if (t.light != null) {
            t.light.intensity = t.baseIntensity * t.current;
            return;
        }
        if (t.renderer == null) return;
        t.renderer.GetPropertyBlock(block);
        Color c = t.baseEmission * t.current;
        if (t.colorProperty != "_EmissionColor") c.a = t.baseEmission.a;   // 색 밝기만 바꾸고 투명도는 그대로
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
