using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    [Header("Input Settings")] 
    private float moveInput = 0;
    private float steerInput = 0;
    private IA_Steering steeringAction;
    
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    
    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;
    
    private int[] wheelsIsGrounded = new int [4];
    private bool isGrounded = false;
    
    #region Unity Methods
    
    private void Awake()
    {
        steeringAction = new IA_Steering();
    }
    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
    }
    
    
    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }
    
    private void OnEnable()
    {
        steeringAction.Enable();
    }
    
    private void OnDisable()
    {
        steeringAction.Disable();
    }
    
    private void Update()
    {
        ProcessInput();
    }
    
    #endregion
    
    #region Movement

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
        }
    }

    private void Acceleration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
    
    private void Deceleration()
    {
        carRB.AddForceAtPosition( deceleration * moveInput * -transform.forward,accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        carRB.AddTorque(
            steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) *
            transform.up, ForceMode.Acceleration);
    }
    
    #endregion
    
    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;
        
        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }
        
        if (tempGroundedWheels > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    
    #endregion
    
    #region Input Handling
    
    private void ProcessInput()
    {
        Vector2 steeringInput = steeringAction.Steering.Steering.ReadValue<Vector2>();
        moveInput = steeringInput.y;
        steerInput = steeringInput.x;
    }
    
    #endregion
    
    #region Suspension Methods
    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;
            
            if (Physics.Raycast(rayPoints[i].position,-rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;
                
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;
                
                float springForce = springStiffness * springCompression;
                
                float netForce = springForce - dampForce;
                
                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);
                
                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;
                // Optional: Draw debug line for visualization when not hitting anything
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }
    
    #endregion

}
