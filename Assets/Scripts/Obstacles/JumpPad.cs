using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 15f;          // 얼마나 높이 튕겨 올릴지
    public bool resetVerticalVelocity = true; // 기존 위/아래 속도 초기화 여부

    // 점프 방향(기본은 패드의 위쪽 방향)
    public Transform jumpDirection;        // 비워두면 이 오브젝트의 up 사용

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody playerRb = other.attachedRigidbody;
        if (playerRb == null) return;

        // 점프 방향 결정
        Vector3 dir = jumpDirection != null ? jumpDirection.up : transform.up;
        dir.Normalize();

        // 기존 수직 속도 삭제해서 항상 비슷한 점프 높이 유지
        if (resetVerticalVelocity)
        {
            Vector3 v = playerRb.velocity;
            v.y = 0f;
            playerRb.velocity = v;
        }

        // 순간적으로 위로 튀게 만들기
        playerRb.AddForce(dir * jumpForce, ForceMode.VelocityChange);
    }
}
