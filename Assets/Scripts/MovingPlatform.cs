using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform")]
    public bool isPlayer1Platform = true;

    [Header("Movement")]
    public float moveHeight = 2f;
    public float speed = 2f;

    [Header("P2 Delay")]
    public float player2Delay = 1f;

    private Vector3 bottomPos;
    private Vector3 topPos;

    private bool goingUp = true;
    private float sceneStartTime;

    void Start()
    {
        // Starting position is the lowest point
        bottomPos = transform.position;

        // Platform can only move upward from its starting position
        topPos = bottomPos + Vector3.up * moveHeight;

        sceneStartTime = Time.time;
    }

    void Update()
    {
        // P1 starts immediately.
        // P2 waits before starting.
        if (!isPlayer1Platform &&
            Time.time < sceneStartTime + player2Delay)
        {
            return;
        }

        MovePlatform();
    }

    void MovePlatform()
    {
        Vector3 target = goingUp ? topPos : bottomPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // Reached top or bottom
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            goingUp = !goingUp;
        }
    }
}