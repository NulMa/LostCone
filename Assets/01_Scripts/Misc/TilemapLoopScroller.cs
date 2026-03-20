using UnityEngine;

/// <summary>
/// Grid 또는 Tilemap GameObject에 부착.
/// loopWidth만큼 이동하면 시작 위치로 되돌아가 무한 반복처럼 보이게 함.
/// </summary>
public class TilemapLoopScroller : MonoBehaviour
{
    [Tooltip("초당 이동 속도 (월드 유닛/초)")]
    [SerializeField] private float scrollSpeed = 2f;

    [Tooltip("반복 구간 너비 (타일맵 패턴 1사이클의 월드 유닛 길이)")]
    [SerializeField] private float loopWidth = 20f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= startPosition.x - loopWidth)
        {
            transform.position = startPosition;
        }
    }
}
