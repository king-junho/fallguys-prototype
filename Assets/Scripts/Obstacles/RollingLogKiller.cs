using UnityEngine;

public class RollingLogKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 🔥 자식에 Collider 있고, 부모에 RollingLog가 붙어 있으므로
        // 부모까지 포함해서 찾아야 한다
        RollingLog log = other.GetComponentInParent<RollingLog>();
        if (log != null)
        {
            Destroy(log.gameObject);
        }
    }
}
