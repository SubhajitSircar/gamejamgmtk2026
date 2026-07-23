using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class RagdollArmLook : MonoBehaviour
{
    [Header("Motor Settings")]
    [SerializeField] private float motorSpeed = 500f;
    [SerializeField] private float maxMotorTorque = 1000f;

    private HingeJoint2D hingeJoint;
    private Rigidbody2D rb;
    private Camera mainCamera;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Ensure the joint is configured to use the motor system
        hingeJoint.useMotor = true;
        
        JointMotor2D motor = hingeJoint.motor;
        motor.maxMotorTorque = maxMotorTorque;
        hingeJoint.motor = motor;
    }

    void FixedUpdate()
    {
        // 1. Get the direction from the arm's pivot to the mouse position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        // 2. Calculate the target angle the arm wants to reach
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust target angle relative to the Torso's rotation 
        // (Hinge Joint limits operate relative to the parent connected body)
        if (hingeJoint.connectedBody != null)
        {
            targetAngle -= hingeJoint.connectedBody.rotation;
        }

        // 3. Normalize the angles between -180 and 180 degrees
        targetAngle = Mathf.DeltaAngle(0, targetAngle);
        float currentAngle = hingeJoint.jointAngle;

        // 4. Calculate the shortest directional step to reach the target angle
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // 5. Apply motor velocity based on how far away the target is
        JointMotor2D motor = hingeJoint.motor;
        
        // If the angle difference is small, slow down to prevent jittering
        if (Mathf.Abs(angleDifference) > 1f)
        {
            // Set direction: positive speed rotates counter-clockwise, negative clockwise
            motor.motorSpeed = Mathf.Sign(angleDifference) * motorSpeed;
        }
        else
        {
            motor.motorSpeed = 0f;
        }

        hingeJoint.motor = motor;
    }
}
