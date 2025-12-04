using UnityEngine;

public class SwingHammer : MonoBehaviour
{
    [Header("Swing Settings")]
    public float maxAngle = 45f;    // 양 끝 각도 (+/-)
    public float swingSpeed = 1.5f; // 왕복 속도

    [Header("Knockback Settings")]
    public float horizontalStrength = 25f; // XZ 방향 힘
    public float verticalStrength = 3f;    // 위로 튀기는 힘

    private Quaternion _startLocalRotation;

    private void Start()
    {
        // 시작 회전값 저장 (기본 각도 기준으로 좌우 움직이게)
        _startLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        // -maxAngle ~ +maxAngle 사이 왕복
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;

        // Z축 기준 회전 (추처럼)
        transform.localRotation = _startLocalRotation * Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody playerRb = collision.rigidbody;
        if (playerRb == null) return;

        // Hammer 중심 → 플레이어 방향 (수평)
        Vector3 dir = collision.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f)
        {
            // 혹시 거의 같은 위치라면 망치가 향한 쪽으로
            dir = transform.right;
        }
        dir.Normalize();

        // 기존 속도 리셋
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // 수평 + 수직 넉백 분리
        Vector3 knockback = dir * horizontalStrength;     // XZ로 멀리 밀어내기
        knockback += Vector3.up * verticalStrength;       // 살짝 위로만

        playerRb.AddForce(knockback, ForceMode.VelocityChange);
    }

}
