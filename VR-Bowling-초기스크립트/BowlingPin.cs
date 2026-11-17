using UnityEngine;

public class BowlingPin : MonoBehaviour {
    public bool isFallen = false;

        private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Floor")) {
            isFallen = true;
            if (GameManager.gm != null) {
                GameManager.gm.AddScore(100);
                SoundManager.sm.PlaySFX(SoundManager.sm.hitClip);
            }
        }
    }

}
