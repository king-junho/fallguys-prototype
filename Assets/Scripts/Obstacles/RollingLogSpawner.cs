using System.Collections.Generic;
using UnityEngine;

public class RollingLogSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject logPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;   // 몇 초마다 생성할지
    public int maxLogs = 3;            // 동시에 존재 가능한 최대 개수

    private float _timer;
    private readonly List<GameObject> _logs = new List<GameObject>();

    private void Update()
    {
        _timer += Time.deltaTime;

        // 이미 파괴된 로그는 리스트에서 제거
        _logs.RemoveAll(log => log == null);

        if (_timer >= spawnInterval && _logs.Count < maxLogs)
        {
            SpawnLog();
            _timer = 0f;
        }
    }

    private void SpawnLog()
    {
        if (logPrefab == null)
        {
            Debug.LogWarning("RollingLogSpawner: logPrefab이 설정되지 않았습니다.");
            return;
        }

        GameObject log = Instantiate(logPrefab, transform.position, transform.rotation);
        _logs.Add(log);
    }
}
