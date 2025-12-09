using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class IntroSequenceController : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector introDirector;   // Timeline이 달린 오브젝트 (Timeline)

    [Header("Cameras")]
    public Camera introCamera;              // 타임라인에서 쓰는 카메라
    public Camera mainCamera;               // 3인칭 게임 플레이 카메라
    public MonoBehaviour cameraController;  // mainCamera에 붙은 CameraController

    [Header("Gameplay Objects")]
    public GameObject player;               // Player 오브젝트 (루트)
    public MonoBehaviour playerController;  // PlayerController 스크립트

    [Header("BGM")]
    public AudioSource bgmSource;           // BGM 용 AudioSource (BGM_Audio)
    public AudioClip introBgm;              // 타임라인 재생 중 나올 음악
    public AudioClip mainBgm;               // 게임 플레이용 메인 BGM

    [Header("Countdown UI & SFX")]
    public TMP_Text countdownText;
    public AudioSource sfxSource;           // 카운트다운 SFX용 AudioSource
    public AudioClip count3Clip;            // "3"
    public AudioClip count2Clip;            // "2"
    public AudioClip count1Clip;            // "1"
    public AudioClip startClip;             // "START!!"

    [Tooltip("카운트다운 한 단계당 시간(초)")]
    public float countdownInterval = 1f;

    private bool countdownStarted = false;

    // ===== 이벤트 등록/해제 =====
    private void OnEnable()
    {
        if (introDirector != null)
        {
            introDirector.stopped += OnIntroFinished;
        }
    }

    private void OnDisable()
    {
        if (introDirector != null)
        {
            introDirector.stopped -= OnIntroFinished;
        }
    }

    private void Start()
    {
        // 1) 인트로 시작 전: 게임플레이 비활성화
        if (player != null) player.SetActive(false);
        if (playerController != null) playerController.enabled = false;

        // 카메라 설정: 인트로 카메라만 활성화, 메인 카메라는 끄기
        if (introCamera != null) introCamera.gameObject.SetActive(true);
        if (cameraController != null) cameraController.enabled = false;

        // 카운트다운 텍스트는 처음엔 숨김
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // 2) 인트로 BGM 재생 (루프)
        if (bgmSource != null && introBgm != null)
        {
            bgmSource.Stop();
            bgmSource.clip = introBgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // 3) Timeline 재생 시작
        if (introDirector != null)
        {
            introDirector.Play();
        }
        else
        {
            // 타임라인이 없으면 바로 게임 시작
            StartGameplay();
        }
    }

    // ===== 타임라인이 끝났을 때 호출 =====
    private void OnIntroFinished(PlayableDirector director)
    {
        // 이 오브젝트가 이미 비활성/파괴된 상태라면 그냥 무시
        if (!isActiveAndEnabled) return;
        if (countdownStarted) return;

        countdownStarted = true;

        // 🔹 먼저 3인칭 카메라 시점으로 전환
        if (introCamera != null)
            introCamera.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = true;

        // 플레이어 모델 보이게
        if (player != null)
            player.SetActive(true);

        // 인트로 BGM 정지 (카운트다운 구간에서는 SFX만)
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        // 🔹 카운트다운 코루틴 시작
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        yield return ShowStep("3", count3Clip);
        yield return ShowStep("2", count2Clip);
        yield return ShowStep("1", count1Clip);
        yield return ShowStep("START!!", startClip);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // 카운트다운이 끝나면 실제 게임 시작
        StartGameplay();
    }

    private IEnumerator ShowStep(string text, AudioClip clip)
    {
        if (countdownText != null)
            countdownText.text = text;

        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);

        yield return new WaitForSeconds(countdownInterval);
    }

    private void StartGameplay()
    {
        // 플레이어 조작 활성화
        if (playerController != null)
            playerController.enabled = true;

        // 인트로용 타임라인 오브젝트는 끄기만 (Destroy ❌)
        if (introDirector != null && introDirector.gameObject != null)
        {
            introDirector.gameObject.SetActive(false);
        }

        // 메인 BGM으로 전환
        if (bgmSource != null && mainBgm != null)
        {
            bgmSource.Stop();
            bgmSource.clip = mainBgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
}
