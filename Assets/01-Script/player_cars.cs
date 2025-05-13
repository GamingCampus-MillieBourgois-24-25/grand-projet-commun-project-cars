using UnityEngine;

public class PlayerCars : MonoBehaviour
{
    
    [SerializeField] private Rigidbody rb;
    
    // Car parameters
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float extraSteering = 2f;

// Input system
    private float turnInput;
    private bool isGrounded;
    private IA_Steering steeringAction;

    void Awake()
    {
        steeringAction = new IA_Steering();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        steeringAction.Enable();
    }
    
    private void OnDisable()
    {
        steeringAction.Disable();
    }

    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        Move();
    }
    
    private void ProcessInput()
    {
        Vector2 steeringInput = steeringAction.Steering.Steering.ReadValue<Vector2>();
        turnInput = steeringInput.x;
    }
    
    private void Move()
    {
        float currentSpeed = rb.velocity.magnitude;
        float accelerationFactor = Mathf.Exp(-currentSpeed / maxSpeed);
        float adjustedSpeed = speed * accelerationFactor;
        
        // Apply left-right movement based on input
        rb.AddForce(new Vector3(turnInput * speed * extraSteering, 0, adjustedSpeed), ForceMode.Acceleration);
    }

}