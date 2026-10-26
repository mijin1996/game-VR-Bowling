using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour{
    public bool isFallen = false;

    [Header("고정")]
    [Tooltip("공(또는 움직이는 핀)이 이 거리(m) 안으로 들어오면 고정을 푼다")]
    [SerializeField] private float unfreezeMargin = 0.05f;

    private bool isFrozen;
    private float reach;

    private void OnTriggerEnter(Collider other){
        if (isFallen) return;                                    // 이미 쓰러진 핀은 무시
        if (other.gameObject.CompareTag("Floor")){
            MarkFallen();
        }
    }

    // 쓰러짐 처리 (점수 +, 소리) — 머리가 바닥에 닿았을 때 / 많이 기울었을 때 둘 다 여기로
    private void MarkFallen(){
        if (isFallen) return;
        isFallen = true;
        Debug.Log("[Pin] 쓰러짐 +점수: " + name + " (" + (transform.parent != null ? transform.parent.name : "-") + ")");
        if (GameManager.Instance != null){
            SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
            GameManager.Instance.OnPinFallen(TopPosition()); // 점수, 숫자는 GameManager 가
        }
    }

    [Header("쓰러짐 판단")]
    [Tooltip("처음 서 있던 방향에서 이 각도(도) 이상 기울면 쓰러진 것으로 본다")]
    [SerializeField] private float fallenAngle = 45f;
    private Vector3 localUp;

    private void LateUpdate(){
        if (isFallen || isFrozen) return;                       // 고정된(아직 공이 안 온) 핀은 검사 안 함
        Vector3 up = transform.TransformDirection(localUp);
        if (Vector3.Angle(up, Vector3.up) >= fallenAngle) MarkFallen();
    }

    // 이 핀의 머리 위치 (핀 모양 기준)
    private Vector3 TopPosition(){
        Renderer r = GetComponent<Renderer>();
        if (r == null) return transform.position + Vector3.up * 0.3f;
        Bounds b = r.bounds;
        return new Vector3(b.center.x, b.max.y + 0.15f, b.center.z);
    }

    // 다시 세우기는 이제 LevelManager 가 "한 번 던진 결과"를 판정한 뒤에 한다 (ResetAllPins 호출)

    private static readonly List<Pin> allPins = new List<Pin>();

    public static int FallenCount {
        get {
            int count = 0;
            foreach (Pin pin in allPins) {
                if (pin.isFallen) count++;
            }
            return count;
        }
    }

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;   // 게임 시작 때 자리를 기억해 둔다
        startRotation = transform.rotation;
        localUp = transform.InverseTransformDirection(Vector3.up);

        Renderer r = GetComponentInChildren<Renderer>();
        reach = (r != null) ? r.bounds.extents.magnitude : 0.2f;
        Freeze();
    }

    // PinSpawner 가 핀을 바닥에 맞춰 옮긴 뒤 부른다 → 그 자리를 "처음 자리"로 다시 기억 (다시 세울 때 이 자리로)
    public void RememberStartPose() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        localUp = transform.InverseTransformDirection(Vector3.up);
    }

    // 고정: 물리를 꺼서 제자리에 가만히 서 있게
    private void Freeze() {
        isFrozen = true;
        if (rb != null) rb.isKinematic = true;
    }

    // 고정 풀기: 물리를 켜서 맞으면 쓰러질 수 있게
    private void Unfreeze() {
        if (!isFrozen) return;
        isFrozen = false;
        if (rb != null) {
            rb.isKinematic = false;
            rb.WakeUp();
        }
    }

    // 고정된 핀: 공이나 "움직이는 핀"이 가까이 왔는지 매 물리 프레임 확인 (닿기 직전에 풀어야 공이 튕겨나지 않음)
    private void FixedUpdate() {
        if (!isFrozen) return;

        Vector3 center = transform.position;
        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null) center = r.bounds.center;

        Collider[] near = Physics.OverlapSphere(center, reach + unfreezeMargin, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider c in near) {
            if (c.transform.IsChildOf(transform)) continue;

            // 굴러오는 공만 고정을 푼다 (다시 세운 뒤 핀 사이에 멈춰 있는 공 때문에 핀이 바로 풀려 넘어지지 않게)
            Ball ball = c.GetComponentInParent<Ball>();
            if (ball != null) {
                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                if (ballRb == null || ballRb.linearVelocity.sqrMagnitude > 0.04f) { Unfreeze(); return; }
                continue;
            }

            Pin other = c.GetComponentInParent<Pin>();
            if (other != null && !other.isFrozen && other.rb != null && other.rb.linearVelocity.sqrMagnitude > 0.01f) {
                Unfreeze();
                return;
            }
        }
    }

    // 켜질 때 명단에 이름 올리고, 꺼질 때 뺀다
    private void OnEnable() {
        allPins.Add(this);
    }

    private void OnDisable() {
        allPins.Remove(this);
    }

    // 이 핀 하나를 처음 자리로 되돌린다
    // 순서: 힘 없애기 → 물리 끄기(고정) → 물리 몸(rb)과 눈에 보이는 몸(transform) 둘 다 처음 자리로
    //  (transform 만 옮기면 물리 쪽 위치가 남아 있어서 핀이 제자리로 안 돌아오는 경우가 있었음)
    public void ResetPin() {
        if (rb != null) {
            if (!rb.isKinematic) {
                rb.linearVelocity = Vector3.zero;    // 날아가던 힘 없애기 (Unity 6 이전이면 rb.velocity)
                rb.angularVelocity = Vector3.zero;   // 돌던 힘 없애기
            }
            Freeze();                                // 다시 공이 올 때까지 고정 (물리 끄기)
            rb.position = startPosition;
            rb.rotation = startRotation;
        } else {
            Freeze();
        }
        transform.SetPositionAndRotation(startPosition, startRotation);
        isFallen = false;                        // 다음 판에 다시 점수 받을 수 있게
    }

    // 모든 핀을 한 번에 다시 세운다
    public static void ResetAllPins() {
        foreach (Pin pin in allPins) {
            pin.ResetPin();
        }
        Physics.SyncTransforms();                // 옮긴 자리를 물리에 바로 반영
        Debug.Log("[Pin] 핀 " + allPins.Count + "개 다시 세움");

        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.pinResetClip);
    }

}
