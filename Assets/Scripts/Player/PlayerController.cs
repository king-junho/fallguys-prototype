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

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 5f;

    public float rotationSmoothTime = 0.1f;  // 회전 부드러움 정도
    private float rotationVelocity;          // SmoothDamp 내부 상태용

    private Rigidbody rb;
    private bool isGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = transform;
            Debug.LogWarning("PlayerController: respawnPoint? ??? ?? ?? ?? ??? ?????.");
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
    }

    void Update()
    {

        if (!canMove) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ?????? ???????? ??/?? ???? ???? (3???? ????)
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        rb.velocity = velocity;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            // 목표 방향의 Y 각도(앞 방향)
            float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

            // 현재 플레이어의 Y 각도
            float currentYaw = transform.eulerAngles.y;

            // 부드럽게 각도 보간
            float newYaw = Mathf.SmoothDampAngle(
                currentYaw,
                targetYaw,
                ref rotationVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }

        // ????
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
    }

    // ???? ???? (?????????? ?????? ?????? ???? ???? ????)
    private void OnCollisionEnter(Collision collision)
    {
        // ?????? Ground ???? ???? ??????, ?????? ???? ???? ???????? ?????? OK
        if (collision.contacts.Length > 0)
        {
            // ?????????? ?????? ???? true?? ???? ?????? ?? ???????? ?? ???? ????
            isGrounded = true;
        }
    }
}
