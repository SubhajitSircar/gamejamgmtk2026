using UnityEngine;

public class RagdollDragTest : MonoBehaviour
{
    private Camera mainCamera;
    private TargetJoint2D targetJoint;

    void Start()
    {
        // Cache the main camera for performance
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for mouse click or touch input
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        else if (Input.GetMouseButton(0) && targetJoint != null)
        {
            ContinueDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void TryStartDrag()
    {
        // Cast a ray from the camera to the mouse position
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        // Check if we hit a collider belonging to our ragdoll parts
        if (hit.collider != null && hit.rigidbody != null)
        {
            // Attach a TargetJoint2D dynamically to the clicked body part
            targetJoint = hit.rigidbody.gameObject.AddComponent<TargetJoint2D>();
            
            // Configure the joint for snappy, springy movement
            targetJoint.maxForce = 1000f; // High force to lift the ragdoll weight
            targetJoint.frequency = 5f;    // Controls the spring stiffness
            targetJoint.dampingRatio = 1f; // Prevents wild oscillations
            
            // Set the target position to where the mouse is
            targetJoint.target = mouseWorldPos;
        }
    }

    void ContinueDrag()
    {
        // Keep updating the joint's destination to follow the mouse cursor
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetJoint.target = mouseWorldPos;
    }

    void EndDrag()
    {
        if (targetJoint != null)
        {
            // Destroy the joint to release the body part so it falls naturally
            Destroy(targetJoint);
            targetJoint = null;
        }
    }
}
