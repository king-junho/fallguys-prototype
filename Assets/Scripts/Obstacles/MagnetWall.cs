using UnityEngine;

public class MagnetWall : MonoBehaviour
{
    [Header("이동 Z 위치 설정 (local 기준)")]
    [Tooltip("완전히 열린 상태일 때 Z(local) 값")]
    public float openLocalZ = -9f;     // 시작 z

    [Tooltip("완전히 닫힌 상태(벽에 붙는 지점) Z(local) 값")]
    public float closedLocalZ = -1f;   // 부딪히는 z

    [Header("속도 / 대기 시간")]
    public float closeSpeed = 4f;      // 닫힐 때 속도 (빠르게)
    public float openSpeed = 1.5f;     // 열릴 때 속도 (천천히)
    public float waitOpenTime = 0.2f;  // 완전 열렸을 때 잠깐 대기
    public float waitClosedTime = 0.2f; // 완전 닫혔을 때 잠깐 대기

    private Vector3 _openLocalPos;
    private Vector3 _closedLocalPos;

    private enum State { Opening, WaitingOpen, Closing, WaitingClosed }
    private State _state;
    private float _waitTimer;

    private void Start()
    {
        // 시작할 때 X,Y는 현재 값 유지하고 Z만 openLocalZ, closedLocalZ로 맞춰줌
        Vector3 start = transform.localPosition;

        _openLocalPos = new Vector3(start.x, start.y, openLocalZ);
        _closedLocalPos = new Vector3(start.x, start.y, closedLocalZ);

        // 씬에서 이미 -9에 있다면, 시작 상태를 "닫히는 중"으로
        transform.localPosition = _openLocalPos;
        _state = State.Closing;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Opening:
                MoveTowards(_openLocalPos, openSpeed, State.WaitingOpen);
                break;

            case State.Closing:
                MoveTowards(_closedLocalPos, closeSpeed, State.WaitingClosed);
                break;

            case State.WaitingOpen:
                HandleWait(waitOpenTime, State.Closing);
                break;

            case State.WaitingClosed:
                HandleWait(waitClosedTime, State.Opening);
                break;
        }
    }

    private void MoveTowards(Vector3 target, float speed, State nextState)
    {
        transform.localPosition =
            Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, target) < 0.001f)
        {
            _state = nextState;
            _waitTimer = 0f;
        }
    }

    private void HandleWait(float waitTime, State nextState)
    {
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= waitTime)
        {
            _state = nextState;
        }
    }
}
