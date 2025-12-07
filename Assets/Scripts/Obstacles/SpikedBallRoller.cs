using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpikedBallRoller : MonoBehaviour
{
    [Header("경사면 따라 굴리는 힘")]
    [Tooltip("경사 방향으로 얼마나 강하게 밀지 (값을 키울수록 더 힘차게 굴러감)")]
    public float rollForce = 60f;

    [Tooltip("경사로로 인식할 레이어 (Ground 등)")]
    public LayerMask groundLayer;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        // groundLayer에 포함된 오브젝트랑 맞닿아 있을 때만 처리
        if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0)
            return;

        if (collision.contactCount == 0) return;

        // 충돌 지점의 법선 벡터(경사면의 수직 방향)
        Vector3 normal = collision.GetContact(0).normal;

        // 중력을 경사면에 투영해서 "내리막 방향" 벡터 계산
        // Physics.gravity는 (0, -9.81, 0) 같은 값
        Vector3 slopeDir = Vector3.ProjectOnPlane(Physics.gravity, normal).normalized;

        // 그 방향으로 가속도 형태의 힘을 계속 가해줌
        _rb.AddForce(slopeDir * rollForce * Time.deltaTime, ForceMode.Acceleration);
    }
}
