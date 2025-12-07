using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    [Header("Game Clear UI")]
    public GameObject gameClearPanel;

    private bool _cleared = false;

    private void Start()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(false); // 시작할 땐 숨김
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_cleared) return;
        if (!other.CompareTag("Player")) return;

        _cleared = true;

        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }

        // 게임 일시정지 (물리/시간만 멈추고 Update는 계속 돈다)
        Time.timeScale = 0f;

        Debug.Log("Game Clear! Press R to restart.");
    }

    private void Update()
    {
        // 클리어된 상태에서만 R 키 받기
        if (_cleared && Input.GetKeyDown(KeyCode.R))
        {
            // 다시 시간 흐르게 하고 씬 리로드
            Time.timeScale = 1f;
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }
    }
}
