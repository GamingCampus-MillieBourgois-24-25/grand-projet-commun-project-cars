using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] private Transform target; // The object to follow (the sphere)
    [SerializeField] private float rotationSpeed = 10f; // How quickly to rotate
    [SerializeField] private float minMovementThreshold = 0.01f; // Minimum movement to rotate
    [SerializeField] private float verticalOffset = 1f; // Vertical offset to prevent ground collision

    private Rigidbody targetRigidbody;

    private void Start()
    {
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Follow position with vertical offset
        transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, target.position.z);

        // Orient based on the target's movement direction
        if (targetRigidbody != null && targetRigidbody.velocity.magnitude > minMovementThreshold)
        {
            // Create rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(targetRigidbody.velocity.normalized);

            // Apply rotation with smoothing
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}