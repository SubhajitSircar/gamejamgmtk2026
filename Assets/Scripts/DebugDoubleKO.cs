using UnityEngine;

public class DebugDoubleKO : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform player1Head;
    public Transform player2Head;

    void Update()
    {
        // Press 'T' on your keyboard to force the Double KO
        if (Input.GetKeyDown(KeyCode.T))
        {
            // LOGIC: We calculate a position exactly 2.5 units directly in front of each head.
            // This ensures the bullets spawn perfectly inside the detection range of the KillCam Raycast.

            // 1. Spawn a bullet moving RIGHT, aimed at Player 2's Head
            Vector3 p1BulletStart = player2Head.position + (Vector3.left * 2.5f);
            Instantiate(bulletPrefab, p1BulletStart, Quaternion.Euler(0, 0, 0));

            // 2. Spawn a bullet moving LEFT, aimed at Player 1's Head
            Vector3 p2BulletStart = player1Head.position + (Vector3.right * 2.5f);
            Instantiate(bulletPrefab, p2BulletStart, Quaternion.Euler(0, 0, 180f));
        }
    }
}