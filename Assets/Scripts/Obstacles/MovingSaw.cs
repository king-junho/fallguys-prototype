using UnityEngine;

public class MovingSaw : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveDistance = 3f;   // 레일 중앙 기준 좌우 이동 거리
    public float moveSpeed = 1.5f;    // 좌우 이동 속도

    [Header("Rotate Settings")]
    public float rotateSpeed = 360f;  // 1초에 몇 도 회전할지

    [Header("Respawn")]
    public Transform respawnPoint;    // 플레이어가 돌아갈 위치(스타트 지점)

    private Vector3 _startLocalPos;

    private void Start()
    {
        // 레일 기준 시작 위치 저장 (자식 localPosition 사용)
        _startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.localPosition = _startLocalPos + new Vector3(0f, 0f, offset);

        //  자기 X축 기준으로 회전
        transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) PlayerController가 Respawn 메서드를 가지고 있다면 그걸 먼저 시도
        var controller = other.GetComponentInParent<PlayerController>();
        if (controller != null && respawnPoint == null)
        {
            // PlayerController 안에서 자체적으로 시작 위치를 관리하는 버전이면
            // Respawn() 만 호출해도 됨 (원래 스크립트에 따라 다름)
            // 여기선 예시로만 놔두고, respawnPoint를 쓰는 쪽을 기본으로 둘게.
        }

        // 2) respawnPoint로 순간 이동
        if (respawnPoint != null)
        {
            Rigidbody playerRb = other.attachedRigidbody;
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;  // 속도 리셋
                playerRb.angularVelocity = Vector3.zero;
            }

            Transform playerTr = other.transform;
            playerTr.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("MovingSaw: respawnPoint가 설정되어 있지 않습니다.");
        }
    }
}
