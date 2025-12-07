using System.Collections;
using UnityEngine;
using UnityEngine.Playables;   // Timeline
using TMPro;                  // Countdown UI

public class IntroSequenceController : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector introDirector;   // Timeline 오브젝트

    [Header("Gameplay Objects")]
    public GameObject player;                // Player 오브젝트
    public PlayerController playerController; // PlayerController 스크립트
    public Camera mainCamera;                // 3인칭 카메라
    public MonoBehaviour cameraController;   // CameraController 스크립트

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;    // 3,2,1, START!! 텍스트
    public float interval = 1f;              // 숫자 간 간격(초)

    [Header("Audio (선택 사항, 나중에 써도 됨)")]
    public AudioSource sfxSource;            // 비프/스타트 효과음용
    public AudioSource bgmSource;            // 게임 BGM
    public AudioClip beepClip;               // 3,2,1
    public AudioClip startClip;              // START!!

    private void Start()
    {
        // 1) 인트로 시작 전: 게임플레이/카메라 비활성화
        if (player != null) player.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;  // 스크립트 자체 비활성화
            playerController.canMove = false;  // 혹시 모르니 플래그도 꺼두기
        }

        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (cameraController != null) cameraController.enabled = false;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // 2) Timeline 재생 시작
        if (introDirector != null)
        {
            introDirector.stopped += OnIntroFinished;
            introDirector.Play();
        }
        else
        {
            // Timeline이 없으면 바로 카운트다운 후 시작
            StartCoroutine(StartSequenceWithoutIntro());
        }
    }

    private void OnDestroy()
    {
        if (introDirector != null)
        {
            introDirector.stopped -= OnIntroFinished;
        }
    }

    /// <summary>
    /// 타임라인이 끝났을 때 호출
    /// </summary>
    private void OnIntroFinished(PlayableDirector director)
    {
        StartCoroutine(StartSequenceAfterIntro());
    }

    /// <summary>
    /// 타임라인이 있는 경우: 인트로 끝 → 카운트다운 → 게임 시작
    /// </summary>
    private IEnumerator StartSequenceAfterIntro()
    {
        // 인트로용 Timeline 오브젝트 비활성화 (타임라인 카메라 안 보이게)
        if (introDirector != null && introDirector.gameObject != null)
        {
            introDirector.gameObject.SetActive(false);
        }

        // 3인칭 카메라/플레이어만 먼저 켜두고,
        // 조작은 아직 막아둔 상태에서 카운트다운 시작
        ActivateGameplayView();

        // 카운트다운 + START 처리
        yield return StartCoroutine(CountdownRoutine());

        // 카운트다운 후 조작 허용 + BGM 재생
        EnablePlayerControlAndBgm();
    }

    /// <summary>
    /// 타임라인이 없는 경우에도 재사용 가능
    /// </summary>
    private IEnumerator StartSequenceWithoutIntro()
    {
        ActivateGameplayView();
        yield return StartCoroutine(CountdownRoutine());
        EnablePlayerControlAndBgm();
    }

    /// <summary>
    /// 3인칭 시점으로 전환 (플레이어/카메라 보이게)
    /// </summary>
    private void ActivateGameplayView()
    {
        if (player != null) player.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = true;   // 스크립트 켜기
            playerController.canMove = false;  // 아직 조작은 막기
        }

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (cameraController != null) cameraController.enabled = true;
    }

    /// <summary>
    /// 카운트다운 3,2,1, START!!
    /// </summary>
    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        yield return ShowStep("3", false);
        yield return ShowStep("2", false);
        yield return ShowStep("1", false);
        yield return ShowStep("START!!", true);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private IEnumerator ShowStep(string text, bool isStart)
    {
        if (countdownText != null)
            countdownText.text = text;

        // 효과음 (나중에 넣어도 됨)
        if (sfxSource != null)
        {
            if (isStart && startClip != null)
            {
                sfxSource.PlayOneShot(startClip);
            }
            else if (!isStart && beepClip != null)
            {
                sfxSource.PlayOneShot(beepClip);
            }
        }

        yield return new WaitForSeconds(interval);
    }

    /// <summary>
    /// 카운트다운 후 실제로 플레이 조작 허용 + BGM 재생
    /// </summary>
    private void EnablePlayerControlAndBgm()
    {
        if (playerController != null)
        {
            playerController.canMove = true;
        }

        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }
}
