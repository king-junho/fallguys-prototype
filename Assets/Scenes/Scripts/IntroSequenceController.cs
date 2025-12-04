using UnityEngine;
using UnityEngine.Playables;  // ❗ Timeline 제어용

public class IntroSequenceController : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector introDirector;   // Timeline이 달린 오브젝트 (Timeline)

    [Header("Gameplay Objects")]
    public GameObject player;               // Player 오브젝트
    public MonoBehaviour playerController;  // PlayerController 스크립트
    public Camera mainCamera;               // Main Camera (3인칭 카메라)
    public MonoBehaviour cameraController;  // CameraController 스크립트

    private void Start()
    {
        // 1) 인트로 시작 전: 게임플레이 비활성화
        if (player != null) player.SetActive(false);
        if (playerController != null) playerController.enabled = false;

        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (cameraController != null) cameraController.enabled = false;

        // 2) Timeline 재생 시작
        if (introDirector != null)
        {
            // 끝났을 때 호출될 이벤트 등록
            introDirector.stopped += OnIntroFinished;

            introDirector.Play();
        }
        else
        {
            // Timeline이 없으면 바로 게임 시작
            StartGameplay();
        }
    }

    private void OnIntroFinished(PlayableDirector director)
    {
        StartGameplay();
    }

    private void StartGameplay()
    {
        // 플레이어/카메라 활성화
        if (player != null) player.SetActive(true);
        if (playerController != null) playerController.enabled = true;

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (cameraController != null) cameraController.enabled = true;

        // 인트로용 Timeline 오브젝트는 더 이상 필요 없으니 끄기
        if (introDirector != null && introDirector.gameObject != null)
        {
            introDirector.gameObject.SetActive(false);
        }
    }
}
