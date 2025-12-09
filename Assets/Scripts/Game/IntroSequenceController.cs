using System.Collections;
using UnityEngine;
using UnityEngine.Playables;   // Timeline 제어
using TMPro;

public class IntroSequenceController : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector introDirector;   // Timeline이 달린 오브젝트 (Timeline)

    [Header("Timeline Camera")]
    public Camera introCamera;              // 🔹 타임라인용 카메라

    [Header("Gameplay Objects")]
    public GameObject player;               // Player 오브젝트
    public MonoBehaviour playerController;  // PlayerController 스크립트
    public Camera mainCamera;               // Main Camera (3인칭 카메라)
    public MonoBehaviour cameraController;  // CameraController 스크립트

    [Header("BGM")]
    public AudioSource bgmSource;           // BGM 용 AudioSource (BGM_Audio)
    public AudioClip introBgm;              // 타임라인 재생 중 나올 음악
    public AudioClip mainBgm;               // 게임 플레이용 메인 BGM

    [Header("Countdown UI & SFX")]
    public TMP_Text countdownText;
    public AudioSource sfxSource;           // 카운트다운 SFX용 AudioSource
    public AudioClip count3Clip;            // "3" 소리
    public AudioClip count2Clip;            // "2" 소리
    public AudioClip count1Clip;            // "1" 소리
    public AudioClip startClip;             // "START!!" 소리

    [Tooltip("카운트다운 한 단계당 시간(초)")]
    public float countdownInterval = 1f;

    private void Start()
    {
        // 1) 인트로 시작 전: 게임플레이 비활성화
        if (player != null) player.SetActive(false);
        if (playerController != null) playerController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        // 카운트다운 텍스트는 처음엔 숨겨두기
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // 2) 인트로 BGM 재생 (타임라인 동안 루프)
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
            introDirector.stopped += OnIntroFinished;
            introDirector.Play();
        }
        else
        {
            StartGameplay();
        }
    }

    private void OnIntroFinished(PlayableDirector director)
    {
        // 🔹 1) 타임라인용 카메라/오브젝트 끄기
        if (introCamera != null)
            introCamera.gameObject.SetActive(false);

        if (introDirector != null && introDirector.gameObject != null)
            introDirector.gameObject.SetActive(false);

        // 🔹 2) 3인칭 카메라 시점으로 전환
        if (player != null)
            player.SetActive(true);
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);
        if (cameraController != null)
            cameraController.enabled = true;

        // 🔹 3) 인트로 BGM 종료
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        // 🔹 4) 이제 3인칭 관점에서 카운트다운 시작
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
        if (playerController != null)
            playerController.enabled = true;

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
