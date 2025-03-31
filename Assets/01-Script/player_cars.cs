using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCars : MonoBehaviour
{
    
    [SerializeField] private Rigidbody rb;
    
    // Car parameters
    [SerializeField] private float speed = 10f;

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
        rb.AddForce(new Vector3(turnInput * speed, 0,0), ForceMode.Acceleration);
    }

}