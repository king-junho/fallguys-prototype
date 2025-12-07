using UnityEngine;

public class SpinningSawPlate : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 180f;    // 초당 회전 속도 (필요하면 인스펙터에서 조절)

    [Header("Knockback")]
    public float knockbackForce = 12f;  // XZ 평면 넉백 힘
    public float upwardForce = 2f;    // 살짝 위로 튀기는 힘

    private void Update()
    {
        // Y축 기준으로 계속 회전 (카트라이더 게이트처럼)
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryKnockback(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 콜라이더를 Trigger로 쓰고 싶을 때도 대응
        TryKnockback(other);
    }

    private void TryKnockback(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // 톱날판 중심에서 플레이어 쪽으로 향하는 방향 계산
        Vector3 dir = other.transform.position - transform.position;

        // 위아래 방향은 0으로 만들어서 "수평 넉백" 위주로
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = transform.forward; // 혹시 너무 가까우면 그냥 앞 방향으로

        dir.Normalize();

        // 수평 넉백 + 약간 위로
        Vector3 force = dir * knockbackForce + Vector3.up * upwardForce;

        // 기존 속도를 잠깐 지우고
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // VelocityChange로 딱 튕겨 나가는 느낌
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
