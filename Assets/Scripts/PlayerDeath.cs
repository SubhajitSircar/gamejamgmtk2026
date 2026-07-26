using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Body Parts")]
    public Transform head;
    public Transform body;
    public Transform armPivot;

    public GlobalScript.Player playing_against;

    private bool isDead = false;

    // Called by the bullet upon impact
    public void TriggerDeath(float bulletVelocityX)
    {
        if (isDead) return;
        isDead = true;

        // If we are in a level with a BlackoutManager, tell it to flash the lights!
        if (BlackoutManager.instance != null)
        {
            BlackoutManager.instance.RestoreLights();
        }

        // 1. Disable player inputs so they can't keep shooting
        ArmController armCtrl = GetComponentInChildren<ArmController>();
        if (armCtrl != null) armCtrl.enabled = false;

        GunShoot gunCtrl = GetComponentInChildren<GunShoot>();
        if (gunCtrl != null) gunCtrl.enabled = false;

        // Force the arm to instantly drop limp
        if (armPivot != null)
            armPivot.localRotation = Quaternion.Euler(0, 0, -90f);

        // 2. Determine fall direction based on the bullet's travel direction
        // If bullet moves right (positive), player falls backward to the right (- rotation)
        float fallDirection = bulletVelocityX > 0 ? -1f : 1f;

        // 3. Sever the head and body from the parent so they move independently
        head.parent = null;
        body.parent = null;

        // 3.1 Activate gravity in the head for a valid fall
        if (head.GetComponent<Rigidbody2D>() != null)
        {
            head.GetComponent<Rigidbody2D>().gravityScale = 1;
        }

        // 4. Start the grotesque animations
        StartCoroutine(DeathSequence(fallDirection));
    }

    IEnumerator DeathSequence(float fallDirection)
    {
        GlobalScript g = GlobalScript.Instance;

        // 1. SAFETY CHECK: Only record the score if the GlobalScript actually exists!
        if (g != null)
        {
            g.RecordRoundWinner(playing_against);
        }
        else
        {
            Debug.LogWarning("GlobalScript is missing! (You are probably testing a single level). Skipping score tracking.");
        }

        // 2. Start both animations
        StartCoroutine(AnimateHead(fallDirection));
        StartCoroutine(AnimateBody(fallDirection));

        // Wait until the longest animation finishes
        yield return new WaitForSeconds(2f);

        // 3. SAFETY CHECK: Only try to load the next scene if GlobalScript exists
        if (g != null)
        {
            SceneManager.LoadScene(g.GetNextRoundScene());
        }
        else
        {
            Debug.LogWarning("GlobalScript is missing! Cannot load the next scene automatically.");
        }
    }

    IEnumerator AnimateHead(float dir)
    {
        Vector3 startPos = head.position;
        // Target: Fly backward and drop down to the floor level
        Vector3 endPos = startPos + new Vector3(dir * 2.5f, -2.5f, 0);

        float duration = 1.5f; // Slow, dramatic rolling time
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime; // This respects the KillCam's time-freeze!
            float t = elapsed / duration;

            // Creates a mathematical parabola so the head pops up before falling
            float height = Mathf.Sin(t * Mathf.PI) * 1.5f;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            currentPos.y += height;

            head.position = currentPos;

            // Slowly roll the head backward through the air
            head.Rotate(0, 0, (300f * dir) * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator AnimateBody(float dir)
    {
        Vector3 startPos = body.position;
        // The body sinks slightly to simulate the knees buckling
        Vector3 endPos = startPos + new Vector3(dir * 0.5f, -1.2f, 0);

        Quaternion startRot = body.rotation;
        // Rotate 90 degrees to fall completely flat on its back
        Quaternion endRot = Quaternion.Euler(0, 0, -90f * dir);

        float duration = 2f; // Body takes longer to fall than the head
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smoothstep formula gives a heavy, accelerating feel to the fall
            float smoothT = t * t * (3f - 2f * t);

            body.position = Vector3.Lerp(startPos, endPos, smoothT);
            body.rotation = Quaternion.Lerp(startRot, endRot, smoothT);

            yield return null;
        }
    }
}