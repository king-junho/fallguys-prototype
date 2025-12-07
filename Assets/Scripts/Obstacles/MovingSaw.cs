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
        // Player 태그 체크 대신, PlayerController 있는지로 체크하는 게 더 안전
        var player = other.GetComponent<PlayerController>()
                     ?? other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.Respawn();
        }
    }
}
