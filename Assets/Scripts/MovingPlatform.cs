using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public static bool p1ReachedTop = false;

    public bool isPlayer1Platform = true;

    public float moveHeight = 2f;
    public float speed = 2f;

    private Vector3 bottomPos;
    private Vector3 topPos;

    private bool goingUp = true;

    void Start()
    {
        bottomPos = transform.position;              // Ground position
        topPos = bottomPos + Vector3.up * moveHeight;
    }

    void Update()
    {
        if (!GameManager.instance.isDuelActive)
            return;

        if (isPlayer1Platform)
            MovePlayer1Platform();
        else
            MovePlayer2Platform();
    }

    void MovePlayer1Platform()
    {
        if (goingUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                topPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, topPos) < 0.01f)
            {
                goingUp = false;
                p1ReachedTop = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                bottomPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, bottomPos) < 0.01f)
            {
                goingUp = true;
                p1ReachedTop = false;
            }
        }
    }

    void MovePlayer2Platform()
    {
        // Wait until P1 reaches the top
        if (!p1ReachedTop && goingUp)
            return;

        if (goingUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                topPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, topPos) < 0.01f)
            {
                goingUp = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                bottomPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, bottomPos) < 0.01f)
            {
                goingUp = true;
            }
        }
    }
}