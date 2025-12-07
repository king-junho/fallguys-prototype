using System.Collections.Generic;
using UnityEngine;

public class SpikedBallSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject spikedBallPrefab;   // 프리팹
    public Transform spawnPoint;          // 떨어질 위치 (없으면 자기 transform)
    public float spawnInterval = 2f;      // 몇 초마다 하나씩
    public int maxBalls = 3;              // 동시에 존재할 수 있는 최대 개수

    [Header("쇠공 생존 시간(킬존이 따로 있으면 0으로 둬도 됨)")]
    public float ballLifeTime = 10f;

    private float _timer;
    private readonly List<GameObject> _activeBalls = new List<GameObject>();

    private void Update()
    {
        // 리스트에서 이미 Destroy된 애들 정리
        _activeBalls.RemoveAll(ball => ball == null);

        // 아직 최대 개수 안 찼으면 타이머 증가
        if (_activeBalls.Count < maxBalls)
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                SpawnBall();
                _timer = 0f;
            }
        }
    }

    private void SpawnBall()
    {
        if (spikedBallPrefab == null)
        {
            Debug.LogWarning("SpikedBallSpawner: spikedBallPrefab이 비어 있습니다.");
            return;
        }

        Transform point = spawnPoint != null ? spawnPoint : transform;
        GameObject ball = Instantiate(spikedBallPrefab, point.position, point.rotation);
        _activeBalls.Add(ball);

        if (ballLifeTime > 0f)
        {
            Destroy(ball, ballLifeTime);
        }
    }
}
