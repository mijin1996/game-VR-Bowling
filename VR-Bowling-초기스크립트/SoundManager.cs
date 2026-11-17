using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource bgm;
    public AudioSource sfx;

    public AudioClip bgmClip;
    public AudioClip hitClip;

    public static SoundManager sm;

    void Awake() {
        if (sm == null) {
            sm = this;
        } else {
            Destroy(gameObject);
        }
    }
 
    public void ToggleBGM() {
        if (bgm.isPlaying)
            bgm.Pause();
        else
            bgm.Play();
 
    }
 
    public void PlaySFX(AudioClip clip) {
        if (clip != null) {
            sfx.PlayOneShot(clip);
        }
    }
}
