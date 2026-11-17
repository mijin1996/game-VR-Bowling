using UnityEngine;

public class SoundManager : MonoBehaviour{
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip bgmClip;

    [Header("노래 목록 (SongList 에서 고르면 그 노래가 나옴)")]
    [Tooltip("노래 파일들을 순서대로 넣기 (예: 5곡)")]
    public AudioClip[] songs;
    public int CurrentSongIndex { get; private set; } = -1;
    public event System.Action<int> SongChanged;
    public AudioClip hitClip;
    public AudioClip pinResetClip; 

    public static SoundManager instance;

    [Header("투구 결과 소리 (인스펙터에서 넣기)")]
    public AudioClip missClip;
    public AudioClip greatClip;
    public AudioClip gorgeousClip;
    public AudioClip niceJobClip;
    public AudioClip failClip;
    public AudioClip oneMoreClip;


    [Header("공 굴러가는 소리")]
    public AudioSource ballRollAudioSource;
    public AudioClip ballRollClip;


    void Awake(){
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public enum SongEnd { NextSong, RepeatSong, Stop }
    [Header("노래가 끝나면")]
    [Tooltip("NextSong: 다음 노래 (마지막이면 첫 곡) / RepeatSong: 같은 노래 다시 / Stop: 멈춤")]
    public SongEnd onSongEnd = SongEnd.NextSong;

    private bool musicOn;
    public bool MusicOn => musicOn;

    // 토글: 음악 켜기 / 끄기만 담당 (어떤 노래인지는 SongList 에서 고른 것)
    public void ToggleBGM(){
        musicOn = !musicOn;
        stoppedAtEnd = false;
        if (musicOn) {
            if (bgmAudioSource.clip == null) {
                if (songs == null || songs.Length == 0) return;
                SelectSong(CurrentSongIndex >= 0 ? CurrentSongIndex : 0);   // 고른 노래가 없으면 첫 곡
            }
            if (bgmAudioSource.time > 0f) bgmAudioSource.UnPause();          // 끈 자리에서 이어서
            else bgmAudioSource.Play();
        } else {
            bgmAudioSource.Pause();
        }
    }

    // 노래 목록에서 i 번째 노래 고르기
    // - 음악이 켜져 있으면 → 바로 그 노래로 바꿔서 처음부터 재생
    // - 음악이 꺼져 있으면 → 노래만 바꿔 두고, 토글을 켜면 그 노래가 나옴
    public void PlaySong(int index){
        if (!SelectSong(index)) return;
        stoppedAtEnd = false;
        if (musicOn) bgmAudioSource.Play();
    }

    // 노래 바꾸기 (재생은 하지 않음)
    private bool SelectSong(int index){
        if (songs == null || index < 0 || index >= songs.Length || songs[index] == null) return false;
        bgmAudioSource.Stop();
        bgmAudioSource.clip = songs[index];
        bgmAudioSource.loop = false;           // 끝난 것을 알아채서 다음 곡으로 넘어가기 위해 (반복은 onSongEnd 로)
        CurrentSongIndex = index;
        if (SongChanged != null) SongChanged(index);
        return true;
    }

    // 음악이 켜져 있는데 재생이 멈췄다 = 노래가 끝났다 → 다음 곡 / 같은 곡 / 멈춤
    private void Update(){
        if (!musicOn || stoppedAtEnd || bgmAudioSource == null || bgmAudioSource.clip == null) return;
        if (bgmAudioSource.isPlaying) return;

        switch (onSongEnd) {
            case SongEnd.RepeatSong:
                bgmAudioSource.Play();
                break;
            case SongEnd.NextSong:
                int next = NextSongIndex();
                if (next >= 0) PlaySong(next);
                break;
            case SongEnd.Stop:
                bgmAudioSource.Stop();         // 처음으로 되감기 (토글은 켜진 그대로, 노래 버튼을 누르면 바로 재생)
                stoppedAtEnd = true;
                break;
        }
    }
    private bool stoppedAtEnd;

    // 다음 노래 번호 (빈 칸은 건너뜀, 마지막이면 처음으로)
    private int NextSongIndex(){
        if (songs == null || songs.Length == 0) return -1;
        for (int i = 1; i <= songs.Length; i++) {
            int n = (CurrentSongIndex + i) % songs.Length;
            if (songs[n] != null) return n;
        }
        return -1;
    }

    public void PlaySFX(AudioClip clip){
        if (clip != null){
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    

    // 기존 함수들 아래에 추가
    // 굴러가는 소리 켜기 (이미 나고 있으면 그대로)
    public void PlayBallRoll(){
        if (ballRollAudioSource == null || ballRollAudioSource.isPlaying) return;   // 이미 나는 중이면 겹치지 않게
        ballRollAudioSource.clip = ballRollClip;
        ballRollAudioSource.loop = false;      // ← true 에서 false 로: 반복하지 않음
        ballRollAudioSource.Play();
    }
}
