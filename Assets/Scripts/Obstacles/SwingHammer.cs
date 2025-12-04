using UnityEngine;

public class SwingHammer : MonoBehaviour
{
    [Header("Swing Settings")]
    public float maxAngle = 45f;    // 양 끝 각도 (+/-)
    public float swingSpeed = 1.5f; // 왕복 속도

    [Tooltip("Hammer Rotation(단위: 라디안)")]
    public float phaseOffset = 0f;

    [Header("Knockback Settings")]
    public float horizontalStrength = 30f; // XZ 방향 힘


    private Quaternion _startLocalRotation;

    private void Start()
    {
        // 시작 회전값 저장 (기본 각도 기준으로 좌우 움직이게)
        _startLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        // 시간 * 속도 + 해머별 위상 오프셋
        float t = Time.time * swingSpeed + phaseOffset;

        // -maxAngle ~ +maxAngle 사이 왕복
        float angle = Mathf.Sin(t) * maxAngle;

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
            // 거의 같은 위치면, 망치가 휘두르는 오른쪽 방향으로
            dir = transform.right;
        }
        dir.Normalize();

        // 기존 속도 날리고
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // 👇 완전 수평 넉백만 준다 (y 성분 0 보장)
        Vector3 knockback = dir * horizontalStrength;
        knockback.y = 0f;

        playerRb.AddForce(knockback, ForceMode.VelocityChange);
    }

}
