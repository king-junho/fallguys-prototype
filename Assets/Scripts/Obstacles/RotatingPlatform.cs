using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("회전 설정")]
    [Tooltip("1초에 몇 도 회전할지 (양수면 한 방향, 음수면 반대 방향)")]
    public float rotationSpeed = 90f; // 90도/초 → 4초에 한 바퀴

    private void Update()
    {
        // X축 기준으로 계속 회전 (자기 로컬 X축)
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
