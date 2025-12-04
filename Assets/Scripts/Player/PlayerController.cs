using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 이동 입력 (Horizontal: A/D, 좌우 화살표 / Vertical: W/S, 상하 화살표)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 카메라 기준으로 앞/옆 방향 잡기 (3인칭 느낌)
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        rb.velocity = velocity;

        // 방향 바라보기
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // 바닥 체크 (콜라이더가 바닥과 닿으면 다시 점프 가능)
    private void OnCollisionEnter(Collision collision)
    {
        // 나중에 Ground 태그 써도 되지만, 지금은 그냥 아무 바닥이나 닿으면 OK
        if (collision.contacts.Length > 0)
        {
            // 아래쪽에서 닿았을 때만 true로 하고 싶으면 더 정교하게 할 수도 있음
            isGrounded = true;
        }
    }
}
