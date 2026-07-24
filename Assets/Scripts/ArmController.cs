using UnityEngine;

public class ArmController : MonoBehaviour
{
    public KeyCode raiseKey = KeyCode.W;
    public float raiseSpeed = 180f;
    public float fallSpeed = 120f;

    private float angle = -90f;

    void Update()
    {
        // Logic gate: Only allow raising if the duel has officially started
        if (GameManager.instance != null && GameManager.instance.isDuelActive)
        {
            if (Input.GetKey(raiseKey))
                angle += raiseSpeed * Time.deltaTime;
            else
                angle -= fallSpeed * Time.deltaTime;
        }
        else
        {
            // Force arm down during the countdown
            angle -= fallSpeed * Time.deltaTime;
        }

        angle = Mathf.Clamp(angle, -90f, 90f);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}