using UnityEngine;

public class RotatingBlade : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 120f;    // 초당 회전 속도 (도)

    [Header("Knockback")]
    public float knockbackForce = 10f;    // 수평으로 미는 힘
    public float knockupForce = 4f;       // 위로 튀기는 힘

    private void Update()
    {
        // Y축 기준으로 계속 회전 (360도 반복)
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Player 태그를 가진 오브젝트만 처리
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        // 회전 중심(기둥) 기준으로 수평 방향 계산
        Vector3 dir = collision.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
        {
            // 거의 같은 위치면 대충 앞으로 밀기
            dir = transform.forward;
            dir.y = 0f;
        }

        dir.Normalize();

        // 🔥 기존 속도를 잠깐 눌러버리고, 강제로 수평 속도 세팅
        float horizontalSpeed = knockbackForce;  // 숫자는 감으로 조절 (10~15 정도)
        float verticalSpeed = 0.5f;            // 살짝만 띄우거나 0으로 해도 됨

        Vector3 newVelocity = dir * horizontalSpeed;
        newVelocity.y = Mathf.Max(rb.velocity.y, verticalSpeed); // 위로는 살짝만

        rb.velocity = newVelocity;
    }

}
