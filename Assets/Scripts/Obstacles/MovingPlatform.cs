using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveDistance = 2f;   // 위아래로 얼마나 이동할지
    public float moveSpeed = 1f;      // 이동 속도
    public Vector3 moveDirection = Vector3.up; // 이동 방향 (기본: 위/아래)

    private Vector3 _startPos;

    private void Start()
    {
        // 시작 위치 기억
        _startPos = transform.position;
        // 방향은 안전하게 정규화
        moveDirection = moveDirection.normalized;
    }

    private void Update()
    {
        // 0~1 사이로 왔다 갔다 하는 값 (PingPong)
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);

        // 시작 지점 ~ 목표 지점 사이를 반복해서 왕복
        Vector3 targetOffset = moveDirection * moveDistance;
        transform.position = Vector3.Lerp(_startPos, _startPos + targetOffset, t);
    }
}
