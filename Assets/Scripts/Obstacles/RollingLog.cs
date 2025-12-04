using UnityEngine;

public class RollingLog : MonoBehaviour
{
    [Header("Movement")]
    public float rollForce = 5f;      // 앞으로 굴리는 힘
    public Vector3 moveDirection = Vector3.right; // X+ 방향이 "앞"

    [Header("Knockback")]
    public float knockbackSpeed = 15f;
    public float minVerticalSpeed = 0.5f;

    private Rigidbody _rb;
    private Vector3 _moveDir;
    private bool _hasLanded = false;  // 바닥에 닿았는지 여부

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning("RollingLog: Rigidbody가 필요합니다.");
        }

        // X+ 방향이 '앞'
        moveDirection.y = 0f;
        _moveDir = moveDirection.normalized;
    }



    private void OnCollisionEnter(Collision collision)
    {
        // 🔥 무엇이든 한 번이라도 부딪히면 "착지했다"고 간주
        _hasLanded = true;

        // 플레이어 아니면 여기서 끝
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody playerRb = collision.rigidbody;
        if (playerRb == null) return;

        Vector3 dir = collision.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
        {
            dir = _moveDir; // 거의 같은 위치면 통나무 진행 방향으로
        }

        dir.Normalize();

        Vector3 newVel = dir * knockbackSpeed;
        newVel.y = Mathf.Max(playerRb.velocity.y, minVerticalSpeed);

        playerRb.velocity = newVel;
    }

    private void FixedUpdate()
    {
        if (_rb == null || !_hasLanded) return;   // 아직 땅에 안 닿았으면 그냥 자유낙하

        _rb.AddForce(_moveDir * rollForce, ForceMode.Acceleration);
    }

}
