using UnityEngine;

public class SpikedBall : MonoBehaviour
{
    [Header("리스폰 설정")]
    [Tooltip("플레이어가 되돌아갈 시작 위치(스타트 라인 근처 Empty)")]
    public Transform respawnPoint;

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.collider.GetComponent<PlayerController>()
                 ?? collision.collider.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.Respawn();
            return;
        }
    }
}
