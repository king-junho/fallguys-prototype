using UnityEngine;

public class PlayerFallZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>()
                     ?? other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.Respawn();
        }
    }
}
