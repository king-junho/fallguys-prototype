using UnityEngine;

public class SpikePlatform : MonoBehaviour
{
    [Header("Movement")]
    public float minY = -0.5f;        // 최저 높이
    public float maxY = 0.12f;        // 최고 높이
    public float cycleDuration = 2f;  // 올라갔다 내려오는 전체 시간(초)
    [Range(0f, 1f)]
    public float activeThreshold = 0.6f; // 이 값 이상일 때 '가시 튀어나온 상태'

    [Header("Knockback")]
    public float knockbackForce = 8f;  // 옆으로 밀어내는 힘
    public float knockupForce = 4f;    // 위로 튀기는 힘

    private Vector3 _startLocalPos;
    private float _timeOffset;
    private bool _isActive;

    private void Start()
    {
        // Spike의 시작 로컬 위치 저장
        _startLocalPos = transform.localPosition;

        // 여러 스파이크가 동시에 안 움직이게 약간 랜덤 오프셋
        _timeOffset = Random.Range(0f, cycleDuration);
    }

    private void Update()
    {
        // 0→1→0으로 왕복하는 값
        float t01 = Mathf.PingPong((Time.time + _timeOffset) / cycleDuration, 1f);

        // y를 -0.5 ~ 0.12 사이에서 왕복
        float y = Mathf.Lerp(minY, maxY, t01);

        Vector3 localPos = _startLocalPos;
        localPos.y = y;
        transform.localPosition = localPos;

        // 어느 정도 이상 올라왔을 때만 '활성 상태'
        _isActive = t01 >= activeThreshold;
    }

    // Spike 콜라이더는 IsTrigger = true 여야 함!
    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;                  // 가시가 내려가 있으면 무시
        if (!other.CompareTag("Player")) return; // Player 태그인지 확인

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // 수평 방향 계산
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
        {
            // 거의 같은 위치면 대충 앞으로 밀기
            dir = transform.forward;
        }

        dir.Normalize();

        // 옆으로 + 위로 튕겨내는 힘
        Vector3 force = dir * knockbackForce + Vector3.up * knockupForce;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
