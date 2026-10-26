using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;   // XRI 3.x 기준 (2.x 라면 이 줄을 지울 것)
using UnityEngine.XR.Interaction.Toolkit.Interactors;     // XRI 3.x 기준 (2.x 라면 이 줄을 지울 것)

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Ball : MonoBehaviour {
    public event Action<Ball> Thrown;
    public event Action<Ball> OutOfBounds;

    [Header("던짐 판단")]
    [Tooltip("손에서 놓은 위치가 소켓에서 이 거리(m) 안이면 '다시 내려놓기'로 보고 소켓에 되돌린다")]
    [SerializeField] private float leaveSocketDistance = 0.5f;

    [Header("회수 조건")]
    [Tooltip("이 높이(Y축) 밑으로 떨어지면 회수")]
    [SerializeField] private float destroyYLimit = -10f;
    [Tooltip("소켓으로부터 이 거리 이상 멀어지면 회수")]
    [SerializeField] private float maxDistance = 30f;
    [Tooltip("손에서 놓은 뒤 이 시간(초)이 지나면 회수 (레인 끝에 멈춘 공 정리)")]
    [SerializeField] private float maxSecondsAfterThrow = 30f;

    private Rigidbody rb;
    private XRGrabInteractable grab;
    private Transform socket;
    private bool isThrown;
    private bool isHeld;
    private float releasedTime;

    private bool hasPlayedRoll;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    // 공이 켜질 때마다 잡기/놓기 신호를 연결하고, 꺼질 때 끊는다 (창고에서 재사용해도 중복 연결 없음)
    private void OnEnable() {
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable() {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    // 소켓 위치로 옮기고 물리를 깨끗이 초기화한다.
    public void PlaceAt(Transform targetSocket) {
        socket = targetSocket;
        isThrown = false;
        isHeld = false;

        rb.linearVelocity = Vector3.zero;       // Unity 6 이전 버전이면 rb.velocity
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(socket.position, socket.rotation);
        rb.Sleep();                             // 놓자마자 튀는 현상 방지
    }

    // ---- 손으로 잡기 / 놓기 ----
    private void OnGrabbed(SelectEnterEventArgs args) {
        if (args.interactorObject is XRSocketInteractor) return;   // 소켓이 붙잡는 건 손이 아니므로 무시
        isHeld = true;
    }

    private void OnReleased(SelectExitEventArgs args) {
        if (args.interactorObject is XRSocketInteractor) return;
        isHeld = false;
        releasedTime = Time.time;               // 회수 시간은 "놓은 순간"부터 잰다

        if (isThrown || socket == null) return; // 이미 던진 공을 다시 주웠다 놓은 경우: 시간만 새로 잰다

        if (DistanceToSocket() < leaveSocketDistance) {
            PlaceAt(socket);                    // 소켓 근처에 내려놓음: 던진 게 아니므로 제자리로 (5개 방지)
        } else {
            MarkThrown();                       // 소켓에서 먼 곳에서 놓음: 던진 것
        }
    }

    private void MarkThrown() {
        isThrown = true;
         hasPlayedRoll = false;    // 새로 던졌으니 소리 낼 수 있게
        if (Thrown != null) Thrown(this);
    }

    // ---- 던진 공 회수 검사 ----
    private void Update() {
        if (socket == null || !isThrown) return;
        if (isHeld) return;                     // 손에 들고 있는 동안은 절대 회수하지 않는다

        bool tooLow = transform.position.y < destroyYLimit;
        bool tooFar = DistanceToSocket() > maxDistance;
        bool tooLong = Time.time - releasedTime > maxSecondsAfterThrow;

        if (tooLow || tooFar || tooLong) {
            socket = null;                      // 회수될 때까지 다시 검사하지 않도록
            if (OutOfBounds != null) OutOfBounds(this);
        }
    }

    private float DistanceToSocket() {
        return Vector3.Distance(transform.position, socket.position);
    }



    private void OnCollisionEnter(Collision collision) {
        if (!isThrown || hasPlayedRoll) return;                  // 던진 공이 아니거나, 이미 냈으면 무시
        if (!collision.gameObject.CompareTag("Floor")) return;   // 바닥이 아니면 무시

        hasPlayedRoll = true;
        if (SoundManager.instance != null) {
            SoundManager.instance.PlayBallRoll();
        }
    }   
    
}