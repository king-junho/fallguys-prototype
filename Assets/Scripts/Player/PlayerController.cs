using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint;

    [Header("Control")]
    public bool canMove = true;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip jumpClip;

    [Header("Animation")]
    public Animator animator;          // 애니메이터 참조
    public float animDampTime = 0.1f;  // Speed 전환 부드럽게

    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public float rotationSmoothTime = 0.1f;  // 회전 부드러움 정도
    private float rotationVelocity;          // SmoothDamp 내부 상태용

    private Rigidbody rb;
    private bool isGrounded = true;

    // Animator 파라미터 해시 (문자열 대신 정수 해시 사용)
    private int hashSpeed;
    private int hashIsGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // 자식에서 Animator 자동으로 찾기 (Inspector에서 안 넣어도 되게)
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Animator 파라미터 이름과 일치해야 함
        hashSpeed = Animator.StringToHash("Speed");
        hashIsGrounded = Animator.StringToHash("IsGrounded");
    }

    void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = transform;
            Debug.LogWarning("PlayerController: respawnPoint가 설정되지 않아 현재 위치를 respawn으로 사용합니다.");
        }

        // 시작할 때 땅에 있다고 가정
        if (animator != null)
        {
            animator.SetBool(hashIsGrounded, isGrounded);
            animator.SetFloat(hashSpeed, 0f);
        }
    }

    public void Respawn()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        isGrounded = true;

        if (animator != null)
        {
            animator.SetBool(hashIsGrounded, true);
            animator.SetFloat(hashSpeed, 0f);
        }
    }

    void Update()
    {
        if (!canMove)
        {
            // 멈췄을 때 애니메이션도 Idle 유지
            if (animator != null)
            {
                animator.SetFloat(hashSpeed, 0f, animDampTime, Time.deltaTime);
                animator.SetBool(hashIsGrounded, isGrounded);
            }
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 카메라 기준 앞/오른쪽 벡터 (3인칭 시점 이동)
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 수평 이동 속도 적용 (y는 기존 중력/점프 유지)
        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        rb.velocity = velocity;

        // 방향 입력이 있을 때만 회전
        if (moveDir.sqrMagnitude > 0.001f)
        {
            float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float currentYaw = transform.eulerAngles.y;

            float newYaw = Mathf.SmoothDampAngle(
                currentYaw,
                targetYaw,
                ref rotationVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }

        // ===== 애니메이션 파라미터 업데이트 =====
        if (animator != null)
        {
            // 수평 속도(걷는 정도)만 계산해서 Speed로 보냄
            Vector3 horizontalVel = rb.velocity;
            horizontalVel.y = 0f;

            float speed = horizontalVel.magnitude;     // 0 ~ moveSpeed
            float speed01 = speed / moveSpeed;         // 0 ~ 1 로 정규화

            animator.SetFloat(hashSpeed, speed01, animDampTime, Time.deltaTime);
            animator.SetBool(hashIsGrounded, isGrounded);
        }

        // 점프 입력
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (audioSource != null && jumpClip != null)
        {
            audioSource.PlayOneShot(jumpClip);
        }

        isGrounded = false;

        if (animator != null)
            animator.SetBool(hashIsGrounded, false);  // 공중 상태로 전환 → Jump 상태로 넘어가게
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 바닥이든 장애물이든, 뭔가에 닿으면 일단 "땅"이라고 간주 (지금 구조 기준)
        if (collision.contacts.Length > 0)
        {
            isGrounded = true;

            if (animator != null)
                animator.SetBool(hashIsGrounded, true);
        }
    }
}
