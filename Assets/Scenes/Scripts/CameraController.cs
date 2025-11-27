using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 따라갈 플레이어
    public Vector3 offset = new Vector3(0f, 8f, -10f); // 플레이어 기준 뒤쪽

    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // 🔥 offset을 플레이어의 로컬 방향으로 변환
        Vector3 offsetWorld = target.TransformDirection(offset);

        // 플레이어 위치 + (플레이어 기준 뒤쪽/위쪽)
        Vector3 desiredPosition = target.position + offsetWorld;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        transform.position = smoothedPosition;

        // 플레이어 약간 위쪽을 바라보도록
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }
}
