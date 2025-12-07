using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. Rigidbody 기준 루트 오브젝트 찾기
        Rigidbody rb = other.attachedRigidbody;
        GameObject target = rb != null ? rb.gameObject : other.gameObject;

        // 2. 통나무 또는 쇠공만 삭제
        if (target.CompareTag("RollingLog") || target.CompareTag("SpikedBall"))
        {
            Destroy(target);
        }
    }
}
